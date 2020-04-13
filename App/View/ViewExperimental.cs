using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using App.Engine;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.RigidBody;
using App.Model;
using App.Model.LevelData;
using App.View.Renderings;

namespace App.View
{
    public class ViewExperimental : ContractView
    {
        
        
        
        private const int tileSize = 64;
        
        private static Size renderSizeInTiles;
        
        private Vector previousTopLeftTileIndex;
        
        
        
        
        
        
        private Bitmap bmpRenderedTiles;
        private Graphics gfxRenderedTiles;
        
        private BufferedGraphics cameraBuffer;
        private Graphics gfxCamera;
        
        private Font debugFont;
        private Brush debugBrush;

        

        

        private ContractCore engineCore;

        private class KeyStates
        {
            public bool W, S, A, D;
        }
        
        public ViewExperimental()
        {
            
            SetUpRenderer();
            SetUpView();
            
            engineCore = new Core(this);
           
            
            
            
            
            debugFont = new Font("Arial", 18, FontStyle.Regular, GraphicsUnit.Pixel);
            debugBrush = new SolidBrush(Color.White);
            
            RerenderCamera();
            
            
            var timer = new Timer();
            timer.Interval = 15;
            timer.Tick += engineCore.GameLoop;
            timer.Start();
        }
        

        
        private void SetUpRenderer()
        {
            DoubleBuffered = true;
            renderSizeInTiles = new Size(camera.size.Width / tileSize + 2, camera.size.Height / tileSize + 2);
            
            bmpRenderedTiles = new Bitmap(renderSizeInTiles.Width * tileSize, renderSizeInTiles.Height * tileSize);
            gfxRenderedTiles = Graphics.FromImage(bmpRenderedTiles);
            
            var context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(camera.size.Width + 1, camera.size.Height + 1);
            using (var g = CreateGraphics())
                cameraBuffer = context.Allocate(g, new Rectangle(0, 0, camera.size.Width, camera.size.Height));
            gfxCamera = cameraBuffer.Graphics;
            gfxCamera.InterpolationMode = InterpolationMode.Bilinear;
            
            previousTopLeftTileIndex = new Vector(0, 0);
        }
        
        private void SetUpView()
        {
            ClientSize = camera.size;
            Text = "New Game";
        }

        public override void Render()
        {
            RerenderCamera();
            RenderObjects();
            PrintDebugInfo();
            
            Invalidate();
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
            engineCore.OnKeyDown(e.KeyCode);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            engineCore.OnKeyUp(e.KeyCode);
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            cameraBuffer.Render(e.Graphics);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            engineCore.OnMouseMove(new Vector(e.X, e.Y));
        }
    }
}