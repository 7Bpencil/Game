using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidShape;
using App.Engine.Render;
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

        private List<Sprite> sprites;
        private List<CollisionInfo> collisionInfo;
        private List<Bullet> bullets;
        private List<ShootingRangeTarget> targets;

        private class KeyStates
        {
            public bool W, S, A, D, I;
            public int pressesOnIAmount;
        }

        private class MouseState
        {
            public bool LMB;
        }

        public Core(ViewForm viewForm, Size screenSize, RenderPipeline renderPipeline)
        {
            this.viewForm = viewForm;
            this.renderPipeline = renderPipeline;
            
            sprites = new List<Sprite>();
            bullets = new List<Bullet>();
            
            SetLevels();
            SetTargets();
            var playerStartPosition = currentLevel.PlayerStartPosition;
            SetPlayer(playerStartPosition);
            camera = new Camera(playerStartPosition, player.Radius, screenSize);
            SetCursor(playerStartPosition);
            keyState = new KeyStates();
            mouseState = new MouseState();
            clock = new Stopwatch();
            
            var musicPlayer = new MusicPlayer();
            var soundEngineThread = new Thread(() => musicPlayer.PlayPlaylist());
            soundEngineThread.Start();
        }

        private void SetLevels()
        {
            levelManager = new LevelManager();
            currentLevel = levelManager.CurrentLevel;
        }
        
        private void SetPlayer(Vector position)
        {
            var bmpPlayer = levelManager.GetTileMap("boroda.png");
            var playerShape = new RigidCircle(position, bmpPlayer.Height / 4, false, true);
            
            var playerLegs = new Sprite
            (
                playerShape.Center,
                bmpPlayer,
                4,
                7,
                new Size(64, 64),
                4);

            var playerTorso = new Sprite
            (
                playerShape.Center,
                bmpPlayer,
                0,
                3,
                new Size(64, 64),
                4);

            var weapons = new List<Weapon>
            {
                new AK303(30),
                new Shotgun(8),
                new SaigaFA(20),
                new MP6(40)
            };
            player = new Player(playerShape, playerTorso, playerLegs, weapons);
            
            sprites.Add(player.Legs);
            sprites.Add(player.Torso);
            currentLevel.Shapes.Add(playerShape);
        }

        private void SetCursor(Vector position)
        {
            var bmpCursor = levelManager.GetTileMap("crosshair.png");

            var cursorPosition = position.Copy();
            var shape = new RigidCircle(cursorPosition, 3, false, true);
            var cursorSprite = new Sprite
            (
                cursorPosition,
                bmpCursor,
                0,
                9,
                new Size(64, 64),
                10);
            
            cursor = new CustomCursor(shape, cursorSprite);
            sprites.Add(cursorSprite);
        }

        private void SetTargets()
        {
            targets = new List<ShootingRangeTarget>
            {
                new ShootingRangeTarget(
                    100,
                    50,
                    new RigidCircle(new Vector(50, 260), 30, false, true),
                    new Vector(5, 0),
                    80,
                    false),
                new ShootingRangeTarget(
                    200,
                    100,
                    new RigidCircle(new Vector(450, 580), 30, false, true),
                    new Vector(-5, 0),
                    80,
                    false),
                new ShootingRangeTarget(
                    50,
                    300,
                    new RigidCircle(new Vector(40, 200), 30, false, true),
                    new Vector(1, 5),
                    80,
                    false)
            };
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
            renderPipeline.Start(player.Center, camera.Position, camera.Size, sprites, currentLevel.RaytracingEdges);
            
            if (keyState.pressesOnIAmount % 2 == 1)
                renderPipeline.RenderDebugInfo(
                    camera.Position, camera.Size, currentLevel.Shapes, collisionInfo,
                    currentLevel.RaytracingEdges, cursor.Position, player.Center,
                    camera.GetChaser(), currentLevel.LevelSizeInTiles);
            
            clock.Stop();
            Console.WriteLine(clock.ElapsedMilliseconds);
            clock.Reset();
        }
        
        private void UpdateState()
        {
            const int step = 6;
            var previousPosition = player.Center.Copy();
            var playerVelocity = UpdatePlayerPosition(step);
            CorrectPlayer();
            
            collisionInfo = CollisionSolver.ResolveCollisions(currentLevel.Shapes);
            var positionDelta = player.Center - previousPosition;
            cursor.MoveBy(viewForm.GetCursorDiff() + positionDelta);
            UpdatePlayerByMouse();
            RotatePlayerLegs(playerVelocity);

            if (mouseState.LMB)
            {
                var firedBullets = player.CurrentWeapon.Fire(player.Center, cursor);
                if (firedBullets != null) bullets.AddRange(firedBullets);
            }
            
            UpdateEntities();
            
            camera.UpdateCamera(player.Center, playerVelocity, cursor.Position, step);
            viewForm.CursorReset();
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
            var direction = cursor.Position - player.Center;
            var dirAngle = Math.Atan2(-direction.Y, direction.X);
            var angle = 180 / Math.PI * dirAngle;
            player.Torso.Angle = angle;
        }
        
        private void RotatePlayerLegs(Vector delta)
        {
            var dirAngle = Math.Atan2(-delta.Y, delta.X);
            var angle = 180 / Math.PI * dirAngle;
            if (!delta.Equals(Vector.ZeroVector)) player.Legs.Angle = angle;
        }
        
        private void CorrectPlayer()
        {
            var delta = Vector.ZeroVector;
            var rightBorder = currentLevel.LevelSizeInTiles.Width * tileSize;
            const int leftBorder = 0;
            var bottomBorder = currentLevel.LevelSizeInTiles.Height * tileSize;
            const int topBorder = 0;

            var a = player.Center.Y - player.Radius - topBorder;
            var b = player.Center.Y + player.Radius - bottomBorder;
            var c = player.Center.X - player.Radius - leftBorder;
            var d = player.Center.X + player.Radius - rightBorder;

            if (a < 0) delta.Y -= a;
            if (b > 0) delta.Y -= b;
            if (c < 0) delta.X -= c;
            if (d > 0) delta.X -= d;
            
            player.MoveBy(delta);
        }

        private void UpdateEntities()
        {
            for (var i = 0; i < bullets.Count; i++)
            {
                if (bullets[i] == null) continue;
                if (bullets[i].collisionWithStaticInfo.Count == 0)
                    foreach (var obstacle in currentLevel.Shapes)
                    {
                        var penetrationTime = BulletCollisionSolver.AreCollideWithStatic(bullets[i], obstacle);
                        if (penetrationTime == null) continue;

                        var penetrationPlace = new []
                        {
                            bullets[i].position + bullets[i].velocity * penetrationTime[0],
                            bullets[i].position + bullets[i].velocity * penetrationTime[1]
                        };

                        bullets[i].collisionWithStaticInfo.Add(penetrationPlace);
                    }

                foreach (var target in targets)
                {
                    var c = BulletCollisionSolver.AreCollideWithDynamic(bullets[i], target.collisionShape, target.Velocity);
                    if (c == null) continue;

                    var a = new List<Vector>();
                    foreach (var time in c)
                        a.Add(bullets[i].position + bullets[i].velocity * time);

                    bullets[i].collisionWithDynamicInfo.AddRange(a);
                    target.TakeHit(bullets[i].damage);
                    if (target.isDead)
                    {
                        target.MoveTo(target.collisionShape.Center + target.Velocity * c[0]);
                        target.ChangeVelocity(Vector.ZeroVector);
                    }
                }

                var e = bullets[i].shape.End - bullets[i].shape.Start;
                var shouldBeDestroyed = false;
                foreach (var vectorPair in bullets[i].collisionWithStaticInfo)
                {
                    if (Vector.ScalarProduct(e, vectorPair[0] - bullets[i].shape.Start) < -2 
                        && !bullets[i].CanPenetrate(vectorPair))
                    {
                        shouldBeDestroyed = true;
                    }
                }

                if (shouldBeDestroyed) bullets[i] = null;
            }

            foreach (var b in bullets)
            {
                b?.Move();
            }

            foreach (var t in targets)
            {
                t.IncrementTick();
            }
            
            player.IncrementWeaponsTick();
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
            if (buttons == MouseButtons.Left) mouseState.LMB = true;
        }

        public void OnMouseUp(MouseButtons buttons)
        {
            if (buttons == MouseButtons.Left) mouseState.LMB = false;
        }
    }
}