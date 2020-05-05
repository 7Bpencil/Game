using System.Collections.Generic;
using App.Engine.Physics;

namespace App.Model.Entities
{
    public abstract class Weapon
    {
        public abstract string Name { get; }
        public abstract int AmmoAmount { get;}
        public abstract int MagazineCapacity { get;}
        
        /// <summary>
        /// Player version - it plays 2D gunfire sound, and fire has impact on cursor
        /// </summary>
        /// <param name="gunPosition"></param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        public abstract List<Bullet> Fire(Vector gunPosition, CustomCursor cursor);
        
        /// <summary>
        /// Bot version - it plays 3D sound
        /// </summary>
        /// <param name="gunPosition"></param>
        /// <param name="sightDirection"></param>
        /// <param name="listenerPosition"></param>
        /// <returns></returns>
        public abstract List<Bullet> Fire(Vector gunPosition, Vector sightDirection, Vector listenerPosition);
        public abstract void IncrementTick();
        public abstract float BulletWeight { get; }
        public abstract void AddAmmo(int amount);
        public abstract bool IsReady();
    }
}