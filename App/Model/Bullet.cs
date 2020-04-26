using System.Collections.Generic;
using App.Engine.PhysicsEngine;

namespace ParallelDrawingTest.Weapons
{
    public class Bullet
    {
        public Vector position;
        public Vector velocity;
        public readonly List<Vector[]> collisionWithStaticInfo;
        public readonly List<Vector> collisionWithDynamicInfo;
        public Edge shape;
        public float bulletPenetration;
        public int damage;

        public Bullet(Vector position, Vector velocity, float weight, Edge shape, int damage)
        {
            this.position = position;
            this.velocity = velocity;
            collisionWithStaticInfo = new List<Vector[]>();
            collisionWithDynamicInfo = new List<Vector>();
            this.shape = shape;
            bulletPenetration = CalculateBulletPenetration(velocity, weight);
            this.damage = damage;
        }

        public void Move()
        {
            position += velocity;
            shape.Move(velocity);
        }

        private static float CalculateBulletPenetration(Vector bulletVelocity, float bulletWeight)
        {
            return bulletVelocity.Length * bulletWeight;
        }

        public bool CanPenetrate(Vector[] penetrationPlaces)
        {
            return (penetrationPlaces[1] - penetrationPlaces[0]).Length < bulletPenetration;
        }
    }
}