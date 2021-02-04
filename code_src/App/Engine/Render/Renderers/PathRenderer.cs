using System.Collections.Generic;
using System.Drawing;
using App.Engine.Physics;

namespace App.Engine.Render.Renderers
{
    public static class PathRenderer
    {
        public static void Draw(List<Vector> path, Vector cameraPosition, Graphics g)
        {
            var pathBrush = new SolidBrush(Color.Aqua);
            var pathStrokePen = new Pen(Color.Aqua, 6);
            for (var i = 0; i < path.Count - 1; i++)
            {
                VectorRenderer.Fill(path[i], cameraPosition, pathBrush, g);
                var edgeStartInCamera = path[i].ConvertFromWorldToCamera(cameraPosition);
                var edgeEndInCamera = path[i + 1].ConvertFromWorldToCamera(cameraPosition);
                g.DrawLine(pathStrokePen,
                    edgeStartInCamera.X, edgeStartInCamera.Y,
                    edgeEndInCamera.X, edgeEndInCamera.Y);
            }
        }
    }
}