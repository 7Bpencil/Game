using System;
using System.Collections.Generic;
using App.Engine.Physics;

namespace App.Model.Entities.Weapons
{
    public class Shotgun : Weapon
    {
        private readonly Random r;

        private readonly string name;
        public override string Name => name;
        
        private readonly int capacity;
        public override int MagazineCapacity => capacity;
        
        private readonly float bulletWeight;
        public override float BulletWeight => bulletWeight;
        
        private int ammo;
        public override int AmmoAmount => ammo;

        private readonly int firePeriod;
        private int ticksFromLastFire;
        
        public Shotgun(int ammo)
        {
            name = "Shotgun";
            capacity = 8;
            firePeriod = 33;
            ticksFromLastFire = firePeriod + 1;
            bulletWeight = 0.3f;
            this.ammo = ammo;
            r = new Random();
        }

        public override void IncrementTick() => ticksFromLastFire++;

        public override List<Bullet> Fire(Vector playerPosition, CustomCursor cursor)
        {
            if (ticksFromLastFire < firePeriod 
                || ammo == 0) return null;
            
            var spray = new List<Bullet>();
            
            var direction = (cursor.Position - playerPosition).Normalize();
            
            const int shotsAmount = 6;
            for (var i = 0; i < shotsAmount; i++)
            {
                var offset = new Vector(r.Next(-3, 3), r.Next(-3, 3)) / 30;
                var e = direction + offset;
                var position = playerPosition + e * 40;
                spray.Add(new Bullet(
                    position,
                    e * 30,
                    bulletWeight,
                    new Edge(playerPosition.Copy(), position),
                    12));
            }

            ammo--;
            ticksFromLastFire = 0;
            cursor.MoveBy(new Vector(r.Next(-40, 40), r.Next(-40, 40)));

            return spray;
        }
        
        public override void AddAmmo(int amount)
        {
            ammo += amount;
            if (ammo > capacity) ammo = capacity;
        }
    }
}