using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using App.Engine;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.Collision;
using App.Engine.PhysicsEngine.RigidBody;
using App.View.Renderings;

namespace App.View
{
    public class ViewForm : Form
    {
        private readonly int tileSize;
        private Bitmap bmpRenderedTiles;
        private Graphics gfxRenderedTiles;
        private BufferedGraphics cameraBuffer;
        private Graphics gfxCamera;
        private Font debugFont;
        private Brush debugBrush;
        private Pen debugPen;
        private Pen collisionPen;
        private Core engineCore;
        private Size cameraSize;

        public ViewForm()
        {
            var screenSize = new Size(
                SystemInformation.PrimaryMonitorSize.Width,
                SystemInformation.PrimaryMonitorSize.Height);
            
            engineCore = new Core(this, screenSize);
            tileSize = engineCore.GetTileSize();
            cameraSize = engineCore.CameraSize;
            
            SetUpView(screenSize);
            SetUpRenderer();
            
            debugFont = new Font("Arial", 18, FontStyle.Regular, GraphicsUnit.Pixel);
            debugBrush = new SolidBrush(Color.White);
            debugPen = new Pen(Color.White, 4);
            collisionPen = new Pen(Color.Crimson, 4);
            
            var timer = new Timer();
            timer.Interval = 15;
            timer.Tick += engineCore.GameLoop;
            timer.Start();
        }
        
        private void SetUpView(Size viewSize)
        {
            ClientSize = viewSize;
            FormBorderStyle = FormBorderStyle.None;
            Text = "Cyber Renaissance";
        }
        
        private void SetUpRenderer()
        {
            DoubleBuffered = true;
            var renderSizeInTiles = engineCore.GetRenderSizeInTiles();
            
            bmpRenderedTiles = new Bitmap(renderSizeInTiles.Width * tileSize, renderSizeInTiles.Height * tileSize);
            gfxRenderedTiles = Graphics.FromImage(bmpRenderedTiles);
         
            SetCameraBuffer();
            gfxCamera = cameraBuffer.Graphics;
            gfxCamera.InterpolationMode = InterpolationMode.Bilinear;
        }

        private void SetCameraBuffer()
        {
            var context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(cameraSize.Width + 1, cameraSize.Height + 1);
            using (var g = CreateGraphics())
                cameraBuffer = context.Allocate(g, new Rectangle(0, 0, cameraSize.Width, cameraSize.Height));
        }

        public void RenderTile(Bitmap tileMap, int x, int y, Rectangle src)
        {
            gfxRenderedTiles.DrawImage(tileMap, x, y, src, GraphicsUnit.Pixel);
        }

        public void RenderCamera(Rectangle sourceRectangle)
        {
            gfxCamera.DrawImage(bmpRenderedTiles, 0, 0, sourceRectangle, GraphicsUnit.Pixel);
        }

        public void RenderSpriteOnCamera(Sprite sprite, Vector cameraPosition)
        {
            sprite.DrawNextFrame(gfxCamera, cameraPosition);
        }
        
        public void RenderSpriteOnCamera(Sprite sprite)
        {
            sprite.DrawNextFrame(gfxCamera);
        }

        public void RenderPolygonOnCamera(Polygon polygon, Vector cameraPosition)
        {
            PolygonRenderer.Draw(polygon, cameraPosition, debugPen, gfxCamera);
        }
        
        public void RenderPolygonOnWorld(Polygon polygon)
        {
            PolygonRenderer.Draw(polygon, debugPen, gfxRenderedTiles);
        }

        public void RenderShapeOnCamera(RigidShape shape, Vector cameraPosition)
        {
            RigidBodyRenderer.Draw(shape, cameraPosition, debugPen, gfxCamera);
        }
        
        public void RenderShapeOnWorld(RigidShape shape)
        {
            RigidBodyRenderer.Draw(shape, debugPen, gfxRenderedTiles);
        }

        public void RenderCollisionInfoOnCamera(CollisionInfo info, Vector cameraPosition)
        {
            CollisionInfoRenderer.Draw(info, cameraPosition, collisionPen, gfxCamera);
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

        protected override void OnMouseMove(MouseEventArgs e)
        {
            engineCore.OnMouseMove(new Vector(e.X, e.Y));
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            cameraBuffer.Render(e.Graphics);
        }
    }
}