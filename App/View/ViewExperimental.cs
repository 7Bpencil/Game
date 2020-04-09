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
        
        private Bitmap bmpTiles;
        private Bitmap bmpScrollBuffer;
        private Graphics gfxScrollBuffer;
        
        private PictureBox pbSurface;

        private Font debugFont;
        private Brush debugBrush;
        
        
        public class KeyStates
        {
            public bool Up, Down, Left, Right;
        }
        
        public ViewExperimental()
        {
            cameraSizeInTiles = new Size(16, 9);
            ClientSize = new Size(cameraSizeInTiles.Width * tileSize, cameraSizeInTiles.Height * tileSize);
            Text = "New Game";
            
            currentLevel = LevelParser.ParseLevel("Levels/secondTry.tmx");
            bmpTiles = new Bitmap("Images/sprite_map.png");
            sceneSizeInTiles = new Size(currentLevel.Layers[0].Width, currentLevel.Layers[0].Height);
            
            keyState = new KeyStates();
            scrollPosition = Vector.ZeroVector;
            
            bmpScrollBuffer = new Bitmap(cameraSizeInTiles.Width * tileSize, cameraSizeInTiles.Height * tileSize);
            gfxScrollBuffer = Graphics.FromImage(bmpScrollBuffer);
            
            // that sections looks suspicious
            pbSurface = new PictureBox();
            pbSurface.Parent = this;
            pbSurface.BackColor = Color.Black;
            pbSurface.Dock = DockStyle.Fill;
            pbSurface.Image = bmpScrollBuffer;

            debugFont = new Font("Arial", 18, FontStyle.Regular, GraphicsUnit.Pixel);
            debugBrush = new SolidBrush(Color.White);
            
            UpdateScrollBuffer();
            
            var timer = new Timer();
            timer.Interval = 15;
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var delta = Vector.ZeroVector;
            const int step = 1;
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
                UpdateScrollBuffer();
            }
            
        }

        private void RemoveEscapingFromScene(Vector position)
        {
            var rightBorder = sceneSizeInTiles.Width - cameraSizeInTiles.Width;
            const int leftBorder = 0;
            var bottomBorder = sceneSizeInTiles.Height - cameraSizeInTiles.Height;
            const int topBorder = 0;
            
            if (position.Y < topBorder) position.Y = topBorder;
            if (position.Y > bottomBorder) position.Y = bottomBorder;
            if (position.X < leftBorder) position.X = leftBorder;
            if (position.X > rightBorder) position.X = rightBorder;
        }

        private void UpdateScrollBuffer()
        {
            foreach (var layer in currentLevel.Layers)
            {
                for (var x = 0; x <= cameraSizeInTiles.Width; ++x)
                for (var y = 0; y <= cameraSizeInTiles.Height; ++y)
                {
                    var sx = (int) scrollPosition.X + x;
                    var sy = (int) scrollPosition.Y + y;
                    var tileIndex = sy * sceneSizeInTiles.Height + sx;
                    if (tileIndex > layer.Tiles.Length - 1) break;
                    
                    if (layer.Tiles[tileIndex] != 0) 
                        DrawTile(x, y, layer.Tiles[tileIndex] - 1);
                }
            }
            PrintDebugInfo();
            pbSurface.Image = bmpScrollBuffer;
        }

        private void PrintDebugInfo()
        {
            Print(0, 0, "Scroll Position: " + scrollPosition, debugBrush);
            Print(0, debugFont.Height, "Camera Size: " + cameraSizeInTiles.Width + " x "+ cameraSizeInTiles.Height, debugBrush);
            Print(0, 2 * debugFont.Height, "Scene Size: " + sceneSizeInTiles.Width + " x "+ sceneSizeInTiles.Height, debugBrush);
        }

        private void Print(float x, float y, string text, Brush color)
        {
            gfxScrollBuffer.DrawString(text, debugFont, color, x, y);
        }

        private void DrawTile(int x, int y, int tile)
        {
            var columnsAmountInPalette = bmpTiles.Width / tileSize;
            var sx = tile % columnsAmountInPalette * tileSize;
            var sy = tile / columnsAmountInPalette * tileSize;

            var src = new Rectangle(sx, sy, tileSize - 1, tileSize - 1);
            gfxScrollBuffer.DrawImage(bmpTiles, x * tileSize, y * tileSize, src, GraphicsUnit.Pixel);
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