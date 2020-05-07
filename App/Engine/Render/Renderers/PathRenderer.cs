using System.Collections.Generic;
using System.Drawing;
using App.Engine.Physics;
using App.Model.LevelData;

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

            var start = new Vector(22 * 32, 29 * 32).ConvertFromWorldToCamera(cameraPosition);
            var end = new Vector(8 * 32, 10 * 32).ConvertFromWorldToCamera(cameraPosition);
            g.DrawEllipse(strokePen,  start.X - 10, start.Y - 10, 20, 20);
            g.DrawEllipse(strokePen, end.X - 10, end.Y - 10, 20, 20);
        }
    }
}