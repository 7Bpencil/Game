using System.Drawing;

namespace App.Engine
{
    public abstract class AbstractParticle
    {
        public abstract Bitmap Bitmap { get; }
        public abstract Rectangle DestRectInCamera { get; }
        public abstract Rectangle CurrentFrame { get; }
    }
}