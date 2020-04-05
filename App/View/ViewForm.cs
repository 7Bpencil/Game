using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using App.Engine;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.RigidBody;

namespace App.View
{
    public class ViewForm : ContractView
    {
        private ContractCore engineCore;
        protected override ContractCore EngineCore { get; set; }
        private List<RigidShape> sceneObjects;

        public ViewForm()
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

        public override void Render()
        {
            Invalidate();
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics; // TODO: move it to somewhere else
            var collisionStrokePen = new Pen(Color.Lime, 4);
            foreach (var formObject in sceneObjects)
                if (formObject.IsCollided) formObject.DrawCollision(g, collisionStrokePen);
                else formObject.Draw(g);
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