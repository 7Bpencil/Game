﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Thread = System.Threading.Thread;
using App.Engine.Audio;
using App.Engine.Particles;
using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Render;
using App.Engine.Sprites;
using App.Model;
using App.Model.Entities;
using App.Model.Entities.Weapons;
using App.Model.LevelData;
using App.View;

namespace App.Engine
{
    public class Core
    {
        private ViewForm viewForm;
        private RenderPipeline renderPipeline;
        private Stopwatch clock;
        private KeyStates keyState;
        private Level currentLevel;
        private MouseState mouseState;
        private LevelManager levelManager;
        private Player player;
        private CustomCursor cursor;
        private Camera camera;
        private const int tileSize = 32;
        private bool isLevelLoaded;

        private List<SpriteContainer> sprites;
        private List<AbstractParticleUnit> particles;
        private List<CollisionInfo> collisionInfo;
        private List<Bullet> bullets;
        private List<ShootingRangeTarget> targets;
        private List<Collectable> collectables;
        private List<Raytracing.VisibilityRegion> visibilityRegions;
        private List<List<Vector>> paths;

        private readonly WeaponFactory<AK303> AKfactory;
        private readonly WeaponFactory<Shotgun> ShotgunFactory;
        private readonly WeaponFactory<SaigaFA> SaigaFAfactory;
        private readonly WeaponFactory<MP6> MP6factory;
        private readonly ParticleFactory particleFactory;

        private string updateTime;

        private class KeyStates
        {
            public bool W, S, A, D, I;
            public int pressesOnIAmount;
        }

        private class MouseState
        {
            public bool LMB, RMB;
        }

        public Core(ViewForm viewForm, Size screenSize, RenderPipeline renderPipeline)
        {
            this.viewForm = viewForm;
            this.renderPipeline = renderPipeline;
            
            AKfactory = AbstractWeaponFactory.CreateAK303factory();
            ShotgunFactory = AbstractWeaponFactory.CreateShotgunFactory();
            SaigaFAfactory = AbstractWeaponFactory.CreateSaigaFAfactory();
            MP6factory = AbstractWeaponFactory.CreateMP6factory();
            particleFactory = new ParticleFactory();

            sprites = new List<SpriteContainer> {Capacity = 50};
            particles = new List<AbstractParticleUnit> {Capacity = 500};
            bullets = new List<Bullet> {Capacity = 500};

            SetLevels();
            SetTargets();
            var playerStartPosition = new Vector(720, 930);
            SetPlayer(playerStartPosition);
            camera = new Camera(playerStartPosition, player.Radius, screenSize);
            SetCursor(playerStartPosition);
            keyState = new KeyStates();
            mouseState = new MouseState();
            clock = new Stopwatch();
            
            AudioEngine.Initialize();
            while (!AudioEngine.Ready)
                Thread.Sleep(1);

            //AudioEngine.PlayNewInstance(@"event:/themes/THEME");
        }

        private void SetLevels()
        {
            levelManager = new LevelManager();
            currentLevel = levelManager.CurrentLevel;
            
            collectables = new List<Collectable>
            {
                AKfactory.GetCollectable(new Vector(600, 400), 45, 20),
                SaigaFAfactory.GetCollectable(new Vector(600, 500), 0, 8)
            };

            foreach (var collectable in collectables)
                sprites.Add(collectable.SpriteContainer);
        }
        
        private void SetPlayer(Vector position)
        {
            var weapons = new List<Weapon>
            {
                ShotgunFactory.GetNewGun(8),
                MP6factory.GetNewGun(40)
            };
            player = EntityCreator.CreatePlayer(position, 0, weapons, currentLevel);

            sprites.Add(player.LegsContainer);
            sprites.Add(player.TorsoContainer);
            currentLevel.DynmaicShapes.Add(player.Shape);
            currentLevel.DynmaicShapes.AddRange(player.MeleeWeapon.range);
        }

        private void SetCursor(Vector position)
        {
            var bmpCursor = levelManager.GetTileMap("crosshair.png");
            var cursorSprite = new Sprite
            (
                bmpCursor,
                3,
                0,
                9,
                new Size(64, 64));
            
            cursor = new CustomCursor(position.Copy(), cursorSprite);
            sprites.Add(cursor.SpriteContainer);
        }

        private void SetTargets()
        {
            var targetAmmo = 1000;
            var position = new Vector(720, 920);
            var legs = new PlayerBodySprite(position, new Bitmap(@"Assets\TileMaps\boroda.png"), 1, 14, 27, new Size(64, 64));
            var torso = new StaticSprite(new Bitmap(@"Assets\Sprites\Weapons\top_down_view_boroda.png"), 0, new Size(79, 57));
            targets = new List<ShootingRangeTarget>
            {
                new ShootingRangeTarget(currentLevel, player, legs, torso, 90f,
                    100,
                    300,
                    position,
                    new Vector(5, 0),
                    60, AKfactory.GetNewGun(targetAmmo), bullets)
            };

            foreach (var t in targets)
            {
                sprites.Add(t.LegsContainer);
                sprites.Add(t.TorsoContainer);
                currentLevel.DynmaicShapes.Add(t.collisionShape);
            }
            
        }

        public void GameLoop(object sender, EventArgs args)
        {
            if (!isLevelLoaded)
            {
                renderPipeline.Load(currentLevel);
                isLevelLoaded = true;
            }
            
            clock.Start();
            
            UpdateState();
            
            clock.Stop();
            var lTime = clock.ElapsedMilliseconds;
            clock.Reset();
            clock.Start();
            
            renderPipeline.Start(
                player.Position, camera.Position, camera.Size, player.CurrentWeapon,
                sprites, particles, bullets, visibilityRegions);
            
            if (keyState.pressesOnIAmount % 2 == 1)
                renderPipeline.RenderDebugInfo(
                    camera.Position, camera.Size, currentLevel.Shapes, targets, collisionInfo,
                    currentLevel.RaytracingEdges, collectables, bullets, cursor.Position, player.Position,
                    camera.GetChaser(), GetDebugInfo(), paths);
            
            AudioEngine.Update();
            
            clock.Stop();
            var rTime = clock.ElapsedMilliseconds;
            clock.Reset();
            updateTime = "logic=" + lTime + "ms render=" + rTime + "ms";
        }
        
        private void UpdateState()
        {
            const int step = 6;
            var previousPosition = player.Position.Copy();
            var playerVelocity = UpdatePlayerPosition(step);
            CorrectPlayer();
            
            collisionInfo = CollisionSolver.ResolveCollisions(currentLevel.Shapes);
            var positionDelta = player.Position - previousPosition;
            cursor.MoveBy(viewForm.GetCursorDiff() + positionDelta);
            player.MeleeWeapon.MoveRangeShapeBy(positionDelta);
            UpdatePlayerByMouse();
            RotatePlayerLegs(playerVelocity);

            if (mouseState.RMB && player.MeleeWeapon.IsReady)
            {
                player.TakeMeleeWeapon();
                var wasHit = player.MeleeWeapon.Attack(targets);
                if (wasHit) particles.Add(particleFactory.CreateBigBloodSplash(player.Position + (cursor.Position - player.Position).Normalize() * player.Radius * 3));
            }
            else if (mouseState.LMB && player.CurrentWeapon.IsReady() && player.MeleeWeapon.IsReady)
            {
                player.HideMeleeWeapon();
                var firedBullets = player.CurrentWeapon.Fire(player.Position, player.Position, cursor);
                AudioEngine.PlayNewInstance("event:/gunfire/misc/dropped_shell", player.Position, player.Position);
                bullets.AddRange(firedBullets);
                particles.Add(particleFactory.CreateShell(player.Position, cursor.Position - player.Position, player.CurrentWeapon));
            }

            UpdateCollectables();
            UpdateEntities();
            
            camera.UpdateCamera(player.Position, playerVelocity, cursor.Position, step);
            viewForm.CursorReset();

            foreach (var spriteContainer in sprites)
                if (!spriteContainer.IsEmpty()) spriteContainer.Content.UpdateFrame();
            foreach (var unit in particles)
                unit.UpdateFrame();
        }
        
        private Vector UpdatePlayerPosition(int step)
        {
            var delta = Vector.ZeroVector;
            if (keyState.W) 
                delta.Y -= step;
            if (keyState.S)
                delta.Y += step;
            if (keyState.A)
                delta.X -= step;
            if (keyState.D)
                delta.X += step;
            
            player.MoveBy(delta);
            return delta;
        }
        
        private void UpdatePlayerByMouse()
        {
            var direction = cursor.Position - player.Position;
            var dirAngle = direction.Angle;
            var angle = (float) (180 / Math.PI * dirAngle);
            player.TorsoContainer.Angle = angle;
            player.MeleeWeapon.RotateRangeShape(angle, player.Position);
            player.viewVector = direction.Normalize();
        }
        
        private void RotatePlayerLegs(Vector delta)
        {
            if (delta.Equals(Vector.ZeroVector)) return;
            var dirAngle = Math.Atan2(-delta.Y, delta.X);
            var angle = 180 / Math.PI * dirAngle;
            player.LegsContainer.Angle = (float) angle;
        }
        
        private void CorrectPlayer()
        {
            var delta = Vector.ZeroVector;
            var rightBorder = currentLevel.LevelSizeInTiles.Width * tileSize;
            const int leftBorder = 0;
            var bottomBorder = currentLevel.LevelSizeInTiles.Height * tileSize;
            const int topBorder = 0;

            var a = player.Position.Y - player.Radius - topBorder;
            var b = player.Position.Y + player.Radius - bottomBorder;
            var c = player.Position.X - player.Radius - leftBorder;
            var d = player.Position.X + player.Radius - rightBorder;

            if (a < 0) delta.Y -= a;
            if (b > 0) delta.Y -= b;
            if (c < 0) delta.X -= c;
            if (d > 0) delta.X -= d;
            
            player.MoveBy(delta);
        }

        private void UpdateEntities()
        {
            UpdateBullets();
            UpdateTargets();

            player.IncrementWeaponsTick();
        }

        private void UpdateBullets()
        {
            foreach (var bullet in bullets)
            {
                if (bullet.IsStuck) continue;
                if (bullet.StaticPenetrations.Count == 0)
                    CalculateStaticPenetrations(bullet);
                CalculateDynamicPenetrations(bullet);
                bullet.Update();
                if (bullet.ClosestPenetrationPoint != null)
                {
                    particles.Add(particleFactory.CreateWallDust(bullet.ClosestPenetrationPoint, bullet.Velocity));
                    bullet.ClosestPenetrationPoint = null;
                }
            }
        }

        private void CalculateStaticPenetrations(Bullet bullet)
        {
            foreach (var obstacle in currentLevel.StaticShapes)
            {
                var penetrationTime = BulletCollisionDetector.AreCollideWithStatic(bullet, obstacle);
                if (penetrationTime == null) continue;
                var distanceToPenetrations = new float[2];
                for (var i = 0; i < 2; i++)
                    distanceToPenetrations[i] = bullet.Speed * penetrationTime[i];
                bullet.StaticPenetrations.Add(distanceToPenetrations);
            }
            bullet.CalculateTrajectory();
        }

        private void CalculateDynamicPenetrations(Bullet bullet)
        {
            foreach (var target in targets)
            {
                var penetrationTimes = 
                    BulletCollisionDetector.AreCollideWithDynamic(bullet, target.collisionShape, target.Velocity);
                if (penetrationTimes == null) continue;
                var penetrationPlace = bullet.Position + bullet.Velocity * penetrationTimes[0];
                target.TakeHit(bullet.Damage);
                if (target.Armour > 50)
                {
                    bullet.IsStuck = true;
                    //AudioEngine.PlayNewInstance("event:/gunfire/hit_sounds/hit_armor", penetrationPlace, player.Position);
                }
                
                particles.Add(particleFactory.CreateBloodSplash(penetrationPlace));
                particles.Add(particleFactory.CreateBloodSplash(penetrationPlace));

                if (target.IsDead && !target.Velocity.Equals(Vector.ZeroVector))
                    target.MoveTo(target.collisionShape.Center + target.Velocity * penetrationTimes[0]);
            }
        }

        /// <summary>
        /// Updates AI - it should be called every tick
        /// </summary>
        private void UpdateTargets()
        {
            paths = new List<List<Vector>>();
            visibilityRegions = new List<Raytracing.VisibilityRegion> {Capacity = 8};
            visibilityRegions.Add(new Raytracing.VisibilityRegion(player.Position, currentLevel.RaytracingEdges, 1000));
            foreach (var t in targets)
            {
                if (t.IsDead) t.ChangeVelocity(Vector.ZeroVector);
                else
                {
                    var view = new Raytracing.VisibilityRegion(t.Center, currentLevel.RaytracingEdges, 1000);
                    visibilityRegions.Add(view);
                    t.aim = Raytracing.IsInView(player.Shape, view);
                    paths.Add(t.currentPath);
                }
                t.Update();
            }
        }

        private void UpdateCollectables()
        {
            for (var i = 0; i < collectables.Count; i++)
            {
                if (collectables[i] == null) continue;
                var collision = CollisionSolver.GetCollisionInfo(player.Shape, collectables[i].CollisionShape);
                if (collision == null) continue;
                player.AddWeapon(collectables[i].Item);
                collectables[i].SpriteContainer.ClearContent();
                collectables[i] = null;
            }
        }
        
        private string[] GetDebugInfo()
        {
            return new []
            {
                updateTime,
                "Camera Size: " + camera.Size.Width + " x " + camera.Size.Height,
                "Scene Size (in Tiles): " + currentLevel.LevelSizeInTiles.Width + " x " + currentLevel.LevelSizeInTiles.Height,
                "(WAxis) Scroll Position: " + camera.Position,
                "(WAxis) Player Position: " + player.Position,
                "(CAxis) Player Position: " + player.Position.ConvertFromWorldToCamera(camera.Position),
                "(CAxis) Cursor Position: " + cursor.Position
            };
        }

        public void OnKeyDown(Keys keyPressed)
        {
            switch (keyPressed)
            {
                case Keys.W:
                    keyState.W = true;
                    break;

                case Keys.S:
                    keyState.S = true;
                    break;

                case Keys.A:
                    keyState.A = true;
                    break;

                case Keys.D:
                    keyState.D = true;
                    break;
                
                case Keys.I:
                    keyState.I = true;
                    keyState.pressesOnIAmount++;
                    break;
            }
        }

        public void OnKeyUp(Keys keyPressed)
        {
            switch (keyPressed)
            {
                case Keys.Escape:
                    AudioEngine.Release();
                    Application.Exit();
                    break;
                
                case Keys.W:
                    keyState.W = false;
                    break;

                case Keys.S:
                    keyState.S = false;
                    break;

                case Keys.A:
                    keyState.A = false;
                    break;

                case Keys.D:
                    keyState.D = false;
                    break;
                
                case Keys.I:
                    keyState.I = false;
                    break;
            }
        }
        
        public void OnMouseWheel(int wheelDelta)
        {
            if (wheelDelta > 0) player.MoveNextWeapon();
            else player.MovePreviousWeapon();
        }

        public void OnMouseDown(MouseButtons buttons)
        {
            switch (buttons)
            {
                case MouseButtons.Left:
                    mouseState.LMB = true;
                    break;
                
                case MouseButtons.Right:
                    mouseState.RMB = true;
                    break;
            }
        }

        public void OnMouseUp(MouseButtons buttons)
        {
            switch (buttons)
            {
                case MouseButtons.Left:
                    mouseState.LMB = false;
                    break;
                
                case MouseButtons.Right:
                    mouseState.RMB = false;
                    break;
            }
        }
    }
}