using System.Collections.Generic;
using System.Drawing;
using App.Engine.Physics;

namespace App.Engine.Render.Renderers
{
    public static class PathRenderer
    {
        public static void Draw(List<Vector> path, Vector cameraPosition, Pen strokePen, Graphics g)
        {
            for (var i = 0; i < path.Count - 1; i++)
            {
                var edgeStartInCamera = path[i].ConvertFromWorldToCamera(cameraPosition);
                var edgeEndInCamera = path[i + 1].ConvertFromWorldToCamera(cameraPosition);
                g.DrawLine(strokePen,
                    edgeStartInCamera.X, edgeStartInCamera.Y,
                    edgeEndInCamera.X, edgeEndInCamera.Y);
            }
        }
    }
}