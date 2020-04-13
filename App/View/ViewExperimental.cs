using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.RigidBody;
using App.Model;
using App.Model.LevelData;
using App.Model.Parser;
using App.View.Renderings;

namespace App.View
{
    public class ViewExperimental : Form
    {
        private Level currentLevel;
        
        private KeyStates keyState;
        private const int tileSize = 64;
        
        private static Size renderSizeInTiles;
        private Camera camera;
        private Vector previousTopLeftTileIndex;
        
        private Vector cursorPosition;
        private Player player;
        
        private Bitmap bmpPlayer;
        
        private Bitmap bmpRenderedTiles;
        private Graphics gfxRenderedTiles;
        
        private BufferedGraphics cameraBuffer;
        private Graphics gfxCamera;
        
        private Font debugFont;
        private Brush debugBrush;

        private Stopwatch clock;

        private LevelManager levelManager;

        private class KeyStates
        {
            public bool W, S, A, D;
        }
        
        public ViewExperimental()
        {
            
            levelManager = new LevelManager();
            currentLevel = levelManager.CurrentLevel;
            
            DoubleBuffered = true;
            SetUpView();
            currentLevel = LevelParser.ParseLevel("Levels/secondTry.tmx");

            keyState = new KeyStates();
            
            var playerStartPosition = new Vector(14 * 64, 6 * 64);
            SetUpPlayer(playerStartPosition);
            cursorPosition = playerStartPosition;
            
            //create and initialize renderer
            bmpRenderedTiles = new Bitmap(renderSizeInTiles.Width * tileSize, renderSizeInTiles.Height * tileSize);
            gfxRenderedTiles = Graphics.FromImage(bmpRenderedTiles);
            
            var context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(camera.size.Width + 1, camera.size.Height + 1);
            using (var g = CreateGraphics())
                cameraBuffer = context.Allocate(g, new Rectangle(0, 0, camera.size.Width, camera.size.Height));
            gfxCamera = cameraBuffer.Graphics;
            gfxCamera.InterpolationMode = InterpolationMode.Bilinear;
            
            previousTopLeftTileIndex = new Vector(0, 0);
            
            debugFont = new Font("Arial", 18, FontStyle.Regular, GraphicsUnit.Pixel);
            debugBrush = new SolidBrush(Color.White);
            
            RerenderCamera();
            clock = new Stopwatch();
            
            var timer = new Timer();
            timer.Interval = 15;
            timer.Tick += TimerTick;
            timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            cameraBuffer.Render(e.Graphics);
        }

        private void SetUpView()
        {
            var cameraSize = new Size(1280, 720);
            var p = cameraSize.Height / 3;
            var walkableArea = new Rectangle(p, p, cameraSize.Width - 2 * p, cameraSize.Height - 2 * p);
            var q = cameraSize.Height / 5;
            var cursorArea = new Rectangle(q, q, cameraSize.Width - 2 * q, cameraSize.Height - 2 * q);
            camera = new Camera(new Vector(250, 100), cameraSize, walkableArea, cursorArea);
            
            renderSizeInTiles = new Size(cameraSize.Width / tileSize + 2, cameraSize.Height / tileSize + 2);
            ClientSize = cameraSize;

            Text = "New Game";
        }

        private void SetUpPlayer(Vector position)
        {
            bmpPlayer = new Bitmap("Images/boroda.png");
            var playerLegs = new Sprite
            (
                position,
                bmpPlayer,
                4,
                7,
                new Size(64, 64),
                4);
            
            var playerTorso = new Sprite
            (
                position,
                bmpPlayer,
                0,
                3,
                new Size(64, 64),
                4);
            
            player = new Player
            {
                Shape = new RigidCircle(position, tileSize / 2, false),
                Torso = playerTorso,
                Legs = playerLegs
            };
        }
        
        private void TimerTick(object sender, EventArgs e)
        {
            UpdateState();
            clock.Start();
            
            RenderView();
            Invalidate();
            
            clock.Stop();
            Console.WriteLine(clock.ElapsedMilliseconds);
            clock.Reset();
        }

        private void UpdateState()
        {
            const int step = 6;
            UpdatePlayer(step);
            CorrectPlayer();
            camera.CorrectCamera(cursorPosition, player, currentLevel.levelSizeInTiles, tileSize);
        }

        private void RenderView()
        {
            RerenderCamera();
            RenderObjects();
            PrintDebugInfo();
        }

        private void UpdatePlayer(int step)
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
            player.Move(delta);
        }

        private void CorrectPlayer()
        {
            var delta = Vector.ZeroVector;
            var rightBorder = currentLevel.levelSizeInTiles.Width * tileSize;
            const int leftBorder = 0;
            var bottomBorder = currentLevel.levelSizeInTiles.Height * tileSize;
            const int topBorder = 0;

            var a = player.Center.Y - player.Radius - topBorder;
            var b = player.Center.Y + player.Radius - bottomBorder;
            var c = player.Center.X - player.Radius - leftBorder;
            var d = player.Center.X + player.Radius - rightBorder;

            if (a < 0)
                delta.Y -= a;
            if (b > 0)
                delta.Y -= b;
            if (c < 0)
                delta.X -= c;
            if (d > 0)
                delta.X -= d;
            
            player.Move(delta);
        }

        private void RerenderCamera()
        {
            var topLeftTileIndex = TileTools.GetTopLeftTileIndex(camera.position, tileSize);
            if (!topLeftTileIndex.Equals(previousTopLeftTileIndex))
            {
                RerenderTiles(topLeftTileIndex);
                previousTopLeftTileIndex = topLeftTileIndex;
            }
            
            CropRenderedTilesToCamera();
        }

        private void RerenderTiles(Vector topLeftTileIndex)
        {
            foreach (var layer in currentLevel.Layers)
                RenderLayer(layer, topLeftTileIndex);
        }

        private void RenderLayer(Layer layer, Vector topLeftTileIndex)
        {
            for (var x = 0; x <= renderSizeInTiles.Width; ++x)
            for (var y = 0; y <= renderSizeInTiles.Height; ++y)
            {
                var tileIndex = TileTools.GetTileIndex(x, y, topLeftTileIndex, currentLevel.levelSizeInTiles.Height);
                if (tileIndex > layer.Tiles.Length - 1) break;
                
                var tileID = layer.Tiles[tileIndex];
                if (tileID == 0 ) continue;
                
                var tileSetName = levelManager.GetTileSetName(tileID, currentLevel);
                RenderTile(x, y, tileID - 1, levelManager.GetTileMap(tileSetName));
            }
        }
        
        private void RenderTile(int targetX, int targetY, int tileID, Bitmap sourceImage)
        {
            var src = TileTools.GetSourceRectangle(tileID, sourceImage.Width / tileSize, tileSize);
            gfxRenderedTiles.DrawImage(sourceImage, targetX * tileSize, targetY * tileSize, src, GraphicsUnit.Pixel);
        }

        private void CropRenderedTilesToCamera()
        {
            var sourceRectangle = new Rectangle((int) camera.position.X % tileSize, (int) camera.position.Y % tileSize,
                camera.size.Width, camera.size.Height);
            
            gfxCamera.DrawImage(bmpRenderedTiles, 0, 0, sourceRectangle, GraphicsUnit.Pixel);
        }
        
        private void RenderObjects()
        {
            player.Legs.DrawNextFrame(gfxCamera, camera.position);
            player.Torso.DrawNextFrame(gfxCamera, camera.position);
        }
        
        private void PrintDebugInfo()
        {
            Print(0, 0, "Camera Size: " + camera.size.Width + " x "+ camera.size.Height, debugBrush);
            Print(0, debugFont.Height, "Scene Size (in Tiles): " + currentLevel.levelSizeInTiles.Width + " x "+ currentLevel.levelSizeInTiles.Height, debugBrush);
            Print(0, 2 * debugFont.Height, "(WAxis) Scroll Position: " + camera.position, debugBrush);
            Print(0, 3 * debugFont.Height, "(WAxis) Player Position: " + player.Center, debugBrush);
            Print(0, 4 * debugFont.Height, "(CAxis) Player Position: " + player.Center.ConvertFromWorldToCamera(camera.position), debugBrush);
            Print(0, 5 * debugFont.Height, "(CAxis) Cursor Position: " + cursorPosition, debugBrush);
            RigidBodyRenderer.Draw(player.Shape, camera.position, new Pen(Color.Gainsboro, 4), gfxCamera);
            gfxCamera.DrawRectangle(new Pen(Color.White), camera.walkableArea);
            gfxCamera.DrawRectangle(new Pen(Color.White), camera.cursorArea);
        }

        private void Print(float x, float y, string text, Brush color)
        {
            gfxCamera.DrawString(text, debugFont, color, x, y);
        }
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
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
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
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
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            cursorPosition = new Vector(e.X, e.Y);
        }
    }
}