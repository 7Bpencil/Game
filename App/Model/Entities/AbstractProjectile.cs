using System.Collections.Generic;
using System.Linq;
using App.Engine.Physics;

namespace App.Model.Entities
{
    public class AbstractProjectile
    {
        public Vector Position;
        public Vector Velocity;
        public int Damage;
        public List<float[]> StaticPenetrations;
        public float Speed;
        public bool IsStuck;
        public Vector ClosestPenetrationPoint;

        protected AbstractProjectile(Vector position, Vector velocity, int damage)
        {
            Position = position;
            Velocity = velocity;
            Damage = damage;
            Speed = Velocity.Length;
        }
        
        public void CalculateTrajectory()
        {
            StaticPenetrations = StaticPenetrations.OrderBy(n => n[0]).ToList();
        }

        public virtual void Update()
        {
            
        }
    }
}