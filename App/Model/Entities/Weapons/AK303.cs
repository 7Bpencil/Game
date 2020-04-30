using System;
using System.Collections.Generic;
using App.Engine.Physics;

namespace App.Model.Entities.Weapons
{
    public class AK303 : Weapon
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

        public AK303(int ammo)
        {
            name = "AK-303";
            capacity = 30;
            firePeriod = 5;
            ticksFromLastFire = 0;
            bulletWeight = 1f;
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
            var position = playerPosition + direction * 40;
            spray.Add(new Bullet(
                position,
                direction * 40,
                bulletWeight,
                new Edge(playerPosition.Copy(), position),
                40));
            
            ammo--;
            ticksFromLastFire = 0;
            cursor.MoveBy(direction.GetNormal() * r.Next(-30, 30) + new Vector(r.Next(2, 2), r.Next(2, 2)));

            return spray;
        }
        
        public override void AddAmmo(int amount)
        {
            if (amount > ammo) ammo = amount;
        }
    }
}