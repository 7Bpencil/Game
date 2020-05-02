using System.Drawing;

namespace App.Engine.Particles
{
    public class AnimatedParticle : AbstractParticle
    {
        private readonly Bitmap bitmap;
        private readonly Size frameSize;
        private readonly int columns;
        private readonly Rectangle destRectInCamera;
        
        private readonly int startFrame;
        public readonly int FramesAmount;
        public readonly int FramePeriodInTicks;

        public AnimatedParticle(Bitmap bitmap, int framePeriodInTicks, int startFrame, int framesAmount, Size frameSize)
        {
            this.bitmap = bitmap;
            this.frameSize = frameSize;
            this.startFrame = startFrame;
            columns = bitmap.Width / frameSize.Width;
            
            FramesAmount = framesAmount;
            FramePeriodInTicks = framePeriodInTicks;
            
            destRectInCamera = new Rectangle(
                -frameSize.Width / 2,
                -frameSize.Height / 2,
                frameSize.Width, frameSize.Height);
        }
        
        public Rectangle GetFrame(int currentFrame)
        {
            var frame = startFrame + currentFrame;
            return new Rectangle
            {
                X = frame % columns * frameSize.Width,
                Y = frame / columns * frameSize.Height,
                Width = frameSize.Width,
                Height = frameSize.Height
            };
        }

        public override Bitmap Bitmap => bitmap;
        public override Rectangle DestRectInCamera => destRectInCamera;
        public override Rectangle CurrentFrame { get; }
    }
}