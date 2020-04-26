using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidBody;
using App.Engine.Render;
using App.Model;
using App.Model.Entities;
using App.Model.LevelData;
using App.Model.Weapons;
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
        private LevelManager levelManager;
        private Player player;
        private Sprite cursor;
        private Camera camera;
        private const int tileSize = 32;
        private bool isLevelLoaded;

        private List<Sprite> sprites;
        private List<CollisionInfo> collisionInfo;

        private class KeyStates
        {
            public bool W, S, A, D, I;
            public int pressesOnIAmount;
        }

        public Core(ViewForm viewForm, Size screenSize, RenderPipeline renderPipeline)
        {
            this.viewForm = viewForm;
            this.renderPipeline = renderPipeline;
            
            sprites = new List<Sprite>();
            
            SetLevels();
            var playerStartPosition = currentLevel.PlayerStartPosition;
            SetPlayer(playerStartPosition);
            camera = new Camera(playerStartPosition, player.Radius, screenSize);
            SetCursor(playerStartPosition);
            keyState = new KeyStates();
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
            cursor = new Sprite
            (
                position,
                bmpCursor,
                0,
                9,
                new Size(64, 64),
                10);
            sprites.Add(cursor);
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
            renderPipeline.Start(camera.Position, camera.Size, sprites);
            
            if (keyState.pressesOnIAmount % 2 == 1)
                renderPipeline.RenderDebugInfo(
                    camera.Position, camera.Size, currentLevel.Shapes, collisionInfo,
                    currentLevel.RaytracingEdges, cursor.Center, player.Center,
                    camera.GetChaser(), currentLevel.LevelSizeInTiles);
            
            clock.Stop();
            Console.WriteLine(clock.ElapsedMilliseconds);
            clock.Reset();
        }
        
        private void UpdateState()
        {
            const int step = 6;
            var previousPosition = player.Center.Copy();
            var velocity = UpdatePlayerPosition(step);
            RotatePlayerLegs(velocity);
            CorrectPlayer();
            collisionInfo = CollisionSolver.ResolveCollisions(currentLevel.Shapes);
            
            var positionDelta = player.Center - previousPosition;
            cursor.MoveBy(viewForm.GetCursorDiff() + positionDelta);
            UpdatePlayerByMouse();
            camera.UpdateCamera(player.Center, velocity, cursor.Center, step);
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
            var direction = cursor.Center - player.Center;
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
    }
}