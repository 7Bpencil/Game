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
        public readonly RigidCircle CollisionShape;
        private readonly Weapon weapon;

        public Vector Center => CollisionShape.Center;
        
        public Bot(
            int health, int armour, Vector startPosition, float startAngle, 
            Sprite legs, Sprite torso, RigidCircle collisionShape, Weapon weapon)
        {
            Health = health;
            Armour = armour;
            CollisionShape = collisionShape;
            LegsContainer = new SpriteContainer(legs, startPosition, startAngle);
            TorsoContainer = new SpriteContainer(torso, startPosition, startAngle);
            this.weapon = weapon;
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
        
        public void MoveTo(Vector newPosition) => CollisionShape.MoveTo(newPosition);
        public void Update() // Placeholder
        {
        }
    }
}