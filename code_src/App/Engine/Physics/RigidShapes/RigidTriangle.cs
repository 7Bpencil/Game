using System;

namespace App.Engine.Physics.RigidShapes
{
    public class RigidTriangle : RigidShape
    {
        public readonly Vector[] Points;
        private Vector center;
        public override Vector Center => center;

        private bool isStatic;
        public override bool IsStatic { get => isStatic; set => isStatic = value; }

        private bool canCollide;
        public override bool CanCollide { get => canCollide; set => canCollide = value; }

        public override bool IsCollided { get; set; }

        public RigidTriangle(Vector[] points, bool isStatic, bool canCollide)
        {
            if (points.Length != 3) throw new ArgumentException();
            Points = points;
            center = (points[0] + points[1] + points[2]) / 3;
            this.isStatic = isStatic;
            this.canCollide = canCollide;
        }

        public RigidTriangle(Vector a, Vector b, Vector c, bool isStatic, bool canCollide)
        {
            Points = new[] {a, b, c};
            this.isStatic = isStatic;
            this.canCollide = canCollide;
        }

        public override void MoveBy(Vector delta)
        {
            for (var i = 0; i < 3; i++)
            {
                Points[i].X += delta.X;
                Points[i].Y += delta.Y;
            }
            CalculateCenter();
        }

        public override void MoveTo(Vector newCenterPosition)
        {
            var delta = newCenterPosition - Center;
            MoveBy(delta);
        }

        public void Rotate(float angleInDegrees, Vector rotationCenter)
        {
            for (var i = 0; i < 3; i++)
                Points[i] = Points[i].Rotate(angleInDegrees, rotationCenter);
            CalculateCenter();
        }

        public override RigidShape Copy()
        {
            var pointsCopy = new[] { Points[0].Copy(), Points[1].Copy(), Points[2].Copy()};
            return new RigidTriangle(pointsCopy, isStatic, canCollide);
        }

        private void CalculateCenter() => center = (Points[0] + Points[1] + Points[2]) / 3;
    }
}
