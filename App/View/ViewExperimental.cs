using System;
using System.Drawing;
using System.Windows.Forms;
using App.Engine.PhysicsEngine;
using App.Model.LevelData;
using App.Model.Parser;

namespace App.View
{
    public class ViewExperimental : Form
    {
        private Level currentLevel;
        private KeyStates keyState;
        private const int tileSize = 64;
        private static Size cameraSizeInTiles;
        private static Size sceneSizeInTiles;
        private Vector scrollPosition;
        
        private Bitmap bmpSurface;
        private Graphics gfxSurface;
        private Bitmap bmpScrollBuffer;
        private Graphics gfxScrollBuffer;
        private PictureBox pbSurface;
        private Bitmap bmpTiles;
        
        public class KeyStates
        {
            public bool Up, Down, Left, Right;
        }
        
        public ViewExperimental()
        {
            SetUpView();
            currentLevel = LevelParser.ParseLevel("Levels/secondTry.tmx");
            bmpTiles = new Bitmap("Images/sprite_map.png");
            sceneSizeInTiles = new Size(currentLevel.Layers[0].Width, currentLevel.Layers[0].Height);
            
            keyState = new KeyStates();
            scrollPosition = Vector.ZeroVector;
            
            bmpSurface = new Bitmap(sceneSizeInTiles.Width * tileSize, sceneSizeInTiles.Height * tileSize);
            gfxSurface = Graphics.FromImage(bmpSurface);
            bmpScrollBuffer = new Bitmap(cameraSizeInTiles.Width * tileSize, cameraSizeInTiles.Height * tileSize);
            gfxScrollBuffer = Graphics.FromImage(bmpScrollBuffer);
            
            pbSurface = new PictureBox();
            pbSurface.Parent = this;
            pbSurface.BackColor = Color.Black;
            pbSurface.Dock = DockStyle.Fill;
            pbSurface.Image = bmpSurface;
            
            var timer = new Timer();
            timer.Interval = 20;
            timer.Enabled = true;
            timer.Tick += timerTick;
        }
        
        private void SetUpView()
        {
            cameraSizeInTiles = new Size(16, 9);
            ClientSize = new Size(cameraSizeInTiles.Width * tileSize, cameraSizeInTiles.Height * tileSize);
            Text = "Level Viewer";
            
        }
        
        private void timerTick(object sender, EventArgs e)
        {
            var delta = Vector.ZeroVector;
            const int step = 4;
            if (keyState.Up) 
                delta.Y -= step;
            if (keyState.Down)
                delta.Y += step;
            if (keyState.Left)
                delta.X -= step;
            if (keyState.Right)
                delta.X += step;
            
            if (!delta.Equals(Vector.ZeroVector))
            {
                scrollPosition += delta;
                RemoveEscapingFromScene(scrollPosition);
                DrawScrollBuffer();
            }
            
        }

        private void RemoveEscapingFromScene(Vector position)
        {
            var rightBorder = (sceneSizeInTiles.Width - 1) * tileSize;
            const int leftBorder = 0;
            var bottomBorder = (sceneSizeInTiles.Height - 1) * tileSize;
            const int topBorder = 0;
            
            if (position.Y < topBorder) position.Y = topBorder;
            if (position.Y > bottomBorder) position.Y = bottomBorder;
            if (position.X < leftBorder) position.X = leftBorder;
            if (position.X > rightBorder) position.X = rightBorder;
        }
        
        public void DrawScrollBuffer()
        {
            UpdateScrollBuffer();
            var subTile = new Vector(scrollPosition.X % tileSize, scrollPosition.Y % tileSize);
            Rectangle source = new Rectangle(
                (int) subTile.X, (int) subTile.Y,
                bmpScrollBuffer.Width, bmpScrollBuffer.Height);
            
            gfxSurface.DrawImage(bmpScrollBuffer, 0, 0, source, GraphicsUnit.Pixel);
        }
        
        public void UpdateScrollBuffer()
        {
            foreach (var layer in currentLevel.Layers)
            {
                for (var x = 0; x <= cameraSizeInTiles.Width; ++x)
                for (var y = 0; y <= cameraSizeInTiles.Height; ++y)
                {
                    var sx = (int) (scrollPosition.X / tileSize) + x;
                    var sy = (int) (scrollPosition.Y / tileSize) + y;
                    var tilenum = layer.Tiles[sy * sceneSizeInTiles.Height + sx];
                    if (tilenum != 0) 
                        DrawTile(x, y, tilenum - 1);
                }
            }
        }
        
        public void DrawTile(int x, int y, int tile)
        {
            var columnsAmountInPalette = bmpTiles.Width / tileSize;
            var sx = tile % columnsAmountInPalette * tileSize;
            var sy = tile / columnsAmountInPalette * tileSize;
            
            var src = new Rectangle(sx, sy, tileSize, tileSize);

            gfxSurface.DrawImage(bmpTiles, x * tileSize, y * tileSize, src, GraphicsUnit.Pixel);
            pbSurface.Image = bmpSurface;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.W:
                    keyState.Up = true;
                    break;

                case Keys.Down:
                case Keys.S:
                    keyState.Down = true;
                    break;

                case Keys.Left:
                case Keys.A:
                    keyState.Left = true;
                    break;

                case Keys.Right:
                case Keys.D:
                    keyState.Right = true;
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
                case Keys.W:
                    keyState.Up = false;
                    break;

                case Keys.Down:
                case Keys.S:
                    keyState.Down = false;
                    break;

                case Keys.Left:
                case Keys.A:
                    keyState.Left = false;
                    break;

                case Keys.Right:
                case Keys.D:
                    keyState.Right = false;
                    break;
            }
        }
    }
}