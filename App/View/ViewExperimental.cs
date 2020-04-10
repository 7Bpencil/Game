using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.RigidBody;
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
        private static Size sceneSizeInTiles;
        private static Size renderSizeInTiles;
        private Vector cameraPosition;
        
        private RigidCircle player;
        private RigidCircle cursor;
        
        private Sprite playerLegs;
        private Sprite playerTorso;

        private Bitmap bmpTiles;
        private Bitmap bmpRenderBuffer;
        private Bitmap bmpPlayer;
        private Graphics gfxRenderBuffer;
        private Rectangle srcRect;

        private Rectangle walkableArea;
        private Rectangle cursorArea;
        
        private PictureBox pbSurface;

        private Font debugFont;
        private Brush debugBrush;


        private class KeyStates
        {
            public bool Up, Down, Left, Right, W, S, A, D;
        }
        
        public ViewExperimental()
        {
            cameraSize = new Size(1280, 720);
            var p = cameraSize.Height / 3;
            walkableArea = new Rectangle(p, p, cameraSize.Width - 2 * p, cameraSize.Height - 2 * p);
            var q = cameraSize.Height / 5;
            cursorArea = new Rectangle(q, q, cameraSize.Width - 2 * q, cameraSize.Height - 2 * q);

            renderSizeInTiles = new Size(cameraSize.Width / tileSize + 2, cameraSize.Height / tileSize + 2);
            ClientSize = cameraSize;
            Text = "New Game";
            
            currentLevel = LevelParser.ParseLevel("Levels/secondTry.tmx");
            bmpTiles = new Bitmap("Images/sprite_map.png");
            sceneSizeInTiles = new Size(currentLevel.Layers[0].Width, currentLevel.Layers[0].Height);
            
            keyState = new KeyStates();
            cameraPosition = new Vector(500, 200);
            player = new RigidCircle(new Vector(14 * 64, 6 * 64), 32, false);
            cursor = new RigidCircle(new Vector(14 * 64, 6 * 64), 5, false);
            
            //create and inintialize player legs
            bmpPlayer = new Bitmap("Images/boroda.png");
            playerLegs = new Sprite();
            playerLegs.TopLeft = new Vector(0, 0);
            playerLegs.Alive = true;
            playerLegs.Columns = 4;
            playerLegs.Size = new Size(64, 64);
            playerLegs.Image = bmpPlayer;
            
            
            //create and inintialize player body
            playerTorso = new Sprite();
            playerTorso.TopLeft = new Vector(0, 0);
            playerTorso.Alive = true;
            playerTorso.Columns = 4;
            playerTorso.Size = new Size(64, 64);
            playerTorso.Image = bmpPlayer;

            //create and initialize renderer
            bmpRenderBuffer = new Bitmap(renderSizeInTiles.Width * tileSize, renderSizeInTiles.Height * tileSize);
            gfxRenderBuffer = Graphics.FromImage(bmpRenderBuffer);
            
            
            // that sections looks suspicious
            pbSurface = new PictureBox();
            pbSurface.Parent = this;
            pbSurface.BackColor = Color.Black;
            pbSurface.Dock = DockStyle.Fill;
            pbSurface.Image = bmpRenderBuffer;
            pbSurface.MouseMove += Mouse;

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
            const int step = 6;
            UpdateCamera(step);
            UpdatePlayer(step);
            CorrectPlayer();
            CorrectCameraDependsOnCursorPosition();
            CorrectCameraDependsOnPlayerPosition(player.Center.ConvertFromWorldToCamera(cameraPosition));
            RemoveEscapingFromScene(cameraPosition);
            UpdateScrollBuffer();
            /*
            if (!deltaCamera.Equals(Vector.ZeroVector))
            {
                cameraPosition += deltaCamera;
                RemoveEscapingFromScene(cameraPosition);
                UpdateScrollBuffer();
            }
            */
            
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
            if (keyState.W) 
                player.Center.Y -= step;
            if (keyState.S)
                player.Center.Y += step;
            if (keyState.A)
                player.Center.X -= step;
            if (keyState.D)
                player.Center.X += step;
        }

        private void CorrectPlayer()
        {
            var rightBorder = sceneSizeInTiles.Width * tileSize;
            const int leftBorder = 0;
            var bottomBorder = sceneSizeInTiles.Height * tileSize;
            const int topBorder = 0;
            
            if (player.Center.Y - player.Radius < topBorder) player.Center.Y = topBorder + player.Radius;
            if (player.Center.Y + player.Radius > bottomBorder) player.Center.Y = bottomBorder - player.Radius;
            if (player.Center.X - player.Radius < leftBorder) player.Center.X = leftBorder + player.Radius;
            if (player.Center.X + player.Radius > rightBorder) player.Center.X = rightBorder - player.Radius;
        }

        private void RemoveEscapingFromScene(Vector position)
        {
            var rightBorder = sceneSizeInTiles.Width * tileSize - cameraSize.Width;
            const int leftBorder = 0;
            var bottomBorder = sceneSizeInTiles.Height * tileSize - cameraSize.Height;
            const int topBorder = 0;
            
            if (position.Y < topBorder) position.Y = topBorder;
            if (position.Y > bottomBorder) position.Y = bottomBorder;
            if (position.X < leftBorder) position.X = leftBorder;
            if (position.X > rightBorder) position.X = rightBorder;
        }

        private void CorrectCameraDependsOnPlayerPosition(Vector playerCenterInCamera)
        {
            var q = playerCenterInCamera.X - player.Radius - walkableArea.X;
            var b = walkableArea.X + walkableArea.Width - (playerCenterInCamera.X + player.Radius);
            var p = playerCenterInCamera.Y - player.Radius - walkableArea.Y;
            var a = walkableArea.Y + walkableArea.Height - (playerCenterInCamera.Y + player.Radius);
            if (q < 0) cameraPosition.X += q;
            if (b < 0) cameraPosition.X -= b;
            if (p < 0) cameraPosition.Y += p;
            if (a < 0) cameraPosition.Y -= a;
        }

        private void CorrectCameraDependsOnCursorPosition()
        {
            var q = cursor.Center.X - cursorArea.X;
            var b = cursorArea.X + cursorArea.Width - cursor.Center.X;
            var p = cursor.Center.Y - cursorArea.Y;
            var a = cursorArea.Y + cursorArea.Height - cursor.Center.Y;
            if (q < 0) cameraPosition.X += q;
            if (b < 0) cameraPosition.X -= b;
            if (p < 0) cameraPosition.Y += p;
            if (a < 0) cameraPosition.Y -= a;
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
            
            srcRect = new Rectangle((int) cameraPosition.X % tileSize, (int) cameraPosition.Y % tileSize,
                cameraSize.Width, cameraSize.Height);
            gfxRenderBuffer.DrawImage(bmpRenderBuffer, 0, 0, srcRect, GraphicsUnit.Pixel);
            gfxRenderBuffer.DrawRectangle(new Pen(Color.White), walkableArea);
            gfxRenderBuffer.DrawRectangle(new Pen(Color.White), cursorArea);
            RigidBodyRenderer.Draw(player, cameraPosition, new Pen(Color.Gainsboro, 4), gfxRenderBuffer);
            RigidBodyRenderer.Draw(cursor, new Pen(Color.Gainsboro, 4), gfxRenderBuffer);
            playerLegs.Animate(gfxRenderBuffer, 0, 3);
            playerTorso.Animate(gfxRenderBuffer, 4, 7);    
            pbSurface.Image = bmpRenderBuffer;
            PrintDebugInfo();
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
            Print(0, debugFont.Height, "Camera Size: " + cameraSize.Width + " x "+ cameraSize.Height, debugBrush);
            Print(0, 2 * debugFont.Height, "Scene Size: " + sceneSizeInTiles.Width + " x "+ sceneSizeInTiles.Height, debugBrush);
            Print(0, 3 * debugFont.Height, "Player Position: " + player.Center, debugBrush);
            Print(0, 4 * debugFont.Height, "Cursor Position: " + cursor.Center, debugBrush);
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

        private void Mouse(object o, MouseEventArgs e)
        {
            cursor.Center = new Vector(e.X, e.Y);
        }
    }
}