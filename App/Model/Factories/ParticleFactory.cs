using System;
using System.Collections.Generic;
using System.Drawing;
using App.Engine;
using App.Engine.Particles;
using App.Engine.ParticleUnits;
using App.Engine.Physics;
using App.Model.Entities;
using App.Model.Entities.Weapons;

namespace App.Model.Factories
{
    public static class ParticleFactory
    {
        private static Random r;
        
        private static AnimatedParticle bloodSplashSmall;
        private static AnimatedParticle bloodSplashMedium;
        private static AnimatedParticle bloodSplashBig;

        private static AnimatedParticle explosion;
        private static StaticParticle explosion_funnel;
        
        private static AnimatedParticle wallDust;
        
        private static StaticParticle shellGauge12;
        private static StaticParticle shell762;
        private static StaticParticle shell919;
        private static StaticParticle shellGrenade;

        private static StaticParticle grenadeWarhead;

        private static StaticParticle exit;

        public static void Initialize()
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
                0, new Size(4, 20));
            shell762 = new StaticParticle(
                new Bitmap(@"Assets\Sprites\Weapons\gun_shells.png"),
                1, new Size(4, 20));
            shell919 = new StaticParticle(
                new Bitmap(@"Assets\Sprites\Weapons\gun_shells.png"),
                2, new Size(4, 20));
            shellGrenade = new StaticParticle(
                new Bitmap(@"Assets\Sprites\Weapons\grenade_shell.png"), 
                0, new Size(15, 18));
            
            
            wallDust = new AnimatedParticle(
                new Bitmap(@"Assets\Sprites\SMOKE\bullet_smoke.png"),
                1, 0, 5, new Size(13, 25));
            
            grenadeWarhead = new StaticParticle(
                new Bitmap(@"Assets\Sprites\Weapons\grenade_warhead.png"), 
                0, new Size(15, 26));
            
            exit = new StaticParticle(
                new Bitmap(@"Assets\Sprites\exit.png"), 0, new Size(96, 96));
            
            explosion = new AnimatedParticle(
                new Bitmap(@"Assets\Sprites\EXPLOSIONS\explosion.png"), 
                1, 0, 12, new Size(170, 128));
            explosion_funnel = new StaticParticle(
                new Bitmap(@"Assets\Sprites\EXPLOSIONS\explosion_funnel.png"), 
                0, new Size(64, 64));
        }
        
        public static AbstractParticleUnit CreateBloodSplash(Vector centerPosition)
        {
            var chance = r.Next(0, 10);
            if (chance > 8) return CreateBigBloodSplash(centerPosition);
            if (chance > 5) return CreateMediumBloodSplash(centerPosition);
            return CreateSmallBloodSplash(centerPosition);
        }

        public static AbstractParticleUnit CreateExplosion(Vector position)
        {
            return new ExpiringAnimatedParticleUnit(explosion, position, r.Next(-45, 45));
        }
        
        public static AbstractParticleUnit CreateExplosionFunnel(Vector position)
        {
            return new AutoBurnParticleUnit(explosion_funnel, position, r.Next(-45, 45));
        }

        public static AbstractParticleUnit CreateShell(Vector startPosition, Vector direction, Weapon weapon)
        {
            var weaponType = weapon.GetType();
            if (weaponType == typeof(AK303)) return Create762Shell(startPosition, direction);
            if (weaponType == typeof(Shotgun)) return CreateGauge12Shell(startPosition, direction);
            if (weaponType == typeof(SaigaFA)) return CreateGauge12Shell(startPosition, direction);
            if (weaponType == typeof(MP6)) return Create919Shell(startPosition, direction);
            if (weaponType == typeof(GrenadeLauncher)) return CreateGrenadeShell(startPosition, direction);
            return null;
        }

        public static AbstractParticleUnit CreateWallDust(Vector penetrationPosition, Vector direction)
        {
            var angle = 180 / Math.PI * direction.Angle + 90;
            return new ExpiringAnimatedParticleUnit(wallDust, penetrationPosition, (float) angle);
        }

        public static AbstractParticleUnit CreateDeadMenBody(StaticParticle body, Vector position, Vector bodyDirection)
        {
            var angle = Vector.GetAngle(bodyDirection, new Vector(1, 0));
            return new AutoBurnParticleUnit(body, position, angle);
        }

        public static AbstractParticleUnit CreateGrenadeWarhead(Vector startPosition, Vector direction)
        {
            var angle = 180 / Math.PI * direction.Angle - 90;
            return new ExpiringStaticParticleUnit(grenadeWarhead, startPosition, (float) angle);
        }

        public static AbstractParticleUnit CreateExit(Vector exitCenter)
        {
            return new AutoBurnParticleUnit(exit, exitCenter, 0);
        }

        public static AbstractParticleUnit CreateSmallBloodSplash(Vector centerPosition)
        {
            return new BloodSplashParticleUnit(bloodSplashSmall, centerPosition, r.Next(-45, 45), r.Next(0, 6));
        }
        
        private static AbstractParticleUnit CreateMediumBloodSplash(Vector centerPosition)
        {
            return new BloodSplashParticleUnit(bloodSplashMedium, centerPosition, r.Next(-45, 45), r.Next(0, 10));
        }
        
        public static AbstractParticleUnit CreateBigBloodSplash(Vector centerPosition)
        {
            return new BloodSplashParticleUnit(bloodSplashBig, centerPosition, r.Next(-45, 45), r.Next(0, 10));
        }

        private static AbstractParticleUnit CreateGauge12Shell(Vector startPosition, Vector direction)
        {
            return new GunShellParticleUnit(shellGauge12, startPosition, direction, r.Next(-45, 45));
        }
        
        private static AbstractParticleUnit Create762Shell(Vector startPosition, Vector direction)
        {
            return new GunShellParticleUnit(shell762, startPosition, direction, r.Next(-45, 45));
        }
        
        private static AbstractParticleUnit Create919Shell(Vector startPosition, Vector direction)
        {
            return new GunShellParticleUnit(shell919, startPosition, direction, r.Next(-45, 45));
        }
        
        private static AbstractParticleUnit CreateGrenadeShell(Vector startPosition, Vector direction)
        {
            return new GunShellParticleUnit(shellGrenade, startPosition, direction, r.Next(-45, 45));
        }
    }
}