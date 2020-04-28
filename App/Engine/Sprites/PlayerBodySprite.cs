using System.Drawing;
using App.Engine.Physics;

namespace App.Engine.Sprites
{
    public class PlayerBodySprite : Sprite
    {
        private Vector previousCenterPosition;
        
        public PlayerBodySprite(
            Vector center, Bitmap bitmap, int framePeriod, int startFrame,
            int endFrame, Size size, int columns)
            : base(center, bitmap, framePeriod, startFrame, endFrame, size, columns)
        {
            previousCenterPosition = center.Copy();
        }

        /// <summary>
        /// This method will reset animation if player doesn't move
        /// </summary>
        public override void UpdateFrame()
        {
            ticksFromLastFrame++;
            if (ticksFromLastFrame > framePeriod)
            {
                ticksFromLastFrame = 0;
                currentFrame++;
                if (currentFrame > endFrame || center.Equals(previousCenterPosition))
                    currentFrame = startFrame;
            }
            previousCenterPosition = center.Copy();
        }
    }
}