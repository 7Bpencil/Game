using System.Drawing;
using App.Engine.Physics;

namespace App.Engine.Render.Renderers
{
    public static class EdgeRenderer
    {
        public static void Draw(Edge edge, Vector cameraPosition, Pen strokePen, Graphics g)
        {
            var edgeStartInCamera = Vector.ConvertFromWorldToCamera(edge.Start, cameraPosition);
            var edgeEndInCamera = Vector.ConvertFromWorldToCamera(edge.End, cameraPosition);
            g.DrawLine(strokePen,
                edgeStartInCamera.X, edgeStartInCamera.Y,
                edgeEndInCamera.X, edgeEndInCamera.Y);
        }

        public static void Draw(Edge edge, Pen strokePen, Graphics g)
        {
            g.DrawLine(strokePen,
                    edge.Start.X, edge.Start.Y,
                    edge.End.X, edge.End.Y);
        }
    }
}