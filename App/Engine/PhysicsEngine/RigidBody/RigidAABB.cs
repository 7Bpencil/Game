namespace App.Engine.PhysicsEngine.RigidBody
{
    public class RigidAABB
    {
        private readonly Vector center;
        public readonly Vector MinPoint;
        public readonly Vector MaxPoint;
        public readonly float Width;
        public readonly float Height;

        public RigidAABB(Vector minPoint, Vector maxPoint)
        {
            MinPoint = minPoint;
            MaxPoint = maxPoint;

            Width = maxPoint.X - minPoint.X;
            Height = maxPoint.Y - minPoint.Y;
            
            center = minPoint + new Vector(Width, Height) / 2;
        }
    }
}