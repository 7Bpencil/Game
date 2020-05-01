using System.Drawing;

namespace App.Engine
{
    public class Sprite
    {
        protected readonly int StartFrame;
        protected int CurrentFrame;
        protected readonly int EndFrame;

        public readonly Rectangle DestRectInCamera;

        public readonly Size Size;
        public readonly Bitmap Bitmap;
        protected readonly int Columns;
        
        protected readonly int FramePeriodInTicks;
        protected int TicksFromLastFrame;

        public virtual Rectangle GetCurrentFrame()
        {
            return new Rectangle
            {
                X = CurrentFrame % Columns * Size.Width,
                Y = CurrentFrame / Columns * Size.Height,
                Width = Size.Width,
                Height = Size.Height
            };
        }

        public Sprite(Bitmap bitmap, int framePeriodInTicks, int startFrame, int endFrame, Size size)
        {
            Size = size;
            Bitmap = bitmap;
            Columns = bitmap.Width / size.Width;
            DestRectInCamera = new Rectangle(-size.Width / 2, -size.Height / 2, size.Width, size.Height);
            
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