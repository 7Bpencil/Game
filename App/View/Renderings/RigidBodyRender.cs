using System.Drawing;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.RigidBody;

namespace App.View.Renderings
{
    public static class RigidBodyRender
    {
        public static void Draw(RigidShape shapeObject, Pen strokePen, Graphics g)
        {
            if (shapeObject is RigidRectangle)
                DrawRectangle((RigidRectangle) shapeObject, strokePen, g);
            else if (shapeObject is RigidCircle)
                DrawCircle((RigidCircle) shapeObject, strokePen, g);
        }
        
        private static void DrawCircle(RigidCircle shape, Pen strokePen, Graphics g)
        {
            var stateBefore = g.Save();
            if (!shape.Center.Equals(Vector.ZeroVector))
                g.TranslateTransform(shape.Center.X, shape.Center.Y);
            g.DrawEllipse(strokePen, -shape.Radius, -shape.Radius, shape.Diameter, shape.Diameter);
            g.Restore(stateBefore);
        }
        
        private static void DrawRectangle(RigidRectangle shape, Pen strokePen, Graphics g)
        {
            var stateBefore = g.Save();
            if (!shape.Center.Equals(Vector.ZeroVector))
                g.TranslateTransform(shape.Center.X, shape.Center.Y);
            if (shape.angle != 0)
                g.RotateTransform(-shape.angle);
            g.DrawRectangle(strokePen, -shape.Width / 2, -shape.Height / 2, shape.Width, shape.Height);
            g.Restore(stateBefore);
        }
    }
}