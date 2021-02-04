using System.Drawing;

namespace App.Engine.Particles
{
    public class StaticParticle : AbstractParticle
    {
        private readonly Rectangle frame;
        private readonly Bitmap bitmap;
        private readonly Rectangle destRectInCamera;
        
        public override Bitmap Bitmap => bitmap;
        public override Rectangle DestRectInCamera => destRectInCamera;
        public override Rectangle CurrentFrame => frame;

        public StaticParticle(Bitmap bitmap, int frameID, Size frameSize)
        {
            this.bitmap = bitmap;
            destRectInCamera = new Rectangle(
                -frameSize.Width / 2,
                -frameSize.Height / 2,
                frameSize.Width, frameSize.Height);
            
            var columns = bitmap.Width / frameSize.Width;
            frame = new Rectangle(
                frameID % columns * frameSize.Width,
                frameID / columns * frameSize.Height,
                frameSize.Width, frameSize.Height);
        }
    }
}