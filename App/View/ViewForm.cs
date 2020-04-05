using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using App.Engine;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.RigidBody;
using App.View.Renderings;

namespace App.View
{
    public class ViewForm : ContractView
    {
        private ContractCore engineCore;
        protected override ContractCore EngineCore { get; set; }
        private List<RigidShape> sceneObjects;
        private Pen strokePen;
        private Pen collisionStrokePen;
        private Pen collisionInfoStrokePen;

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
            
            strokePen = new Pen(Color.Crimson, 4);
            collisionStrokePen = new Pen(Color.Lime, 4);
            collisionInfoStrokePen = new Pen(Color.Lime, 4);
        }

        public override void Render()
        {
            Invalidate();
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics; // TODO: move it to somewhere else
            var collisions = engineCore.GetCollisions();
            
            foreach (var formObject in sceneObjects)
                if (formObject.IsCollided) RigidBodyRender.Draw(formObject, collisionStrokePen, g);
                else RigidBodyRender.Draw(formObject, strokePen, g);

            //foreach (var collision in collisions)
            //    CollisionInfoRender.Draw(collision, collisionInfoStrokePen, g);
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