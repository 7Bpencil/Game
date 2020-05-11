using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using App.Engine.Audio;
using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Render;
using App.Model;
using App.Model.Entities;
using App.Model.Factories;
using App.Model.LevelData;
using App.View;

namespace App.Engine
{
    public class Core
    {
        private readonly ViewForm viewForm;
        private Size screenSize;

        private KeyStates keyState;
        private MouseState mouseState;
        private CustomCursor cursor;
        private Camera camera;
        
        private Level currentLevel;
        private Player player;
        private int livingBotsAmount;
        
        private const int tileSize = 32;
        
        private class KeyStates
        {
            public bool W, S, A, D, I, R, P;
            public int pressesOnIAmount;
            public int pressesOnPAmount;
        }

        private class MouseState
        {
            public bool LMB, RMB;
        }
        
        public Core(ViewForm viewForm, Size screenSize)
        {
            this.viewForm = viewForm;
            this.screenSize = screenSize;
            InitializeSystems();
            currentLevel = LevelManager.LoadLevel(0);
            InitState();

            AudioEngine.PlayNewInstance(@"event:/themes/THEME");
        }
        
        private void InitializeSystems()
        {
            RenderMachine.Initialize(viewForm, screenSize);
            AudioEngine.Initialize();
            LevelManager.Initialize();
            ParticleFactory.Initialize();
            AbstractWeaponFactory.Initialize();
            BotBank.Initialize();
        }
        
        private void InitState()
        {
            AStarSearch.SetMesh(currentLevel.NavMesh);
            player = currentLevel.Player;
            camera = new Camera(player.Position, player.Radius, screenSize);
            cursor = new CustomCursor(player.Position.Copy());
            currentLevel.Sprites.Add(cursor.SpriteContainer);
            keyState = new KeyStates();
            mouseState = new MouseState();
            livingBotsAmount = currentLevel.Bots.Count;
        }
        
        private void ResetState()
        {
            currentLevel.Reset();
            player = currentLevel.Player;
            cursor.MoveTo(player.Position);
            currentLevel.Sprites.Add(cursor.SpriteContainer);
            camera.Reset(player.Position);
            keyState = new KeyStates();
            mouseState = new MouseState();
        }
        
        public void GameLoop(object sender, EventArgs args)
        {
            if (currentLevel.WavesAmount == 0)
            {
                currentLevel.IsCompleted = true;
                currentLevel.Sprites.Add(SpriteFactory.CreateExitSprite(currentLevel.Exit));
            }
            if (player.IsDead) ResetState();
            if (currentLevel.IsCompleted 
                && CollisionDetector.GetCollisionInfo(player.CollisionShape, currentLevel.Exit) != null)
            {
                currentLevel = LevelManager.LoadLevel(1);
                InitState();
            }

            UpdateState();

            var renderRaytracing = keyState.pressesOnPAmount % 2 == 1;
            var renderDebug = keyState.pressesOnIAmount % 2 == 1; 
            RenderPipeline.Render(currentLevel, camera, cursor.Position, renderRaytracing, renderDebug);

            AudioEngine.Update();
        }
        
        private void UpdateState()
        {
            const int step = 6;
            var previousPosition = player.Position.Copy();
            var playerVelocity = UpdatePlayerPosition(step);
            CorrectPlayer();
            
            currentLevel.CollisionsInfo = CollisionSolver.ResolveCollisions(currentLevel.SceneShapes);
            AudioEngine.UpdateListenerPosition(player.Position);
            var positionDelta = player.Position - previousPosition;
            player.Velocity = positionDelta;
            cursor.MoveBy(viewForm.GetCursorDiff() + positionDelta);
            player.MeleeWeapon.MoveRangeBy(positionDelta);
            UpdatePlayerByMouse();
            RotatePlayerLegs(playerVelocity);

            if (mouseState.RMB && player.MeleeWeapon.IsReady)
            {
                player.RaiseMeleeWeapon();
            }
            else if (mouseState.LMB && player.CurrentWeapon.IsReady && player.MeleeWeapon.IsReady)
            {
                player.HideMeleeWeapon();
                var firedBullets = player.CurrentWeapon.Fire(player.Position, cursor);
                AudioEngine.PlayNewInstance("event:/gunfire/2D/misc/DROPPED_SHELL");
                currentLevel.Bullets.AddRange(firedBullets);
                currentLevel.Particles.Add(ParticleFactory.CreateShell(player.Position, cursor.Position - player.Position, player.CurrentWeapon));
            }

            UpdateCollectables();
            UpdateEntities();
            
            camera.UpdateCamera(player.Position, playerVelocity, cursor.Position, step);
            viewForm.CursorReset();

            foreach (var spriteContainer in currentLevel.Sprites)
                if (!spriteContainer.IsEmpty()) spriteContainer.Content.UpdateFrame();
            foreach (var unit in currentLevel.Particles)
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
            player.MeleeWeapon.RotateRangeTo(angle, player.Position);
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
            UpdateBots();

            player.IncrementWeaponsTick();
        }

        private void UpdateBullets()
        {
            var bullets = currentLevel.Bullets;
            var particles = currentLevel.Particles;
            foreach (var bullet in bullets)
            {
                if (bullet.IsStuck) continue;
                if (bullet.StaticPenetrations.Count == 0)
                    CalculateStaticPenetrations(bullet);
                CalculateDynamicPenetrations(bullet);
                bullet.Update();
                if (bullet.ClosestPenetrationPoint != null)
                {
                    particles.Add(ParticleFactory.CreateWallDust(bullet.ClosestPenetrationPoint, bullet.Velocity));
                    bullet.ClosestPenetrationPoint = null;
                }
            }
        }

        private void CalculateStaticPenetrations(Bullet bullet)
        {
            var staticShapes = currentLevel.StaticShapes;
            foreach (var obstacle in staticShapes)
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
            var bots = currentLevel.Bots;
            var particles = currentLevel.Particles;
            foreach (var bot in bots)
            {
                var penetrationTimes = 
                    BulletCollisionDetector.AreCollideWithDynamic(bullet, bot.CollisionShape, bot.Velocity);
                if (penetrationTimes == null) continue;
                var penetrationPlace = bullet.Position + bullet.Velocity * penetrationTimes[0];
                bot.TakeHit(bullet.Damage);
                if (bot.Armor > 50) bullet.IsStuck = true;
                else bullet.SlowDown();

                particles.Add(ParticleFactory.CreateBloodSplash(penetrationPlace));
                particles.Add(ParticleFactory.CreateBloodSplash(penetrationPlace));

                if (bot.IsDead && !bot.Velocity.Equals(Vector.ZeroVector))
                    bot.MoveTo(bot.CollisionShape.Center + bot.Velocity * penetrationTimes[0]);
            }
            
            // TODO remove copy-paste
            var pTimes = 
                BulletCollisionDetector.AreCollideWithDynamic(bullet, player.CollisionShape, player.Velocity);
            if (pTimes == null) return;
            var pPlace = bullet.Position + bullet.Velocity * pTimes[0];
            player.TakeHit(bullet.Damage);
            if (player.Armor > 50) bullet.IsStuck = true;
            else bullet.SlowDown();

            particles.Add(ParticleFactory.CreateBloodSplash(pPlace));
            particles.Add(ParticleFactory.CreateBloodSplash(pPlace));

            if (player.IsDead && !player.Velocity.Equals(Vector.ZeroVector))
                player.MoveTo(player.CollisionShape.Center + player.Velocity * pTimes[0]);
            
        }
        
        private void UpdateBots()
        {
            if (livingBotsAmount == 0 && currentLevel.WavesAmount != 0)
            {
                LevelDynamicEntitiesFactory.SpawnBots(
                    currentLevel.BotSpawnPoints, player.Position, currentLevel.Bots, currentLevel.Sprites, currentLevel.DynamicShapes);
                livingBotsAmount += currentLevel.BotSpawnPoints.Count;
                currentLevel.WavesAmount--;
            }
            var regions = new List<Raytracing.VisibilityRegion>();
            regions.Add(new Raytracing.VisibilityRegion(player.Position, currentLevel.RaytracingEdges, 1000));
            var paths = new List<List<Vector>> {Capacity = 10};
            var bots = currentLevel.Bots;
            livingBotsAmount = 0;
            foreach (var bot in bots)
            {
                if (bot.IsDead) continue;
                if (player.WasMeleeWeaponRaised && player.MeleeWeapon.IsInRange(bot))
                {
                    bot.TakeHit(player.MeleeWeapon.Damage);
                    var particlePosition = player.Position + (bot.Center - player.Position).Normalize() * player.Radius * 3;
                    currentLevel.Particles.Add(ParticleFactory.CreateBigBloodSplash(particlePosition));
                    currentLevel.Particles.Add(ParticleFactory.CreateBigBloodSplash(particlePosition));
                }
                bot.Update(player.Position, currentLevel.Bullets, currentLevel.Particles, currentLevel.SceneShapes, paths, currentLevel.RaytracingEdges);
                if (!bot.IsDead) livingBotsAmount++;
            }

            currentLevel.VisibilityRegions = regions;
            currentLevel.Paths = paths;
        }

        private void UpdateCollectables() // TODO
        {
            var collectables = currentLevel.Collectables;
            foreach (var collectable in collectables)
            {
                if (collectable.IsPicked) continue;
                var collision = CollisionDetector.GetCollisionInfo(player.CollisionShape, collectable.CollisionShape);
                if (collision == null) continue;
                collectable.Pick(player);
            }
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
                
                case Keys.R:
                    keyState.R = true;
                    break;
                
                case Keys.P:
                    keyState.P = true;
                    keyState.pressesOnPAmount++;
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
                
                case Keys.R:
                    keyState.R = false;
                    break;
                
                case Keys.P:
                    keyState.P = false;
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