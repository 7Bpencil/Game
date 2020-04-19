using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using App.Engine;
using App.Engine.PhysicsEngine;
using App.Engine.Render;

namespace App.View
{
    public class ViewForm : Form
    {
        private Bitmap bmpRenderedTiles;
        private Graphics gfxRenderedTiles;
        private BufferedGraphics cameraBuffer;
        private Graphics gfxCamera;

        private Core engineCore;
        private readonly Point screenCenter;        

        public ViewForm()
        {
            var screenSize = new Size(
                SystemInformation.PrimaryMonitorSize.Width,
                SystemInformation.PrimaryMonitorSize.Height);
            screenCenter = new Point(screenSize.Width / 2, screenSize.Height / 2);

            var renderMachine = new RenderMachine(this, gfxRenderedTiles, bmpRenderedTiles, gfxCamera);
            var renderPipeline = new RenderPipeline(renderMachine);
            engineCore = new Core(this, screenSize, renderPipeline);
            
            SetUpView(screenSize);
            var renderSize = engineCore.GetRenderSize();
            SetUpRenderer(renderSize, screenSize);
            
            var timer = new Timer();
            timer.Interval = 15;
            timer.Tick += engineCore.GameLoop;
            timer.Start();
        }
        
        private void SetUpView(Size viewSize)
        {
            ClientSize = viewSize;
            FormBorderStyle = FormBorderStyle.None;

            Cursor.Hide();
            CursorReset();
        }
        
        private void SetUpRenderer(Size renderSize, Size cameraSize)
        {
            DoubleBuffered = true;

            bmpRenderedTiles = new Bitmap(renderSize.Width, renderSize.Height);
            gfxRenderedTiles = Graphics.FromImage(bmpRenderedTiles);
         
            SetCameraBuffer(cameraSize);
            gfxCamera = cameraBuffer.Graphics;
            gfxCamera.InterpolationMode = InterpolationMode.Bilinear;
        }

        private void SetCameraBuffer(Size cameraSize)
        {
            var context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(cameraSize.Width + 1, cameraSize.Height + 1);
            using (var g = CreateGraphics())
                cameraBuffer = context.Allocate(g, new Rectangle(0, 0, cameraSize.Width, cameraSize.Height));
        }

        public Vector GetCursorDiff()
        {
            return new Vector(
                Cursor.Position.X - screenCenter.X,
                Cursor.Position.Y - screenCenter.Y);
        }

        public void CursorReset()
        {
            Cursor.Position = screenCenter;
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
    }
}