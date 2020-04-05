using System.Drawing;
using App.Engine.PhysicsEngine.Collision;

namespace App.View.Renderings
{
    public static class CollisionInfoRender
    {
        public static void Draw(CollisionInfo collision, Pen strokePen, Graphics g)
        {
            g.DrawLine(strokePen,
                collision.CollisionStart.X, collision.CollisionStart.Y,
                collision.CollisionEnd.X, collision.CollisionEnd.Y);
        }
    }
}