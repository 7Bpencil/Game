using System.Drawing;
using App.Engine.Physics;

namespace App.Engine.Render.Renderers
{
    public static class PolygonRenderer
    {
        public static void Draw(Polygon polygon, Vector cameraPosition, Pen strokePen, Graphics g)
        {
            foreach (var edge in polygon.Edges)
            {
                var edgeStartInCamera = edge.Start.ConvertFromWorldToCamera(cameraPosition);
                var edgeEndInCamera = edge.End.ConvertFromWorldToCamera(cameraPosition);
                g.DrawLine(strokePen,
                    edgeStartInCamera.X, edgeStartInCamera.Y,
                    edgeEndInCamera.X, edgeEndInCamera.Y);
            }
        }
        
        public static void Draw(Polygon polygon, Pen strokePen, Graphics g)
        {
            foreach (var edge in polygon.Edges)
            {
                g.DrawLine(strokePen,
                    edge.Start.X, edge.Start.Y,
                    edge.End.X, edge.End.Y);
            }
        }
    }
}