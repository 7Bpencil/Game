using System.Collections.Generic;
using App.Engine.Physics;
using App.Engine.Physics.RigidBody;

namespace App.Model
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