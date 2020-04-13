using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using App.Engine;
using App.Engine.PhysicsEngine;

namespace App.View
{
    public class ViewExperimental : Form
    {
        private const int tileSize = 64;
        
        private Bitmap bmpRenderedTiles;
        private Graphics gfxRenderedTiles;
        
        private BufferedGraphics cameraBuffer;
        private Graphics gfxCamera;
        
        private Font debugFont;
        private Brush debugBrush;
        
        private Core engineCore;

        private Size cameraSize;

        public ViewExperimental()
        {
            engineCore = new Core(this);
            cameraSize = engineCore.CameraSize;
            
            SetUpView();
            SetUpRenderer();
            
            debugFont = new Font("Arial", 18, FontStyle.Regular, GraphicsUnit.Pixel);
            debugBrush = new SolidBrush(Color.White);
            
            var timer = new Timer();
            timer.Interval = 15;
            timer.Tick += engineCore.GameLoop;
            timer.Start();
        }
        
        private void SetUpRenderer()
        {
            DoubleBuffered = true;
            var renderSizeInTiles = new Size(cameraSize.Width / tileSize + 2, cameraSize.Height / tileSize + 2);
            
            bmpRenderedTiles = new Bitmap(renderSizeInTiles.Width * tileSize, renderSizeInTiles.Height * tileSize);
            gfxRenderedTiles = Graphics.FromImage(bmpRenderedTiles);
            
            var context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(cameraSize.Width + 1, cameraSize.Height + 1);
            using (var g = CreateGraphics())
                cameraBuffer = context.Allocate(g, new Rectangle(0, 0, cameraSize.Width, cameraSize.Height));
            
            gfxCamera = cameraBuffer.Graphics;
            gfxCamera.InterpolationMode = InterpolationMode.Bilinear;
        }
        
        private void SetUpView()
        {
            ClientSize = cameraSize;
            Text = "New Game";
        }

        public void RenderTile(Bitmap sourceImage, int x, int y, Rectangle src)
        {
            gfxRenderedTiles.DrawImage(sourceImage, x, y, src, GraphicsUnit.Pixel);
        }

        public void RenderCamera(Rectangle sourceRectangle)
        {
            gfxCamera.DrawImage(bmpRenderedTiles, 0, 0, sourceRectangle, GraphicsUnit.Pixel);
        }

        public void RenderSprite(Sprite obj, Vector cameraPosition)
        {
            obj.DrawNextFrame(gfxCamera, cameraPosition);
        }
        
        public void PrintMessages(string[] messages)
        {
            for (var i = 0; i < messages.Length; i++)
                gfxCamera.DrawString(messages[i], debugFont, debugBrush, 0, i * debugFont.Height);
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