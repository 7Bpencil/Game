using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidShapes;

namespace App.Model.Entities.Weapons
{
    public class MeleeWeapon
    {
        private readonly Range range;
        private readonly int attackPeriod;
        private int ticksFromLastAttack;
        public readonly int Damage;

        public MeleeWeapon(Vector playerCenter, float startAngle)
        {
            range = new Range(playerCenter);
            range.Rotate(startAngle, playerCenter);

            Damage = 500;
            attackPeriod = 30;
            ticksFromLastAttack = attackPeriod + 1;
        }
        
        public bool IsInRange(Bot target) => range.IsCollide(target.CollisionShape);

        public void IncrementTick() => ticksFromLastAttack++;

        public void ResetTick() => ticksFromLastAttack = 0;
        
        public bool IsReady => ticksFromLastAttack >= attackPeriod;

        public void MoveRangeBy(Vector delta) => range.MoveBy(delta);

        public void RotateRangeTo(float newAngle, Vector playerCenterPosition) =>
            range.Rotate(newAngle, playerCenterPosition);
        
        /// <summary>
        /// Attention: creates new RigidShape[] every time
        /// </summary>
        public RigidShape[] GetRangeShapes() => new RigidShape[] {range.Sector, range.Triangle}; 
        
        private class Range
        {
            public readonly RigidCircleQuarter Sector;
            public readonly RigidTriangle Triangle;
            private float currentAngle;

            public Range(Vector playerCenter)
            {
                var trianglePoints = new[]
                {
                    playerCenter.Copy(),
                    playerCenter + new Vector(0, -74),
                    playerCenter + new Vector(32, 0)
                };
                
                Sector = new RigidCircleQuarter(
                    new Vector(0, -1), 
                    2, 
                    new RigidCircle(playerCenter.Copy(), 74, false, false));
                Triangle = new RigidTriangle(trianglePoints, false, false); 
                currentAngle = 90;
            }
            
            public void Rotate(float newAngle, Vector playerCenterPosition)
            {
                var delta = currentAngle - newAngle;
                Sector.Rotate(delta, playerCenterPosition);
                Triangle.Rotate(delta, playerCenterPosition);
                currentAngle = newAngle;
            }

            public void MoveBy(Vector delta)
            {
                Sector.MoveBy(delta);
                Triangle.MoveBy(delta);
            }

            public bool IsCollide(RigidCircle circle)
            {
                var firstCollision = CollisionDetector.GetCollisionInfo(Sector, circle);
                var secondCollision = CollisionDetector.GetCollisionInfo(Triangle, circle);
                return firstCollision != null || secondCollision != null;
            }
        }
    }
}