namespace App.Engine.Physics.RigidShapes
{
    public class RigidCircleQuarter : RigidShape
    {
        public Vector Direction;
        public Vector DirectionNormal;
        public readonly int QuarterIndex;
        public readonly RigidCircle WholeCircle;

        public RigidCircleQuarter(Vector direction, int quarterIndex, RigidCircle wholeCircle)
        {
            Direction = direction.Normalize();
            DirectionNormal = direction.GetNormal();
            QuarterIndex = quarterIndex;
            WholeCircle = wholeCircle;
        }

        public void Rotate(float angle, Vector rotationCenter)
        {
            WholeCircle.Rotate(angle, rotationCenter);
            Direction = Direction.Rotate(angle, Vector.ZeroVector);
            DirectionNormal = Direction.GetNormal();
        }

        public override Vector Center => WholeCircle.Center;
        public override bool IsStatic { get => WholeCircle.IsStatic; set => WholeCircle.IsStatic = value; }
        public override bool CanCollide { get => WholeCircle.CanCollide; set => WholeCircle.CanCollide = value; }
        public override bool IsCollided { get => WholeCircle.IsCollided; set => WholeCircle.IsCollided = value; }
        public override void MoveBy(Vector delta) => WholeCircle.MoveBy(delta);

        public override void MoveTo(Vector newPosition) => WholeCircle.MoveTo(newPosition);

        public override RigidShape Copy()
        {
            return new RigidCircleQuarter(Direction.Copy(), QuarterIndex, (RigidCircle) WholeCircle.Copy());
        }

        public Vector GetCurveStart()
        {
            var r = WholeCircle.Radius;
            switch (QuarterIndex)
            {
                case 1:
                    return WholeCircle.Center + DirectionNormal * r;
                case 2:
                    return WholeCircle.Center + Direction * r;
                case 3:
                    return WholeCircle.Center - DirectionNormal * r;
                case 4:
                    return WholeCircle.Center - Direction * r;
            }

            return null;
        }
        
        public Vector GetCurveCorner()
        {
            var r = WholeCircle.Radius;
            switch (QuarterIndex)
            {
                case 1:
                    return WholeCircle.Center + DirectionNormal * r + Direction * r;
                case 2:
                    return WholeCircle.Center + Direction * r - DirectionNormal * r;
                case 3:
                    return WholeCircle.Center - DirectionNormal * r - Direction * r;
                case 4:
                    return WholeCircle.Center - Direction * r + DirectionNormal * r;
            }

            return null;
        }
        
        public Vector GetCurveEnd()
        {
            var r = WholeCircle.Radius;
            switch (QuarterIndex)
            {
                case 1:
                    return WholeCircle.Center + Direction * r;
                case 2:
                    return WholeCircle.Center - DirectionNormal * r;
                case 3:
                    return WholeCircle.Center - Direction * r;
                case 4:
                    return WholeCircle.Center + DirectionNormal * r;
            }

            return null;
        }
    }
}