using System.Collections.Generic;
using App.Engine.Physics;

namespace App.Model.Entities
{
    public class Bullet : AbstractProjectile
    {
        public readonly Edge Shape;
        private float bulletPenetration;
        public bool IsDeformed;

        public Bullet(Vector position, Vector velocity, int damage, float weight, Edge shape) : 
            base(position, velocity, damage)
        {
            bulletPenetration = Speed * weight;
            StaticPenetrations = new List<float[]> {Capacity = 4};
            Shape = shape;
        }

        public override void Update()
        {
            if (IsDeformed) IsStuck = true;
            foreach (var distanceBeforeCollision in StaticPenetrations)
            {
                distanceBeforeCollision[0] -= Speed; distanceBeforeCollision[1] -= Speed;
                if (distanceBeforeCollision[0] < 0 || distanceBeforeCollision[0] >= Speed ) continue;
                ClosestPenetrationPoint = Position + Velocity.Normalize() * distanceBeforeCollision[1];
                if (bulletPenetration < distanceBeforeCollision[1] - distanceBeforeCollision[0])
                {
                    IsDeformed = true;
                    var stuckPoint = Position + Velocity.Normalize() * distanceBeforeCollision[0];    
                    Shape.End = stuckPoint;
                    Shape.Start = Position.Copy();
                    Position = stuckPoint;
                }
                else SlowDown();
            }
            
            if (StaticPenetrations[StaticPenetrations.Count - 1][1] < -500) IsStuck = true;
            Move();
        }

        public void SlowDown()
        {
            Velocity *= 0.8f;
            Speed *= 0.8f;
            bulletPenetration *= 0.8f;
            Damage = (int) (0.8 * Damage);
        }

        private void Move()
        {
            Position += Velocity;
            Shape.MoveBy(Velocity);
        }
    }
}