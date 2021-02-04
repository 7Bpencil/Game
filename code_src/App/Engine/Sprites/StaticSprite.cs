using System.Drawing;

namespace App.Engine.Sprites
{
    public class StaticSprite : Sprite
    {
        private readonly Rectangle frameTile;
        public override Rectangle GetCurrentFrame() => frameTile;

        public StaticSprite(Bitmap bitmap, int frameID, Size size)
            : base(bitmap, 0, frameID, frameID, size)
        {
            frameTile = new Rectangle
            {
                X = CurrentFrame % Columns * size.Width,
                Y = CurrentFrame / Columns * size.Height,
                Width = size.Width,
                Height = size.Height
            };
        }

        public StaticSprite(Bitmap bitmap, int frameID, Size size, float destWidth, float destHeight)
            : base(bitmap, 0, frameID, frameID, size, destWidth, destHeight)
        {
            frameTile = new Rectangle
            {
                X = CurrentFrame % Columns * size.Width,
                Y = CurrentFrame / Columns * size.Height,
                Width = size.Width,
                Height = size.Height
            };
        }

        /// <summary>
        /// This method does nothing (because it's static, LOL)
        /// </summary>
        public override void UpdateFrame()
        {
        }
    }
}
