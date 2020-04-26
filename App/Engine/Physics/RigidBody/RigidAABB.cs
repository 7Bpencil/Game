namespace App.Engine.Physics.RigidBody
{
    public class RigidAABB : RigidShape
    {
        private readonly Vector center;
        public override Vector Center => center;
        
        public readonly Vector MinPoint;
        public readonly Vector MaxPoint;
        public readonly float Width;
        public readonly float Height;
        
        private bool isStatic;
        public override bool IsStatic { get => isStatic; set => isStatic = value; }
        
        private bool canCollide;
        public override bool CanCollide { get => canCollide; set => canCollide = value; }

        public override bool IsCollided { get; set; }

        public RigidAABB(Vector minPoint, Vector maxPoint, bool isStatic, bool canCollide)
        {
            MinPoint = minPoint;
            MaxPoint = maxPoint;
            this.isStatic = isStatic;
            this.canCollide = canCollide;

            Width = maxPoint.X - minPoint.X;
            Height = maxPoint.Y - minPoint.Y;
            center = minPoint + new Vector(Width, Height) / 2;
        }

        public override void MoveBy(Vector delta)
        {
            throw new System.NotImplementedException();
        }

        public override RigidShape Copy()
        {
            return new RigidAABB(MinPoint, MaxPoint, isStatic, canCollide);
        }
    }
}