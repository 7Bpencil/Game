using System;
using System.Collections.Generic;
using App.Engine.Audio;
using App.Engine.Physics;

namespace App.Model.Entities.Weapons
{
    public class SaigaFA : Weapon
    {
        private readonly Random r;
        private readonly string fireSoundPath;

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

        public SaigaFA(int ammo)
        {
            name = "Saiga Full-Auto";
            capacity = 20;
            firePeriod = 8;
            ticksFromLastFire = firePeriod + 1;
            bulletWeight = 0.2f;
            this.ammo = ammo;
            r = new Random();
            
            fireSoundPath = @"event:/gunfire/SAIGAFA_FIRE";
        }

        public override void IncrementTick() => ticksFromLastFire++;
        
        public override List<Bullet> Fire(Vector gunPosition, Vector listenerPosition, CustomCursor cursor)
        {
            var spray = new List<Bullet>();
            var direction = (cursor.Position - gunPosition).Normalize();
                
            const int shotsAmount = 6;
            for (var i = 0; i < shotsAmount; i++)
            {
                var offset = new Vector(r.Next(-3, 3), r.Next(-3, 3)) / 30;
                var e = direction + offset;
                var position = gunPosition + e * 40;
                spray.Add(new Bullet(
                    position,
                    e * 35,
                    bulletWeight,
                    new Edge(gunPosition.Copy(), position),
                    10));
            }
            
            ammo--;
            ticksFromLastFire = 0;
            AudioEngine.PlayNewInstance(fireSoundPath, gunPosition, listenerPosition);
            cursor.MoveBy(new Vector(r.Next(-20, 20), r.Next(-20, 20)));

            return spray;
        }

        public override void AddAmmo(int amount)
        {
            if (amount > ammo) ammo = amount;
        }
        
        public override bool IsReady() => ticksFromLastFire >= firePeriod && ammo > 0;
    }
}