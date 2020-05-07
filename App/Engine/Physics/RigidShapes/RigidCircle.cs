namespace App.Engine.Physics.RigidShapes
{
    public class RigidCircle : RigidShape
    {
        private readonly float radius;
        public float Radius => radius;
        public float Diameter => 2 * radius;
        private RigidAABB boundShape;

        private readonly Vector center;
        public override Vector Center => center;
        
        private bool isStatic;
        public override bool IsStatic { get => isStatic; set => isStatic = value; }

        private bool canCollide;
        public override bool CanCollide { get => canCollide; set => canCollide = value; }

        public override bool IsCollided { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="isStatic"></param>
        /// <param name="canCollide">should a collision be calculated</param>
        public RigidCircle(Vector center, float radius, bool isStatic, bool canCollide)
        {
            this.radius = radius;
            this.center = center;
            this.isStatic = isStatic;
            this.canCollide = canCollide;
            
            var v = new Vector(radius, radius);
            boundShape = new RigidAABB(center - v, center + v, isStatic, canCollide);
        }

        public override void MoveBy(Vector delta)
        {
            center.X += delta.X;
            center.Y += delta.Y;
            var v = new Vector(radius, radius);
            boundShape = new RigidAABB(center - v, center + v, isStatic, canCollide);
        }
        
        public override void MoveTo(Vector newPosition)
        {
            center.X = newPosition.X;
            center.Y = newPosition.Y;
            var v = new Vector(radius, radius);
            boundShape = new RigidAABB(center - v, center + v, isStatic, canCollide);
        }

        public override RigidShape Copy()
        {
            return new RigidCircle(center.Copy(), radius, isStatic, canCollide);
        }

        public override RigidAABB BoundShape => boundShape;
    }
}