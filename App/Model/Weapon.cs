using System.Collections.Generic;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.RigidBody;

namespace ParallelDrawingTest.Weapons
{
    public abstract class Weapon
    {
        public abstract string Name { get; }
        public abstract int AmmoAmount { get;}
        public abstract int MagazineCapacity { get;}
        public abstract List<Bullet> Fire(Vector playerPosition, RigidCircle cursor);
        public abstract void IncrementTick();
        public abstract float BulletWeight { get; }
        public abstract void AddAmmo(int amount);
    }
}