namespace App.Engine.PhysicsEngine.Collision
{
    public class CollisionInfo
    {
        private float depth;
        public float Depth => depth;

        private Vector normal;
        public Vector Normal => normal;

        private Vector collisionStart;
        public Vector CollisionStart => collisionStart;

        private Vector collisionEnd;
        public Vector CollisionEnd => collisionEnd;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depth">the smallest amount that the objects interpenetrated</param>
        /// <param name="normal">the direction along which the collision depth is measured</param>
        /// <param name="collisionStart"></param>
        public CollisionInfo(float depth, Vector normal, Vector collisionStart)
        {
            this.depth = depth;
            this.normal = normal;
            this.collisionStart = collisionStart;
            collisionEnd = collisionStart + this.normal * depth;
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