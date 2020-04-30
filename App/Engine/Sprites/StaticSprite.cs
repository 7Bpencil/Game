using System.Drawing;
using App.Engine.Physics;

namespace App.Engine.Sprites
{
    public class StaticSprite : Sprite
    {
        private readonly Rectangle frameTile;
        protected override Rectangle GetCurrentFrameTile() => frameTile;
        
        public StaticSprite(Vector center, Bitmap bitmap, float angle, int frameID, Size size, int columns)
            : base(center, bitmap, angle, 0, frameID, frameID, size, columns)
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