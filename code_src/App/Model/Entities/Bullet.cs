using System.Collections.Generic;
using System.Linq;
using App.Engine.Physics;

namespace App.Model.Entities
{
    public class Bullet
    {
        public Vector Position;
        public Vector Velocity;
        public float Speed;
        public List<float[]> StaticPenetrations;
        public readonly Edge Shape;
        private float bulletPenetration;
        public int Damage;
        public bool IsStuck;
        public bool IsDeformed;
        public Vector ClosestPenetrationPoint;

        public Bullet(Vector position, Vector velocity, float weight, Edge shape, int damage)
        {
            Position = position;
            Velocity = velocity;
            Speed = velocity.Length;
            bulletPenetration = Speed * weight;
            StaticPenetrations = new List<float[]> {Capacity = 4};
            Shape = shape;
            Damage = damage;
        }

        public void CalculateTrajectory()
        {
            StaticPenetrations = StaticPenetrations.OrderBy(n => n[0]).ToList();
        }

        public void Update()
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
