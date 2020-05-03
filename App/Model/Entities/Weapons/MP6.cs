using System;
using System.Collections.Generic;
using App.Engine.Physics;

namespace App.Model.Entities.Weapons
{
    public class MP6 : Weapon
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

        public MP6(int ammo)
        {
            name = "MP6";
            capacity = 40;
            firePeriod = 3;
            ticksFromLastFire = firePeriod + 1;
            bulletWeight = 0.4f;
            this.ammo = ammo;
            r = new Random();
        }

        public override void IncrementTick() => ticksFromLastFire++;
        
        public override List<Bullet> Fire(Vector playerPosition, CustomCursor cursor)
        {
            var spray = new List<Bullet>();
            var direction = (cursor.Position - playerPosition).Normalize();
            var position = playerPosition + direction * 30;
            
            spray.Add(new Bullet(
                position,
                direction * 30,
                bulletWeight,
                new Edge(playerPosition.Copy(), position),
                20));
            
            ammo--;
            ticksFromLastFire = 0;
            cursor.MoveBy(new Vector(r.Next(-10, 10), r.Next(-10, 10)));

            return spray;
        }
        
        public override void AddAmmo(int amount)
        {
            if (amount > ammo) ammo = amount;
        }
        
        public override bool IsReady() => ticksFromLastFire >= firePeriod && ammo > 0;
    }
}