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
            var collisionInfoStrokePen = new Pen(Color.Lime, 4);
            var collisions = engineCore.GetCollisions();
            
            foreach (var formObject in sceneObjects)
                if (formObject.IsCollided)
                    DrawShapeCollision(g, formObject, collisionStrokePen);
                else DrawShape(g, formObject);

            //foreach (var collision in collisions)
            //    collision.Draw(g, collisionInfoStrokePen);
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

        private static void DrawShape(Graphics g, RigidShape shapeObject)
        {
            if (shapeObject is RigidRectangle)
                DrawRectangle(g, (RigidRectangle) shapeObject);
            else if (shapeObject is RigidCircle)
                DrawCircle(g, (RigidCircle) shapeObject);
        }
        
        private static void DrawCircle(Graphics g, RigidCircle shape)
        {
            var stateBefore = g.Save();
            if (!shape.Center.Equals(Vector.ZeroVector))
                g.TranslateTransform(shape.Center.X, shape.Center.Y);
            g.DrawEllipse(shape.StrokePen, -shape.Radius, -shape.Radius, shape.Diameter, shape.Diameter);
            g.Restore(stateBefore);
        }
        
        private static void DrawRectangle(Graphics g, RigidRectangle shape)
        {
            var stateBefore = g.Save();
            if (!shape.Center.Equals(Vector.ZeroVector))
                g.TranslateTransform(shape.Center.X, shape.Center.Y);
            if (shape.angle != 0)
                g.RotateTransform(-shape.angle);
            g.DrawRectangle(shape.StrokePen, -shape.Width / 2, -shape.Height / 2, shape.Width, shape.Height);
            g.Restore(stateBefore);
        }

        private static void DrawShapeCollision(Graphics g, RigidShape shapeCollision, Pen collisionStrokePen)
        {
            if (shapeCollision is RigidRectangle)
                DrawRectangleCollision(g, (RigidRectangle) shapeCollision, collisionStrokePen);
            else if (shapeCollision is RigidCircle)
                DrawCircleCollision(g, (RigidCircle) shapeCollision, collisionStrokePen);
        }
        
        private static void DrawCircleCollision(Graphics g, RigidCircle shape, Pen collisionStrokePen)
        {
            var stateBefore = g.Save();
            if (!shape.Center.Equals(Vector.ZeroVector))
                g.TranslateTransform(shape.Center.X, shape.Center.Y);
            g.DrawEllipse(collisionStrokePen, -shape.Radius, -shape.Radius, shape.Diameter, shape.Diameter);
            g.Restore(stateBefore);
        }
        
        private static void DrawRectangleCollision(Graphics g, RigidRectangle shape, Pen collisionStrokePen)
        {
            var stateBefore = g.Save();
            if (!shape.Center.Equals(Vector.ZeroVector))
                g.TranslateTransform(shape.Center.X, shape.Center.Y);
            if (shape.angle != 0)
                g.RotateTransform(-shape.angle);
            g.DrawRectangle(collisionStrokePen, -shape.Width / 2, -shape.Height / 2, shape.Width, shape.Height);
            g.Restore(stateBefore);
        }
    }
}