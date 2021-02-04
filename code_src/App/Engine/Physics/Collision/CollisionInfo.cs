namespace App.Engine.Physics.Collision
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

        public override string ToString()
        {
            return "Depth=" + Depth.ToString() + ", Normal=" + Normal + ", Start=" + Start;
        }

        private bool Equals(CollisionInfo other)
        {
            return Depth.Equals(other.Depth)
                   && Normal.Equals(other.Normal)
                   && Start.Equals(other.Start)
                   && End.Equals(other.End);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CollisionInfo) obj);
        }
    }
}
