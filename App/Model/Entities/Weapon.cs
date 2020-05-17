using System;
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
        public abstract List<AbstractProjectile> Fire(Vector gunPosition, CustomCursor cursor);
        
        /// <summary>
        /// Bot version - it plays 3D sound
        /// </summary>
        /// <param name="gunPosition"></param>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        public abstract List<AbstractProjectile> Fire(Vector gunPosition, Vector targetPosition);
        public abstract void IncrementTick();
        public abstract float BulletWeight { get; }
        public abstract void AddAmmo(int amount);
        public abstract bool IsReady { get; }
    }

    public class WeaponInfo
    {
        public readonly Type WeaponType;
        public readonly int AmmoAmount;

        public WeaponInfo(Type weaponType, int ammoAmount)
        {
            WeaponType = weaponType;
            AmmoAmount = ammoAmount;
        }
    }
}