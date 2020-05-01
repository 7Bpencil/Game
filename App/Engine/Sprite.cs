using System.Drawing;

namespace App.Engine
{
    public class Sprite
    {
        protected int startFrame;
        protected int currentFrame;
        protected int endFrame;

        public readonly Rectangle DestRectInCamera;

        public readonly Size Size;
        public readonly Bitmap Bitmap;
        private readonly int columns;
        
        protected int framePeriod;
        protected int ticksFromLastFrame;

        public virtual Rectangle GetCurrentFrameTile()
        {
            return new Rectangle
            {
                X = currentFrame % columns * Size.Width,
                Y = currentFrame / columns * Size.Height,
                Width = Size.Width,
                Height = Size.Height
            };
        }

        public Sprite(Bitmap bitmap, int framePeriod, int startFrame, int endFrame, Size size, int columns)
        {
            Size = size;
            Bitmap = bitmap;
            this.columns = columns;
            DestRectInCamera = new Rectangle(-size.Width / 2, -size.Height / 2, size.Width, size.Height);
            
            this.startFrame = startFrame;
            currentFrame = startFrame;
            this.endFrame = endFrame;
            
            this.framePeriod = framePeriod;
            ticksFromLastFrame = 0;
        }

        /// <summary>
        /// Default method that loops through frames
        /// </summary>
        public virtual void UpdateFrame()
        {
            ticksFromLastFrame++;
            if (ticksFromLastFrame > framePeriod)
            {
                ticksFromLastFrame = 0;
                currentFrame++;
                if (currentFrame > endFrame) currentFrame = startFrame;
            }
        }
    }
}