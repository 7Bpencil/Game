using System;
using System.Collections.Generic;
using System.Drawing;
using App.Engine;
using App.Engine.Physics;

namespace App.Model.Entities.Weapons
{
    public class MP6 : Weapon
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

        public MP6(int ammo)
        {
            name = "MP6";
            capacity = 40;
            firePeriod = 3;
            ticksFromLastFire = 0;
            bulletWeight = 0.7f;
            this.ammo = ammo;
            r = new Random();
            
            HUDicon = new Bitmap(@"Assets\TileMaps\Weapons\MP6_hud.png");
            CollectableIcon = new Bitmap(@"Assets\TileMaps\Weapons\MP6_icon.png");
            TopDownView = new Sprite(
                Vector.ZeroVector, new Bitmap( @"Assets\TileMaps\Weapons\top_down_view.png"),
                0, 2, 2, new Size(85, 64), 1);
            
            //fireSound = new SoundPlayer {SoundLocation = @"Assets\Sounds\GunSounds\fire_MP6.wav"};
            //fireSound.Load();
        }

        public override void IncrementTick() => ticksFromLastFire++;
        
        public override List<Bullet> Fire(Vector playerPosition, CustomCursor cursor)
        {
            if (ticksFromLastFire < firePeriod 
                || ammo == 0) return null;
            
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
            //fireSound.Play();
            cursor.MoveBy(new Vector(r.Next(-10, 10), r.Next(-10, 10)));

            return spray;
        }
        
        public override void AddAmmo(int amount)
        {
            if (amount > ammo) ammo = amount;
        }
    }
}