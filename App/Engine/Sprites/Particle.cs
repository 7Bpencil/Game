using System.Drawing;

namespace App.Engine.Sprites
{
    public class Particle
    {
        private readonly int startFrame;
        private readonly int endFrame;
        public readonly int FramePeriodInTicks;
        public int FramesAmount => endFrame - startFrame;
        
        public readonly Bitmap Bitmap;
        private readonly Size size;
        public readonly Rectangle DestRectInCamera;
        private readonly int columns;

        public Particle(Bitmap bitmap, int framePeriodInTicks, int startFrame, int endFrame, Size size)
        {
            this.size = size;
            Bitmap = bitmap;
            columns = bitmap.Width / size.Width;
            DestRectInCamera = new Rectangle(-size.Width / 2, -size.Height / 2, size.Width, size.Height);
            
            this.startFrame = startFrame;
            this.endFrame = endFrame;
            
            FramePeriodInTicks = framePeriodInTicks;
        }

        public virtual Rectangle GetFrame(int currentFrame)
        {
            var frame = startFrame + currentFrame;
            return new Rectangle
            {
                X = frame % columns * size.Width,
                Y = frame / columns * size.Height,
                Width = size.Width,
                Height = size.Height
            };
        }
    }
}