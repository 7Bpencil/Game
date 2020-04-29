using System;
using System.Collections.Generic;
using System.Drawing;
using App.Engine;
using App.Engine.Physics;

namespace App.Model.Entities.Weapons
{
    public class SaigaFA : Weapon
    {
        private readonly Random r;
        //private readonly SoundPlayer fireSound;

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

        public override Bitmap HUDicon { get; }
        public override Bitmap CollectableIcon { get; }
        public override Sprite TopDownView { get; }

        public SaigaFA(int ammo)
        {
            name = "Saiga Full-Auto";
            capacity = 20;
            firePeriod = 8;
            ticksFromLastFire = 0;
            bulletWeight = 0.2f;
            this.ammo = ammo;
            r = new Random();
            
            HUDicon = new Bitmap(@"Assets\TileMaps\Weapons\SaigaFA_hud.png");
            CollectableIcon = new Bitmap(@"Assets\TileMaps\Weapons\SaigaFA_icon.png");
            TopDownView = new Sprite(
                Vector.ZeroVector, new Bitmap( @"Assets\TileMaps\Weapons\top_down_view.png"),
                3, 3, 3, new Size(85, 64), 1);
            
            //fireSound = new SoundPlayer {SoundLocation = @"Assets\Sounds\GunSounds\fire_SaigaFA.wav"};
            //fireSound.Load();
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
                    e * 35,
                    bulletWeight,
                    new Edge(playerPosition.Copy(), position),
                    10));
            }
            
            ammo--;
            ticksFromLastFire = 0;
            //fireSound.Play();
            cursor.MoveBy(new Vector(r.Next(-20, 20), r.Next(-20, 20)));

            return spray;
        }

        public override void AddAmmo(int amount)
        {
            if (amount > ammo) ammo = amount;
        }
    }
}