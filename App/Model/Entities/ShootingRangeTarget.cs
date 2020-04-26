using App.Engine.Physics;
using App.Engine.Physics.RigidBody;

namespace App.Model.Entities
{
    public class ShootingRangeTarget
    {
        public int Health;
        public int Armour;
        public bool isDead;
        public RigidCircle collisionShape;
        private int tick;
        
        private Vector velocity;
        public Vector Velocity => velocity;
        
        public ShootingRangeTarget(int health, int armour, RigidCircle collisionShape, Vector velocity, bool isDead)
        {
            Health = health;
            Armour = armour;
            this.collisionShape = collisionShape;
            this.velocity = velocity;
            this.isDead = isDead;
        }
        
        public void TakeHit(int damage)
        {
            if (isDead) return;
            Armour -= damage;
            if (Armour < 0)
            {
                Health += Armour;
                Armour = 0;
            }

            if (Health <= 0) isDead = true;
        }
        
        public void MoveBy(Vector delta) => collisionShape.MoveBy(delta);
        public void MoveTo(Vector newPosition) => collisionShape.MoveTo(newPosition);
        public void ChangeVelocity(Vector newVelocity) => velocity = newVelocity;
        public void IncrementTick()
        {
            tick++;
            if (tick > 80)
            {
                tick = 0;
                velocity = -velocity;
            }
            MoveBy(velocity);
        }
    }
}