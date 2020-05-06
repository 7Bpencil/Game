using System.Collections.Generic;
using System.Drawing;
using App.Engine;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using App.Engine.Sprites;

namespace App.Model.Entities
{
    public class Bot
    {
        public int Health;
        public int Armour;
        public bool IsDead;
        public readonly SpriteContainer TorsoContainer;
        public readonly SpriteContainer LegsContainer;
        private Weapon weapon;
        private List<Bullet> sceneBullets;
        public readonly RigidCircle CollisionShape;
        private readonly int ticksForMovement;
        private int tick;
        
        private Vector velocity;
        public Vector Velocity => velocity;
        public Vector Center => CollisionShape.Center;
        
        public Bot(
            int health, int armour, Vector startPosition, float startAngle, 
            Sprite legs, Sprite torso, Vector velocity, RigidCircle collisionShape,
            int ticksForMovement, Weapon weapon, List<Bullet> sceneBullets)
        {
            this.weapon = weapon;
            this.sceneBullets = sceneBullets;
            Health = health;
            Armour = armour;
            CollisionShape = collisionShape; 
            this.velocity = velocity;
            IsDead = false;
            this.ticksForMovement = ticksForMovement;
            LegsContainer = new SpriteContainer(legs, startPosition, startAngle);
            TorsoContainer = new SpriteContainer(torso, startPosition, startAngle);
        }
        
        public void TakeHit(int damage)
        {
            if (IsDead) return;
            Armour -= damage;
            if (Armour < 0)
            {
                Health += Armour;
                Armour = 0;
            }

            if (Health <= 0) IsDead = true;
        }
        
        public void MoveBy(Vector delta) => collisionShape.MoveBy(delta);
        public void MoveTo(Vector newPosition) => collisionShape.MoveTo(newPosition);
        public void ChangeVelocity(Vector newVelocity) => velocity = newVelocity;
        public void Update()
        {
            tick++;
            weapon.IncrementTick();
            if (!IsDead && weapon.IsReady)
            {
                //sceneBullets.AddRange(weapon.Fire(collisionShape.Center, new Vector(1, 0), collisionShape.Center));
            }
            if (tick > ticksForMovement)
            {
                tick = 0;
                velocity = -velocity;
            }
            MoveBy(velocity);
        }
    }
}