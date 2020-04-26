using System.Drawing;
using System.Windows.Forms;
using App.Engine;
using App.Engine.Physics;
using App.Engine.Render;

namespace App.View
{
    public class ViewForm : Form
    {
        private BufferedGraphics cameraBuffer;
        private Core engineCore;
        private readonly Point screenCenter;        

        public ViewForm()
        {
            var screenSize = new Size(
                SystemInformation.PrimaryMonitorSize.Width,
                SystemInformation.PrimaryMonitorSize.Height);
            screenCenter = new Point(screenSize.Width / 2, screenSize.Height / 2);
            SetUpView(screenSize);

            var renderMachine = new RenderMachine(this, screenSize);
            var renderPipeline = new RenderPipeline(renderMachine);
            engineCore = new Core(this, screenSize, renderPipeline);
            cameraBuffer = renderMachine.GetCameraBuffer();
            
            var timer = new Timer();
            timer.Interval = 16;
            timer.Tick += engineCore.GameLoop;
            timer.Start();
        }
        
        private void SetUpView(Size viewSize)
        {
            DoubleBuffered = true;
            ClientSize = viewSize;
            FormBorderStyle = FormBorderStyle.None;

            Cursor.Hide();
            CursorReset();
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
        
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            engineCore.OnMouseWheel(e.Delta);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            engineCore.OnMouseDown(e.Button);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            engineCore.OnMouseUp(e.Button);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            cameraBuffer.Render(e.Graphics);
        }
    }
}