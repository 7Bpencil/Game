namespace App.Engine.Physics.RigidShapes
{
    public class RigidCircle : RigidShape
    {
        private readonly float radius;
        public float Radius => radius;
        public float Diameter => 2 * radius;

        private readonly Vector center;
        public override Vector Center => center;
        
        private bool isStatic;
        public override bool IsStatic { get => isStatic; set => isStatic = value; }

        private bool canCollide;
        public override bool CanCollide { get => canCollide; set => canCollide = value; }

        public override bool IsCollided { get; set; }
        
        
        public RigidCircle(Vector center, float radius, bool isStatic, bool canCollide)
        {
            this.radius = radius;
            this.center = center;
            this.isStatic = isStatic;
            this.canCollide = canCollide;
        }

        public override void MoveBy(Vector delta)
        {
            center.X += delta.X;
            center.Y += delta.Y;
        }
        
        public override void MoveTo(Vector newPosition)
        {
            center.X = newPosition.X;
            center.Y = newPosition.Y;
        }

        public void Rotate(float angleInDegrees, Vector rotationCenter)
        {
            MoveTo(Center.Rotate(angleInDegrees, rotationCenter));
        }

        public override RigidShape Copy()
        {
            return new RigidCircle(center.Copy(), radius, isStatic, canCollide);
        }
    }
}