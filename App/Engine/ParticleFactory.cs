using System;
using System.Drawing;
using App.Engine.Particles;
using App.Engine.Physics;
using App.Model.Entities;
using App.Model.Entities.Weapons;

namespace App.Engine
{
    public class ParticleFactory
    {
        private readonly Random r;
        
        private readonly AnimatedParticle bloodSplashSmall;
        private readonly AnimatedParticle bloodSplashMedium;
        private readonly AnimatedParticle bloodSplashBig;
        
        private readonly StaticParticle shellGauge12;
        private readonly StaticParticle shell762;
        private readonly StaticParticle shell919;

        public ParticleFactory()
        {
            r = new Random();
            
            bloodSplashSmall = new AnimatedParticle(
                new Bitmap(@"Assets\Sprites\BLOOD\blood_splash_small.png"),
                1, 0, 5, new Size(64, 64));
            bloodSplashMedium = new AnimatedParticle(
                new Bitmap(@"Assets\Sprites\BLOOD\blood_splash_medium.png"), 
                1, 0, 9, new Size(64, 64));
            bloodSplashBig = new AnimatedParticle(
                new Bitmap(@"Assets\Sprites\BLOOD\blood_splash_big.png"),
                1, 0, 9, new Size(128, 128));
            
            shellGauge12 = new StaticParticle(
                new Bitmap(@"Assets\Sprites\Weapons\gun_shells.png"),
                0, new Size(2, 10));
            shell762 = new StaticParticle(
                new Bitmap(@"Assets\Sprites\Weapons\gun_shells.png"),
                1, new Size(2, 10));
            shell919 = new StaticParticle(
                new Bitmap(@"Assets\Sprites\Weapons\gun_shells.png"),
                2, new Size(2, 10));
        }
        
        public AbstractParticleUnit CreateBloodSplash(Vector centerPosition)
        {
            var chance = r.Next(0, 10);
            if (chance > 8) return CreateBigBloodSplash(centerPosition);
            if (chance > 5) return CreateMediumBloodSplash(centerPosition);
            return CreateSmallBloodSplash(centerPosition);
        }
        
        public AbstractParticleUnit CreateShell(Vector startPosition, Vector direction, Weapon weapon)
        {
            var weaponType = weapon.GetType();
            if (weaponType == typeof(AK303)) return Create762Shell(startPosition, direction);
            if (weaponType == typeof(Shotgun)) return CreateGauge12Shell(startPosition, direction);
            if (weaponType == typeof(SaigaFA)) return CreateGauge12Shell(startPosition, direction);
            if (weaponType == typeof(MP6)) return Create919Shell(startPosition, direction);
            return null;
        }

        private AbstractParticleUnit CreateSmallBloodSplash(Vector centerPosition)
        {
            return new BloodSplashParticleUnit(bloodSplashSmall, centerPosition, r.Next(-45, 45), r.Next(0, 10));
        }
        
        private AbstractParticleUnit CreateMediumBloodSplash(Vector centerPosition)
        {
            return new BloodSplashParticleUnit(bloodSplashMedium, centerPosition, r.Next(-45, 45), r.Next(0, 10));
        }
        
        private AbstractParticleUnit CreateBigBloodSplash(Vector centerPosition)
        {
            return new BloodSplashParticleUnit(bloodSplashBig, centerPosition, r.Next(-45, 45), r.Next(0, 9));
        }

        private AbstractParticleUnit CreateGauge12Shell(Vector startPosition, Vector direction)
        {
            return new GunShellParticleUnit(shellGauge12, startPosition, direction, r.Next(-45, 45));
        }
        
        private  AbstractParticleUnit Create762Shell(Vector startPosition, Vector direction)
        {
            return new GunShellParticleUnit(shell762, startPosition, direction, r.Next(-45, 45));
        }
        
        private  AbstractParticleUnit Create919Shell(Vector startPosition, Vector direction)
        {
            return new GunShellParticleUnit(shell919, startPosition, direction, r.Next(-45, 45));
        }
    }
}