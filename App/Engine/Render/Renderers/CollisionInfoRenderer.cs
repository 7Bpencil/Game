using System.Drawing;
using App.Engine.Physics;
using App.Engine.Physics.Collision;

namespace App.Engine.Render.Renderers
{
    public static class CollisionInfoRenderer
    {
        public static void Draw(CollisionInfo collision, Vector cameraPosition, Pen strokePen, Graphics g)
        {
            var collisionStartInCamera = collision.Start.ConvertFromWorldToCamera(cameraPosition);
            var collisionEndInCamera = collision.End.ConvertFromWorldToCamera(cameraPosition);
            g.DrawEllipse(strokePen, 
                collisionEndInCamera.X - strokePen.Width, collisionEndInCamera.Y - strokePen.Width, 
                2 * strokePen.Width, 2 * strokePen.Width);
            g.DrawLine(strokePen,
                collisionStartInCamera.X, collisionStartInCamera.Y,
                collisionEndInCamera.X, collisionEndInCamera.Y);
        }
    }
}