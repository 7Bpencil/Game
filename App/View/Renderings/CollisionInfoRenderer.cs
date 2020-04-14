using System.Drawing;
using App.Engine.PhysicsEngine.Collision;

namespace App.View.Renderings
{
    public static class CollisionInfoRenderer
    {
        public static void Draw(CollisionInfo collision, Pen strokePen, Graphics g)
        {
            g.DrawEllipse(strokePen, 
                collision.End.X - strokePen.Width, collision.End.Y - strokePen.Width, 
                2 * strokePen.Width, 2 * strokePen.Width);
            g.DrawLine(strokePen,
                collision.Start.X, collision.Start.Y,
                collision.End.X, collision.End.Y);
        }
    }
}