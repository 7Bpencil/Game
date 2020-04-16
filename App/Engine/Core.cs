using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.Collision;
using App.Engine.PhysicsEngine.RigidBody;
using App.Model;
using App.Model.LevelData;
using App.View;

namespace App.Engine
{
    public class Core
    {
        private ViewForm viewForm;
        private Stopwatch clock;
        private KeyStates keyState;
        private Level currentLevel;
        private LevelManager levelManager;
        private Player player;
        private Sprite cursor;
        private Camera camera;
        private const int tileSize = 16;
        private bool isLevelLoaded;

        private List<Sprite> sprites;
        private List<RigidShape> collisionShapes;
        private List<CollisionInfo> collisionInfo;
        
        public Size CameraSize => camera.size;
        
        private class KeyStates
        {
            public bool W, S, A, D, I;
            public int pressesOnIAmount;
        }

        public Core(ViewForm viewForm, Size screenSize)
        {
            this.viewForm = viewForm;
            
            sprites = new List<Sprite>();
            collisionShapes = new List<RigidShape>();

            SetCamera(screenSize);
            SetLevels();
            var playerStartPosition = new Vector(14 * 32, 6 * 32);
            SetPlayer(playerStartPosition);
            SetCursor(playerStartPosition);
            keyState = new KeyStates();
            clock = new Stopwatch();

            var soundPlayer = new SoundPlayer {SoundLocation = @"Assets\Music\Magna_-_Divide.wav"};
            soundPlayer.Load();
            soundPlayer.Play();
        }
        
        private void SetCamera(Size cameraSize)
        {
            var p = cameraSize.Height / 3;
            var q = cameraSize.Height / 5;
            
            var walkableArea = new Rectangle(p, p, cameraSize.Width - 2 * p, cameraSize.Height - 2 * p);
            var cursorArea = new Rectangle(q, q, cameraSize.Width - 2 * q, cameraSize.Height - 2 * q);
            camera = new Camera(new Vector(0, 0), cameraSize, walkableArea, cursorArea);
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
                new Size(32, 32),
                4);
            
            var playerTorso = new Sprite
            (
                playerShape.Center,
                bmpPlayer,
                0,
                3,
                new Size(32, 32),
                4);
            
            player = new Player
            {
                Shape = playerShape,
                Torso = playerTorso,
                Legs = playerLegs
            };
            
            sprites.Add(playerLegs);
            sprites.Add(playerTorso);
            collisionShapes.Add(playerShape);
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
                new Size(32, 32),
                10);
            cursor.AnimationRate = 10;
        }

        public void GameLoop(object sender, EventArgs args)
        {
            if (!isLevelLoaded) LoadLevel();
            
            clock.Start();
            UpdateState();
            Render();
            
            clock.Stop();
            Console.WriteLine(clock.ElapsedMilliseconds);
            clock.Reset();
        }
        
        private void UpdateState()
        {
            const int step = 6;
            UpdatePlayerPosition(step);
            UpdatePlayerByMouse();
            CorrectPlayer();
            collisionInfo = CollisionSolver.ResolveCollisions(collisionShapes);
            camera.UpdateCamera(cursor.Center, player, currentLevel.LevelSizeInTiles, tileSize);
        }
        
        private void UpdatePlayerPosition(int step)
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
            
            RotatePlayerLegs(delta);
            player.Move(delta);
        }
        
        private void UpdatePlayerByMouse()
        {
            var playerCenterInCamera = player.Center.ConvertFromWorldToCamera(camera.position);
            var direction = cursor.Center - playerCenterInCamera;
            var dirAngle = Math.Atan2(-direction.Y, direction.X);
            var angle = 180 / Math.PI * dirAngle;
            player.Torso.Angle = angle;
        }
        
        private void RotatePlayerLegs(Vector delta)
        {
            var dirAngle = Math.Atan2(-delta.Y, delta.X);
            var angle = 180 / Math.PI * dirAngle;
            player.Legs.Angle = angle;
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
            
            player.Move(delta);
        }

        private void Render()
        {
            RerenderCamera();
            RenderSprites();
            
            if (keyState.pressesOnIAmount % 2 == 1)
                RenderDebugInfo();
            
            viewForm.Invalidate();
        }

        private void LoadLevel()
        {
            foreach (var layer in currentLevel.Layers)
                RenderLayer(layer);
            collisionShapes.AddRange(currentLevel.StaticShapes);
            isLevelLoaded = true;
        }
        
        private void RenderLayer(Layer layer)
        {
            for (var x = 0; x <= layer.WidthInTiles; ++x)
            for (var y = 0; y <= layer.HeightInTiles; ++y)
            {
                var tileIndex = y * layer.WidthInTiles + x;
                if (tileIndex > layer.Tiles.Length - 1) break;
                
                var tileID = layer.Tiles[tileIndex];
                if (tileID == 0) continue;
                
                RenderTile(x, y, tileID - 1, currentLevel.TileSet.image);
            }
        }

        private void RenderTile(int targetX, int targetY, int tileID, Bitmap tileMap)
        {
            var src = levelManager.GetSourceRectangle(tileID, tileMap.Width / tileSize, tileSize);
            viewForm.RenderTile(tileMap, targetX * tileSize, targetY * tileSize, src);
        }

        private void RerenderCamera()
        {
            var sourceRectangle = new Rectangle(
                (int) camera.position.X, (int) camera.position.Y,
                camera.size.Width, camera.size.Height);
            
            viewForm.RenderCamera(sourceRectangle);
        }
        
        private void RenderSprites()
        {
            foreach (var sprite in sprites)
                viewForm.RenderSpriteOnCamera(sprite, camera.position);    
            
            viewForm.RenderSpriteOnCamera(cursor);
        }
        
        private void RenderDebugInfo()
        {
            RenderStaticShapes();
            RenderRaytracingPolygons();
            RenderCollisionInfo();
            viewForm.PrintMessages(GetDebugInfo());
        }

        private void RenderStaticShapes()
        {
            foreach (var shape in currentLevel.StaticShapes)
                viewForm.RenderShapeOnCamera(shape, camera.position);
        }

        private void RenderCollisionInfo()
        {
            foreach (var info in collisionInfo)
                viewForm.RenderCollisionInfoOnCamera(info, camera.position);
        }
        
        private void RenderRaytracingPolygons()
        {
            foreach (var polygon in currentLevel.RaytracingShapes)
                viewForm.RenderPolygonOnCamera(polygon, camera.position);
        }
        
        private string[] GetDebugInfo()
        {
            return new []
            {
                "Camera Size: " + camera.size.Width + " x " + camera.size.Height,
                "Scene Size (in Tiles): " + currentLevel.LevelSizeInTiles.Width + " x " + currentLevel.LevelSizeInTiles.Height,
                "(WAxis) Scroll Position: " + camera.position,
                "(WAxis) Player Position: " + player.Center,
                "(CAxis) Player Position: " + player.Center.ConvertFromWorldToCamera(camera.position),
                "(CAxis) Cursor Position: " + cursor.Center
            };
        }

        public void OnMouseMove(Vector newPosition)
        {
            cursor.MoveTo(newPosition);
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

        public Size GetRenderSizeInTiles()
        {
            return currentLevel.LevelSizeInTiles;
        }

        public int GetTileSize()
        {
            return tileSize;
        }
    }
}