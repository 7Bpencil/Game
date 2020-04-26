using System.Collections.Generic;
using System.Drawing;
using App.Engine.PhysicsEngine;

namespace App.Engine.Render.Renderers
{
    public static class VisibilityPolygonRenderer
    {
        public static void Draw(Vector lightSourcePosition, List<Raytracing.RaytracingPoint> visibilityPolygonPoints, Brush brush, Graphics g)
        {
            for (var i = 0; i < visibilityPolygonPoints.Count - 1; i++)
                FillTriangle(
                    lightSourcePosition,
                    visibilityPolygonPoints[i].position,
                    visibilityPolygonPoints[i + 1].position, brush, g);
            FillTriangle(
                lightSourcePosition,
                visibilityPolygonPoints[visibilityPolygonPoints.Count - 1].position,
                visibilityPolygonPoints[0].position, brush, g);
        }
        
        private static void FillTriangle(Vector a, Vector b, Vector c, Brush brush, Graphics g)
        {
            g.FillPolygon(brush, new []{a.GetPoint(), b.GetPoint(), c.GetPoint()});
        }
    }
}