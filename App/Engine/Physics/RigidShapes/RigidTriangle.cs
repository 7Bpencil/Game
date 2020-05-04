using System;

namespace App.Engine.Physics.RigidShapes
{
    public class RigidTriangle : RigidShape
    {
        public readonly Vector[] Points;

        public RigidTriangle(Vector a, Vector b, Vector c, bool isStatic, bool canCollide)
        {
            Points = new[] {a, b, c};
            this.isStatic = isStatic;
            this.canCollide = canCollide;

            Center = null;
        }

        public RigidTriangle(Vector[] points, bool isStatic, bool canCollide)
        {
            if (points.Length == 3) points = this.Points;
            else throw new ArgumentException();
            this.isStatic = isStatic;
            this.canCollide = canCollide;

            Center = null;
        }

        public override Vector Center { get; }
        
        private bool isStatic;
        public override bool IsStatic { get => isStatic; set => isStatic = value; }

        private bool canCollide;
        public override bool CanCollide { get => canCollide; set => canCollide = value; }

        public override bool IsCollided { get; set; }
        
        public override void MoveBy(Vector delta)
        {
            throw new System.NotImplementedException();
        }

        public override void MoveTo(Vector newPosition)
        {
            throw new System.NotImplementedException();
        }

        public override RigidShape Copy()
        {
            return new RigidTriangle(new []{Points[0].Copy(), Points[1].Copy(), Points[2].Copy()}, isStatic, canCollide);
        }
    }
}