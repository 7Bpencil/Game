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
        private static Size cameraSize;
        private static Size levelSizeInTiles;
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
        
        private Rectangle sourceRectangle;
        private Rectangle walkableArea;
        private Rectangle cursorArea;
        
        private Font debugFont;
        private Brush debugBrush;

        private Stopwatch clock;

        private LevelManager levelManager;

        private class KeyStates
        {
            public bool Up, Down, Left, Right, W, S, A, D;
        }
        
        public ViewExperimental()
        {
            
            levelManager = new LevelManager();
            currentLevel = levelManager.CurrentLevel;
            
            DoubleBuffered = true;
            SetUpView();
            currentLevel = LevelParser.ParseLevel("Levels/secondTry.tmx");
            levelSizeInTiles = new Size(currentLevel.Layers[0].WidthInTiles, currentLevel.Layers[0].HeightInTiles);

            keyState = new KeyStates();
            
            var playerStartPosition = new Vector(14 * 64, 6 * 64);
            SetUpPlayer(playerStartPosition);
            cursorPosition = playerStartPosition;
            
            //create and initialize renderer
            bmpRenderedTiles = new Bitmap(renderSizeInTiles.Width * tileSize, renderSizeInTiles.Height * tileSize);
            gfxRenderedTiles = Graphics.FromImage(bmpRenderedTiles);
            
            var context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(cameraSize.Width + 1, cameraSize.Height + 1);
            using (var g = CreateGraphics())
                cameraBuffer = context.Allocate(g, new Rectangle(0, 0, cameraSize.Width, cameraSize.Height));
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
            cameraSize = new Size(1280, 720);
            var p = cameraSize.Height / 3;
            walkableArea = new Rectangle(p, p, cameraSize.Width - 2 * p, cameraSize.Height - 2 * p);
            var q = cameraSize.Height / 5;
            cursorArea = new Rectangle(q, q, cameraSize.Width - 2 * q, cameraSize.Height - 2 * q);

            renderSizeInTiles = new Size(cameraSize.Width / tileSize + 2, cameraSize.Height / tileSize + 2);
            ClientSize = cameraSize;
            
            cameraPosition = new Vector(250, 100);
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
            UpdateCamera(step);
            UpdatePlayer(step);
            CorrectPlayer();
            CorrectCameraDependsOnCursorPosition();
            CorrectCameraDependsOnPlayerPosition();
            RemoveEscapingFromScene();
        }

        private void RenderView()
        {
            RerenderCamera();
            RenderObjects();
            PrintDebugInfo();
        }
        
        private void UpdateCamera(int step)
        {
            var deltaCamera = Vector.ZeroVector;
            if (keyState.Up) 
                deltaCamera.Y -= step;
            if (keyState.Down)
                deltaCamera.Y += step;
            if (keyState.Left)
                deltaCamera.X -= step;
            if (keyState.Right)
                deltaCamera.X += step;
            cameraPosition += deltaCamera;
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
            var rightBorder = levelSizeInTiles.Width * tileSize;
            const int leftBorder = 0;
            var bottomBorder = levelSizeInTiles.Height * tileSize;
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

        private void RemoveEscapingFromScene()
        {
            var rightBorder = levelSizeInTiles.Width * tileSize - cameraSize.Width;
            const int leftBorder = 0;
            var bottomBorder = levelSizeInTiles.Height * tileSize - cameraSize.Height;
            const int topBorder = 0;
            
            if (cameraPosition.Y < topBorder) cameraPosition.Y = topBorder;
            if (cameraPosition.Y > bottomBorder) cameraPosition.Y = bottomBorder;
            if (cameraPosition.X < leftBorder) cameraPosition.X = leftBorder;
            if (cameraPosition.X > rightBorder) cameraPosition.X = rightBorder;
        }
        
        private void RerenderCamera()
        {
            var topLeftTileIndex = TileTools.GetTopLeftTileIndex(cameraPosition, tileSize);
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
                var tileIndex = TileTools.GetTileIndex(x, y, topLeftTileIndex, levelSizeInTiles.Height);
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
            sourceRectangle = new Rectangle((int) cameraPosition.X % tileSize, (int) cameraPosition.Y % tileSize,
                cameraSize.Width, cameraSize.Height);
            
            gfxCamera.DrawImage(bmpRenderedTiles, 0, 0, sourceRectangle, GraphicsUnit.Pixel);
        }
        
        private void RenderObjects()
        {
            player.Legs.DrawNextFrame(gfxCamera, cameraPosition);
            player.Torso.DrawNextFrame(gfxCamera, cameraPosition);
        }
        
        private void PrintDebugInfo()
        {
            Print(0, 0, "Camera Size: " + cameraSize.Width + " x "+ cameraSize.Height, debugBrush);
            Print(0, debugFont.Height, "Scene Size (in Tiles): " + levelSizeInTiles.Width + " x "+ levelSizeInTiles.Height, debugBrush);
            Print(0, 2 * debugFont.Height, "(WAxis) Scroll Position: " + cameraPosition, debugBrush);
            Print(0, 3 * debugFont.Height, "(WAxis) Player Position: " + player.Center, debugBrush);
            Print(0, 4 * debugFont.Height, "(CAxis) Player Position: " + player.Center.ConvertFromWorldToCamera(cameraPosition), debugBrush);
            Print(0, 5 * debugFont.Height, "(CAxis) Cursor Position: " + cursorPosition, debugBrush);
            RigidBodyRenderer.Draw(player.Shape, cameraPosition, new Pen(Color.Gainsboro, 4), gfxCamera);
            gfxCamera.DrawRectangle(new Pen(Color.White), walkableArea);
            gfxCamera.DrawRectangle(new Pen(Color.White), cursorArea);
        }

        private void Print(float x, float y, string text, Brush color)
        {
            gfxCamera.DrawString(text, debugFont, color, x, y);
        }
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    keyState.Up = true;
                    break;
                case Keys.W:
                    keyState.W = true;
                    break;

                case Keys.Down:
                    keyState.Down = true;
                    break;
                case Keys.S:
                    keyState.S = true;
                    break;

                case Keys.Left:
                    keyState.Left = true;
                    break;
                case Keys.A:
                    keyState.A = true;
                    break;

                case Keys.Right:
                    keyState.Right = true;
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
                
                case Keys.Up:
                    keyState.Up = false;
                    break;
                case Keys.W:
                    keyState.W = false;
                    break;

                case Keys.Down:
                    keyState.Down = false;
                    break;
                case Keys.S:
                    keyState.S = false;
                    break;

                case Keys.Left:
                    keyState.Left = false;
                    break;
                case Keys.A:
                    keyState.A = false;
                    break;

                case Keys.Right:
                    keyState.Right = false;
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