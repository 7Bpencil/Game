using System.Drawing;
using App.Engine.Physics;
using App.Engine.Physics.RigidBody;

namespace App.Engine.Render.Renderers
{
    public static class RigidBodyRenderer
    {
        /// <summary>
        /// Draws the shapeObject. Coordinates are took in world axis.
        /// </summary>
        /// <param name="shapeObject"></param>
        /// <param name="cameraPosition"></param>
        /// <param name="strokePen"></param>
        /// <param name="g"></param>
        public static void Draw(RigidShape shapeObject, Vector cameraPosition, Pen strokePen, Graphics g)
        {
            if (shapeObject is RigidOBB)
                DrawOBB((RigidOBB) shapeObject, cameraPosition, strokePen, g);
            else if (shapeObject is RigidCircle)
                DrawCircle((RigidCircle) shapeObject, cameraPosition, strokePen, g);
            else if (shapeObject is RigidAABB)
                DrawAABB((RigidAABB) shapeObject, cameraPosition, strokePen, g);
        }
        
        /// <summary>
        /// Draws the shapeObject. Coordinates are took in camera axis.
        /// </summary>
        /// <param name="shapeObject"></param>
        /// <param name="strokePen"></param>
        /// <param name="g"></param>
        public static void Draw(RigidShape shapeObject, Pen strokePen, Graphics g)
        {
            if (shapeObject is RigidOBB)
                DrawOBB((RigidOBB) shapeObject, strokePen, g);
            else if (shapeObject is RigidCircle)
                DrawCircle((RigidCircle) shapeObject, strokePen, g);
            else if (shapeObject is RigidAABB)
                DrawAABB((RigidAABB) shapeObject, strokePen, g);
        }
        
        private static void DrawCircle(RigidCircle shape, Vector cameraPosition, Pen strokePen, Graphics g)
        {
            var inCameraPosition = shape.Center.ConvertFromWorldToCamera(cameraPosition);
            g.DrawEllipse(strokePen,
                inCameraPosition.X - shape.Radius, inCameraPosition.Y - shape.Radius,
                shape.Diameter, shape.Diameter);
        }
        
        private static void DrawCircle(RigidCircle shape, Pen strokePen, Graphics g)
        {
            g.DrawEllipse(strokePen,
                shape.Center.X - shape.Radius, shape.Center.Y - shape.Radius,
                shape.Diameter, shape.Diameter);
        }
        
        private static void DrawOBB(RigidOBB shape, Vector cameraPosition, Pen strokePen, Graphics g)
        {
            var inCameraPosition = shape.Center.ConvertFromWorldToCamera(cameraPosition);
            var stateBefore = g.Save();
            if (!shape.Center.Equals(Vector.ZeroVector))
                g.TranslateTransform(inCameraPosition.X, inCameraPosition.Y);
            if (shape.angle != 0)
                g.RotateTransform(-shape.angle);
            g.DrawRectangle(strokePen, -shape.Width / 2, -shape.Height / 2, shape.Width, shape.Height);
            g.Restore(stateBefore);
        }
        
        private static void DrawOBB(RigidOBB shape, Pen strokePen, Graphics g)
        {
            var stateBefore = g.Save();
            if (!shape.Center.Equals(Vector.ZeroVector))
                g.TranslateTransform(shape.Center.X, shape.Center.Y);
            if (shape.angle != 0)
                g.RotateTransform(-shape.angle);
            g.DrawRectangle(strokePen, -shape.Width / 2, -shape.Height / 2, shape.Width, shape.Height);
            g.Restore(stateBefore);
        }
        
        private static void DrawAABB(RigidAABB shape, Vector cameraPosition, Pen strokePen, Graphics g)
        {
            var inCameraPosition = shape.MinPoint.ConvertFromWorldToCamera(cameraPosition);
            g.DrawRectangle(strokePen, inCameraPosition.X, inCameraPosition.Y, shape.Width, shape.Height);
            
        }
        
        private static void DrawAABB(RigidAABB shape, Pen strokePen, Graphics g)
        {
            g.DrawRectangle(strokePen, shape.MinPoint.X, shape.MinPoint.Y, shape.Width, shape.Height);
        }
    }
}