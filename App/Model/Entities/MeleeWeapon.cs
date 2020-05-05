using System.Collections.Generic;
using App.Engine.Physics;
using App.Engine.Physics.Collision;
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

            damage = 500;
            attackPeriod = 33;
            ticksFromLastAttack = attackPeriod + 1;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targets"></param>
        /// <returns>true if there was at least one hit</returns>
        public bool Attack(List<Bot> targets)
        {
            var createBlood = false;
            foreach (var target in targets)
            {
                var wasHit = false;
                foreach (var circle in range)
                {
                    if (CollisionSolver.GetCollisionInfo(circle, target.collisionShape) == null) continue;
                    wasHit = createBlood = true;
                    break;
                }
                if (wasHit) target.TakeHit(damage);
            }

            ticksFromLastAttack = 0;
            return createBlood;
        }
        
        public void IncrementTick() => ticksFromLastAttack++;

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

        public bool IsReady => ticksFromLastAttack >= attackPeriod;
    }
}