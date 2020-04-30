using System;
using System.Drawing;

namespace App.Engine
{
    public class BloodSystem
    {
        private readonly Random r;
        private readonly Bitmap splash1;
        private readonly Bitmap splash3;

        public BloodSystem()
        {
            r = new Random();
            splash1 = new Bitmap(@"Assets\TileMaps\BLOOD\blood_splash1.png");
            splash3 = new Bitmap(@"Assets\TileMaps\BLOOD\blood_splash3.png");
        }
    }
}