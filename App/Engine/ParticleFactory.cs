using System;
using System.Drawing;
using App.Engine.Physics;
using App.Engine.Sprites;
using App.Model.Entities;

namespace App.Engine
{
    public class ParticleFactory
    {
        private readonly Random r;
        
        private readonly Particle bloodSplashSmall;
        private readonly Particle bloodSplashMedium;
        private readonly Particle bloodSplashBig;

        public ParticleFactory()
        {
            r = new Random();
            
            bloodSplashSmall = new Particle(
                new Bitmap(@"Assets\Sprites\BLOOD\blood_splash_small.png"),
                1, 0, 5, new Size(64, 64));
            bloodSplashMedium = new Particle(
                new Bitmap(@"Assets\Sprites\BLOOD\blood_splash_medium.png"), 
                1, 0, 9, new Size(64, 64));
            bloodSplashBig = new Particle(
                new Bitmap(@"Assets\Sprites\BLOOD\blood_splash_big.png"),
                1, 0, 9, new Size(128, 128));
        }

        public ParticleUnit CreateSmallBloodSplash(Vector centerPosition)
        {
            return new ParticleUnit(bloodSplashSmall, centerPosition, r.Next(-45, 45));
        }
        
        public ParticleUnit CreateMediumBloodSplash(Vector centerPosition)
        {
            return new ParticleUnit(bloodSplashMedium, centerPosition, r.Next(-45, 45));
        }
        
        public ParticleUnit CreateBigBloodSplash(Vector centerPosition)
        {
            return new ParticleUnit(bloodSplashBig, centerPosition, r.Next(-45, 45));
        }
    }
}