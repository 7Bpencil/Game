namespace App.Engine.Physics.RigidBody
{
    public abstract class RigidShape
    {
        public abstract Vector Center { get;}
        public abstract bool IsStatic { get; set; }
        public abstract bool CanCollide { get; set; }
        public abstract bool IsCollided { get; set; }
        public abstract void MoveBy(Vector delta);
        public abstract RigidShape Copy();
    }
}