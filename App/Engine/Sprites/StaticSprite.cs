using System.Drawing;

namespace App.Engine.Sprites
{
    public class StaticSprite : Sprite
    {
        private readonly Rectangle frameTile;
        public override Rectangle GetCurrentFrameTile() => frameTile;
        
        public StaticSprite(Bitmap bitmap, int frameID, Size size, int columns)
            : base(bitmap, 0, frameID, frameID, size, columns)
        {
            frameTile = new Rectangle
            {
                X = currentFrame % columns * size.Width,
                Y = currentFrame / columns * size.Height,
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