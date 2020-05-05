using System.Collections.Generic;
using System.Drawing;
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
        private Weapon weapon;
        private List<Bullet> sceneBullets;
        public RigidCircle collisionShape;
        public SpriteContainer SpriteContainer;
        private readonly int ticksForMovement;
        private int tick;
        
        private Vector velocity;
        public Vector Velocity => velocity;
        public Vector Center => collisionShape.Center;
        
        public Bot(
            int health, int armour, Vector centerPosition, Vector velocity, int ticksForMovement, Weapon weapon, List<Bullet> sceneBullets)
        {
            this.weapon = weapon;
            this.sceneBullets = sceneBullets;
            Health = health;
            Armour = armour;
            collisionShape = new RigidCircle(centerPosition, 32, false, true);
            this.velocity = velocity;
            IsDead = false;
            this.ticksForMovement = ticksForMovement;
            SpriteContainer = new SpriteContainer(
                new StaticSprite(
                    new Bitmap(@"Assets\TileMaps\shooting_range_target.png"), 0, new Size(64, 64)),
                centerPosition, 0);
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