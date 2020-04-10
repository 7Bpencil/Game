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
        private static Size renderSizeInTiles;
        private Vector cameraPosition;
        
        private Bitmap bmpTiles;
        private Bitmap bmpRenderBuffer;
        private Graphics gfxRenderBuffer;
        private Rectangle srcRect;
        
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
            renderSizeInTiles = new Size(cameraSizeInTiles.Width + 1, cameraSizeInTiles.Height + 1);
            ClientSize = new Size(cameraSizeInTiles.Width * tileSize, cameraSizeInTiles.Height * tileSize);
            Text = "New Game";
            
            currentLevel = LevelParser.ParseLevel("Levels/secondTry.tmx");
            bmpTiles = new Bitmap("Images/sprite_map.png");
            sceneSizeInTiles = new Size(currentLevel.Layers[0].Width, currentLevel.Layers[0].Height);
            
            keyState = new KeyStates();
            cameraPosition = Vector.ZeroVector;
            
            bmpRenderBuffer = new Bitmap(renderSizeInTiles.Width * tileSize, renderSizeInTiles.Height * tileSize);
            gfxRenderBuffer = Graphics.FromImage(bmpRenderBuffer);
            
            
            // that sections looks suspicious
            pbSurface = new PictureBox();
            pbSurface.Parent = this;
            pbSurface.BackColor = Color.Black;
            pbSurface.Dock = DockStyle.Fill;
            pbSurface.Image = bmpRenderBuffer;

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
                cameraPosition += delta;
                RemoveEscapingFromScene(cameraPosition);
                UpdateScrollBuffer();
            }
            
        }

        private void RemoveEscapingFromScene(Vector position)
        {
            var rightBorder = (sceneSizeInTiles.Width - cameraSizeInTiles.Width) * tileSize;
            const int leftBorder = 0;
            var bottomBorder = (sceneSizeInTiles.Height - cameraSizeInTiles.Height) * tileSize;
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
                for (var x = 0; x <= renderSizeInTiles.Width; ++x)
                for (var y = 0; y <= renderSizeInTiles.Height; ++y)
                {
                    var sx = GetLeftTileIndex() + x;
                    var sy = GetTopTileIndex() + y;
                    var tileIndex = sy * sceneSizeInTiles.Height + sx;
                    if (tileIndex > layer.Tiles.Length - 1) break;
                    
                    if (layer.Tiles[tileIndex] != 0) 
                        DrawTile(x, y, layer.Tiles[tileIndex] - 1);
                }
            }
            PrintDebugInfo();
            srcRect = new Rectangle((int) cameraPosition.X % tileSize, (int) cameraPosition.Y % tileSize,
                cameraSizeInTiles.Width * tileSize, cameraSizeInTiles.Height * tileSize);
            gfxRenderBuffer.DrawImage(bmpRenderBuffer, 0,0,srcRect, GraphicsUnit.Pixel);
            pbSurface.Image = bmpRenderBuffer;
        }


        private int GetLeftTileIndex()
        {
            var rem = cameraPosition.X % tileSize;
            if (rem == 0)
                return (int) cameraPosition.X / tileSize;
            return (int) (cameraPosition.X - rem) / tileSize;
        }
        
        private int GetTopTileIndex()
        {
            var rem = cameraPosition.Y % tileSize;
            if (rem == 0)
                return (int) cameraPosition.Y / tileSize;
            return (int) (cameraPosition.Y - rem) / tileSize;
        }

        private void PrintDebugInfo()
        {
            Print(0, 0, "Scroll Position: " + cameraPosition, debugBrush);
            Print(0, debugFont.Height, "Camera Size: " + cameraSizeInTiles.Width + " x "+ cameraSizeInTiles.Height, debugBrush);
            Print(0, 2 * debugFont.Height, "Scene Size: " + sceneSizeInTiles.Width + " x "+ sceneSizeInTiles.Height, debugBrush);
        }

        private void Print(float x, float y, string text, Brush color)
        {
            gfxRenderBuffer.DrawString(text, debugFont, color, x, y);
        }

        private void DrawTile(int x, int y, int tile)
        {
            var columnsAmountInPalette = bmpTiles.Width / tileSize;
            var sx = tile % columnsAmountInPalette * tileSize;
            var sy = tile / columnsAmountInPalette * tileSize;

            var src = new Rectangle(sx, sy, tileSize - 1, tileSize - 1);
            gfxRenderBuffer.DrawImage(bmpTiles, x * tileSize, y * tileSize, src, GraphicsUnit.Pixel);
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