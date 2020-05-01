using System.Drawing;
using App.Engine.Physics;

namespace App.Engine.Sprites
{
    public class PlayerBodySprite : Sprite
    {
        private Vector previousCenterPosition;
        private readonly Vector centerPosition;
        
        public PlayerBodySprite(
            Vector centerPosition, Bitmap bitmap, int framePeriod, int startFrame,
            int endFrame, Size size, int columns)
            : base(bitmap, framePeriod, startFrame, endFrame, size, columns)
        {
            this.centerPosition = centerPosition;
            previousCenterPosition = centerPosition.Copy();
        }

        /// <summary>
        /// Method will reset animation if player doesn't move
        /// </summary>
        public override void UpdateFrame()
        {
            ticksFromLastFrame++;
            if (ticksFromLastFrame > framePeriod)
            {
                ticksFromLastFrame = 0;
                currentFrame++;
                if (currentFrame > endFrame || centerPosition.Equals(previousCenterPosition))
                    currentFrame = startFrame;
            }
            previousCenterPosition = centerPosition.Copy();
        }
    }
}