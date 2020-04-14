namespace App.Engine.PhysicsEngine.RigidBody
{
    public abstract class RigidShape
    {
        public abstract Vector Center { get;}
        public abstract float BondRadius { get;}
        public abstract bool IsStatic { get; set; }
        public abstract bool CanCollide { get; set; }
        public abstract bool IsCollided { get; set; }
        public abstract void Move(Vector delta);
        public abstract void Rotate(float delta);
        public bool CanBound(RigidShape other)
        {
            return (other.Center - Center).Length < BondRadius + other.BondRadius;
        }
    }
}