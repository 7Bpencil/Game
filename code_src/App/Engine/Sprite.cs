using System.Drawing;

namespace App.Engine
{
    public class Sprite
    {
        protected readonly int StartFrame;
        protected int CurrentFrame;
        protected readonly int EndFrame;

        public readonly RectangleF DestRectInCamera;

        private readonly Size size;
        public readonly Bitmap Bitmap;
        protected readonly int Columns;

        protected readonly int FramePeriodInTicks;
        protected int TicksFromLastFrame;

        public virtual Rectangle GetCurrentFrame()
        {
            return new Rectangle
            {
                X = CurrentFrame % Columns * size.Width,
                Y = CurrentFrame / Columns * size.Height,
                Width = size.Width,
                Height = size.Height
            };
        }

        public Sprite(Bitmap bitmap, int framePeriodInTicks, int startFrame, int endFrame, Size size)
        {
            this.size = size;
            Bitmap = bitmap;
            Columns = bitmap.Width / size.Width;
            DestRectInCamera = new RectangleF(-size.Width / 2, -size.Height / 2, size.Width, size.Height);

            StartFrame = startFrame;
            CurrentFrame = startFrame;
            EndFrame = endFrame;

            FramePeriodInTicks = framePeriodInTicks;
            TicksFromLastFrame = 0;
        }

        public Sprite(Bitmap bitmap, int framePeriodInTicks, int startFrame, int endFrame, Size size, float destWidth, float destHeight)
        {
            this.size = size;
            Bitmap = bitmap;
            Columns = bitmap.Width / size.Width;
            DestRectInCamera = new RectangleF(-destWidth / 2, -destHeight / 2, destWidth, destHeight);

            StartFrame = startFrame;
            CurrentFrame = startFrame;
            EndFrame = endFrame;

            FramePeriodInTicks = framePeriodInTicks;
            TicksFromLastFrame = 0;
        }

        /// <summary>
        /// Default method that loops through frames
        /// </summary>
        public virtual void UpdateFrame()
        {
            TicksFromLastFrame++;
            if (TicksFromLastFrame > FramePeriodInTicks)
            {
                TicksFromLastFrame = 0;
                CurrentFrame++;
                if (CurrentFrame > EndFrame) CurrentFrame = StartFrame;
            }
        }
    }
}
