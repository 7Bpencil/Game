using System.Drawing;
using App.Engine.Physics;

namespace App.Engine.Sprites
{
    public class StaticSprite : Sprite
    {
        private readonly Rectangle frameTile;
        protected override Rectangle GetCurrentFrameTile() => frameTile;
        
        public StaticSprite(Vector center, Bitmap bitmap, int framePeriod, int startFrame, int endFrame, Size size, int columns) : base(center, bitmap, framePeriod, startFrame, endFrame, size, columns)
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
        /// Does nothing (because it's static, LOL)
        /// </summary>
        public override void UpdateFrame()
        {
        }
    }
}