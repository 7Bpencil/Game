using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using App.Model.Entities;

namespace App.Model
{
    public class LivingEntity
    {
        public int Health;
        public int Armor;
        public bool IsDead;
        public readonly RigidCircle CollisionShape;
        public readonly SpriteContainer TorsoContainer;
        public readonly SpriteContainer LegsContainer;
        public Vector Velocity;
        public Vector Position => CollisionShape.Center;

        public readonly string DeadBodyPath;

        protected LivingEntity(
            int health, int armor, RigidCircle collisionShape, SpriteContainer legsContainer, 
            SpriteContainer torsoContainer, string deadBodyPath)
        {
            Health = health;
            Armor = armor;
            CollisionShape = collisionShape;
            TorsoContainer = torsoContainer;
            LegsContainer = legsContainer;
            Velocity = Vector.ZeroVector;
            this.DeadBodyPath = deadBodyPath;
        }

        public void MoveTo(Vector newPosition) => CollisionShape.MoveTo(newPosition);

        public void MoveBy(Vector delta) => CollisionShape.MoveBy(delta);

        public void TakeHit(int damage)
        {
            if (IsDead) return;
            Armor -= damage;
            if (Armor < 0)
            {
                Health += Armor;
                Armor = 0;
            }

            if (Health <= 0) IsDead = true;
        }
    }
}