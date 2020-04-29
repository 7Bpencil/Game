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
                    Vector.ConvertFromWorldToCamera(lightSourcePosition, cameraPosition),
                    Vector.ConvertFromWorldToCamera(visibilityPolygonPoints[i].position, cameraPosition),
                    Vector.ConvertFromWorldToCamera(visibilityPolygonPoints[i + 1].position, cameraPosition), brush, g);
            FillTriangle(
                Vector.ConvertFromWorldToCamera(lightSourcePosition, cameraPosition),
                Vector.ConvertFromWorldToCamera(visibilityPolygonPoints[visibilityPolygonPoints.Count - 1].position, cameraPosition),
                Vector.ConvertFromWorldToCamera(visibilityPolygonPoints[0].position, cameraPosition), brush, g);
        }
        
        private static void FillTriangle(Vector a, Vector b, Vector c, Brush brush, Graphics g)
        {
            g.FillPolygon(brush, new []{a.GetPoint(), b.GetPoint(), c.GetPoint()});
        }
    }
}