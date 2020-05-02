using System.Collections.Generic;
using System.Linq;
using App.Engine.Physics;

namespace App.Model.Entities
{
    public class Bullet
    {
        public Vector position;
        public Vector velocity;
        public float speed;
        public List<float[]> staticPenetrations;
        public Edge shape;
        private float bulletPenetration;
        public int damage;
        public bool isStuck;
        private bool isDeformed;

        public Bullet(Vector position, Vector velocity, float weight, Edge shape, int damage)
        {
            this.position = position;
            this.velocity = velocity;
            speed = velocity.Length;
            bulletPenetration = speed * weight;
            staticPenetrations = new List<float[]> {Capacity = 4};
            this.shape = shape;
            this.damage = damage;
        }

        public void CalculateTrajectory()
        {
            staticPenetrations = staticPenetrations.OrderBy(n => n[0]).ToList();
        }

        public void Update()
        {
            if (isDeformed) isStuck = true;
            foreach (var distanceBeforeCollision in staticPenetrations)
            {
                distanceBeforeCollision[0] -= speed; distanceBeforeCollision[1] -= speed;
                if (distanceBeforeCollision[0] < 0 || distanceBeforeCollision[0] >= speed ) continue;
                if (bulletPenetration < distanceBeforeCollision[1] - distanceBeforeCollision[0])
                {
                    isDeformed = true;
                    var stuckPoint = position + velocity.Normalize() * distanceBeforeCollision[0];
                    shape.End = stuckPoint;
                    shape.Start = position.Copy();
                    position = stuckPoint;
                }
                else SlowDown();
            }
            
            if (staticPenetrations[staticPenetrations.Count - 1][1] < -500) isStuck = true;
            Move();
        }
        
        private void SlowDown()
        {
            velocity *= 0.8f;
            speed *= 0.8f;
            bulletPenetration *= 0.8f;
        }

        public void Move()
        {
            position += velocity;
            shape.MoveBy(velocity);
        }
    }
}