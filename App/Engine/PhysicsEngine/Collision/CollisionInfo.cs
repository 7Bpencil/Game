using System.Drawing;

namespace App.Engine.PhysicsEngine.Collision
{
    public class CollisionInfo
    {
        private float depth;
        public float Depth { get; set; }

        private Vector normal;
        public Vector Normal { get; set; }

        private Vector collisionStart;
        public Vector CollisionStart { get; set; }

        private Vector collisionEnd;
        public Vector CollisionEnd { get; set; }

        public CollisionInfo(float depth, Vector normal, Vector collisionStart)
        {
            this.depth = depth;
            this.normal = normal;
            this.collisionStart = collisionStart;
            collisionEnd = collisionStart * depth;
        }

        public void ChangeDirection()
        {
            normal *= -1;
            var n = collisionStart;
            collisionStart = collisionEnd;
            collisionEnd = n;
        }
    }
}