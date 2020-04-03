using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using App.Physics_Engine;
using App.Physics_Engine.RigidBody;

namespace App
{
    public class PlaygroundPhysEngine : Form
    {
        private Core engineCore;
        private List<RigidShape> sceneObjects;

        public PlaygroundPhysEngine()
        {
            SetUpForm();
            engineCore = new Core(this);
            sceneObjects = engineCore.GetSceneObjects();

            var timer = new Timer();
            timer.Interval = 15; // around 66 fps
            timer.Tick += engineCore.GameLoop;
            timer.Start();
        }

        private void SetUpForm()
        {
            Cursor.Hide();
            DoubleBuffered = true;
            Size = new Size(854, 480);
            Text = "NEW GAME";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics; // TODO: move it to somewhere else
            var pen = new Pen(Color.Crimson);

            foreach (var formObject in sceneObjects)
                formObject.Draw(g, pen);
        }

        public void Render()
        {
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            engineCore.OnMouseMove(new Vector(e.X, e.Y));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            engineCore.OnKeyDown(e.KeyCode);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            engineCore.OnKeyUp(e.KeyCode);
        }
    }
}