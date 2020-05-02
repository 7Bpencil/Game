using System.Collections.Generic;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;

namespace App.Model.Entities
{
    public class MeleeWeapon
    {
        public List<RigidCircle> range;
        private float currentAngle;
        private readonly int attackPeriod;
        private int ticksFromLastAttack;
        public readonly int damage;

        public MeleeWeapon(Vector playerCenter, float startAngle)
        {
            range = new List<RigidCircle>
            {
                new RigidCircle(playerCenter + new Vector(18.5f, -32), 18.5f, false, false),
                new RigidCircle(playerCenter + new Vector(-18.5f, -32), 18.5f, false, false),
                new RigidCircle(playerCenter + new Vector(0, -32 - 15), 9, false, false),
            };
            currentAngle = 90;
            RotateRangeShape(startAngle, playerCenter);
        }

        public void RotateRangeShape(float newAngle, Vector playerCenterPosition)
        {
            var delta = currentAngle - newAngle;
            foreach (var circle in range)
                circle.MoveTo(circle.Center.Rotate(delta, playerCenterPosition));
            currentAngle = newAngle;
        }

        public void MoveRangeShapeBy(Vector delta)
        {
            foreach (var circle in range)
                circle.MoveBy(delta);
        }
    }
}