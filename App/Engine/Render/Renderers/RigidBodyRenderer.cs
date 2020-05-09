using System.Drawing;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;

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
            switch (shapeObject)
            {
                case RigidCircle circle:
                    DrawCircle(circle, cameraPosition, strokePen, g);
                    break;
                case RigidAABB aabb:
                    DrawAABB(aabb, cameraPosition, strokePen, g);
                    break;
                case RigidTriangle triangle:
                    DrawTriangle(triangle, cameraPosition, strokePen, g);
                    break;
                case RigidCircleQuarter circleQuarter:
                    DrawCircleQuarter(circleQuarter, cameraPosition, strokePen, g);
                    break;
            }
        }
        
        /// <summary>
        /// Draws the shapeObject. Coordinates are took in camera axis.
        /// </summary>
        /// <param name="shapeObject"></param>
        /// <param name="strokePen"></param>
        /// <param name="g"></param>
        public static void Draw(RigidShape shapeObject, Pen strokePen, Graphics g)
        {
            switch (shapeObject)
            {
                case RigidCircle circle:
                    DrawCircle(circle, strokePen, g);
                    break;
                case RigidAABB aabb:
                    DrawAABB(aabb, strokePen, g);
                    break;
                case RigidTriangle triangle:
                    DrawTriangle(triangle, strokePen, g);
                    break;
                case RigidCircleQuarter circleQuarter:
                    DrawCircleQuarter(circleQuarter, strokePen, g);
                    break;
            }
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

        private static void DrawAABB(RigidAABB shape, Vector cameraPosition, Pen strokePen, Graphics g)
        {
            var inCameraPosition = shape.MinPoint.ConvertFromWorldToCamera(cameraPosition);
            g.DrawRectangle(strokePen, inCameraPosition.X, inCameraPosition.Y, shape.Width, shape.Height);
            
        }
        
        private static void DrawAABB(RigidAABB shape, Pen strokePen, Graphics g)
        {
            g.DrawRectangle(strokePen, shape.MinPoint.X, shape.MinPoint.Y, shape.Width, shape.Height);
        }
        
        private static void DrawTriangle(RigidTriangle shape, Vector cameraPosition, Pen strokePen, Graphics g)
        {
            var pointsInCamera = new PointF[3];
            for (var i = 0; i < 3; i++)
                pointsInCamera[i] = shape.Points[i].ConvertFromWorldToCamera(cameraPosition).GetPoint();
            
            g.DrawPolygon(strokePen, pointsInCamera);
        }
        
        private static void DrawTriangle(RigidTriangle shape, Pen strokePen, Graphics g)
        {
            var points = new PointF[3];
            for (var i = 0; i < 3; i++)
                points[i] = shape.Points[i].GetPoint();
            
            g.DrawPolygon(strokePen, points);
        }
        
        private static void DrawCircleQuarter(RigidCircleQuarter shape, Vector cameraPosition, Pen strokePen, Graphics g)
        {
            var start = shape.GetCurveStart().ConvertFromWorldToCamera(cameraPosition).GetPoint();
            var end = shape.GetCurveEnd().ConvertFromWorldToCamera(cameraPosition).GetPoint(); 
            g.DrawBezier(strokePen, 
                start,
                start,
                shape.GetCurveCorner().ConvertFromWorldToCamera(cameraPosition).GetPoint(),
                end);
            EdgeRenderer.Draw(new Edge(shape.Center, shape.GetCurveStart()), cameraPosition, strokePen, g);
            EdgeRenderer.Draw(new Edge(shape.Center, shape.GetCurveEnd()), cameraPosition, strokePen, g);
        }
        
        private static void DrawCircleQuarter(RigidCircleQuarter shape, Pen strokePen, Graphics g)
        {
            var start = shape.GetCurveStart();
            var end = shape.GetCurveEnd(); 
            g.DrawBezier(strokePen, 
                start.GetPoint(),
                start.GetPoint(),
                shape.GetCurveCorner().GetPoint(),
                end.GetPoint());
            EdgeRenderer.Draw(new Edge(shape.Center, start), strokePen, g);
            EdgeRenderer.Draw(new Edge(shape.Center, end), strokePen, g);
        }
        
    }
}