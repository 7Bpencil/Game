using System.Collections.Generic;
using System.Drawing;
using App.Engine.Physics;

namespace App.Engine.Render.Renderers
{
    public static class VisibilityPolygonRenderer
    {
        public static void Draw(
            Vector lightSourcePosition,
            List<Raytracing.RaytracingPoint> visibilityPolygonPoints,
            Vector cameraPosition, Brush brush, Graphics g)
        {
            for (var i = 0; i < visibilityPolygonPoints.Count - 1; i++)
                FillTriangle(
                    lightSourcePosition.ConvertFromWorldToCamera(cameraPosition),
                    visibilityPolygonPoints[i].Position.ConvertFromWorldToCamera(cameraPosition),
                    visibilityPolygonPoints[i + 1].Position.ConvertFromWorldToCamera(cameraPosition), brush, g);
            FillTriangle(
                lightSourcePosition.ConvertFromWorldToCamera(cameraPosition),
                visibilityPolygonPoints[visibilityPolygonPoints.Count - 1].Position.ConvertFromWorldToCamera(cameraPosition),
                visibilityPolygonPoints[0].Position.ConvertFromWorldToCamera(cameraPosition), brush, g);
        }

        private static void FillTriangle(Vector a, Vector b, Vector c, Brush brush, Graphics g)
        {
            g.FillPolygon(brush, new []{a.GetPointF(), b.GetPointF(), c.GetPointF()});
        }
    }
}
