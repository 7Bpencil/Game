using System;
using System.Collections.Generic;
using App.Engine.Audio;
using App.Engine.Physics;

namespace App.Model.Entities.Weapons
{
    public class MP6 : Weapon
    {
        private readonly Random r;
        private readonly string fireSoundPath;
        private readonly string fireSoundPath3D;

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
            
            fireSoundPath = @"event:/gunfire/2D/MP6_FIRE";
            fireSoundPath3D = @"event:/gunfire/3D/MP6_FIRE_3D";
        }

        public override void IncrementTick() => ticksFromLastFire++;
        
        public override List<Bullet> Fire(Vector gunPosition, CustomCursor cursor)
        {
            var spray = new List<Bullet>();
            var direction = (cursor.Position - gunPosition).Normalize();
            var position = gunPosition + direction * 48;
            
            spray.Add(new Bullet(
                position,
                direction * 30,
                bulletWeight,
                new Edge(position, position + direction * 32),
                20));
            
            ammo--;
            ticksFromLastFire = 0;
            AudioEngine.PlayNewInstance(fireSoundPath);
            cursor.MoveBy(new Vector(r.Next(-10, 10), r.Next(-10, 10)));

            return spray;
        }
        
        public override List<Bullet> Fire(Vector gunPosition, Vector sightDirection)
        {
            var spray = new List<Bullet>();
            var direction = sightDirection.Normalize();
            var position = gunPosition + direction * 48;
            
            spray.Add(new Bullet(
                position,
                direction * 30,
                bulletWeight,
                new Edge(position, position + direction * 32),
                20));
            
            ammo--;
            ticksFromLastFire = 0;
            AudioEngine.PlayNewInstance(fireSoundPath3D, gunPosition);

            return spray;
        }
        
        public override void AddAmmo(int amount)
        {
            if (amount > ammo) ammo = amount;
        }
        
        public override bool IsReady => ticksFromLastFire >= firePeriod && ammo > 0;
    }
}