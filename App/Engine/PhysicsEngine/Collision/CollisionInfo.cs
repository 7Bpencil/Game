namespace App.Engine.PhysicsEngine.Collision
{
    public class CollisionInfo
    {
        public readonly float Depth;
        public readonly Vector Normal;
        public readonly Vector Start;
        public readonly Vector End;
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depth">the smallest amount that the objects interpenetrated</param>
        /// <param name="normal">the direction along which the collision depth is measured</param>
        /// <param name="collisionStart"></param>
        public CollisionInfo(float depth, Vector normal, Vector collisionStart)
        {
            Depth = depth;
            Normal = normal;
            Start = collisionStart;
            End = collisionStart + normal * depth;
        }
    }
}