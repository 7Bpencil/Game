using System.Drawing;
using App.Engine.Physics;

namespace App.Engine.Sprites
{
    public class PlayerBodySprite : Sprite
    {
        private Vector previousCenterPosition;
        private readonly Vector centerPosition;

        public PlayerBodySprite(
            Vector centerPosition, Bitmap bitmap, int framePeriodInTicks, int startFrame, int endFrame, Size size)
            : base(bitmap, framePeriodInTicks, startFrame, endFrame, size)
        {
            this.centerPosition = centerPosition;
            previousCenterPosition = centerPosition.Copy();
        }

        public PlayerBodySprite(
            Vector centerPosition, Bitmap bitmap, int framePeriodInTicks, int startFrame,
            int endFrame, Size size, float destWidth, float destHeight)
            : base(bitmap, framePeriodInTicks, startFrame, endFrame, size, destWidth, destHeight)
        {
            this.centerPosition = centerPosition;
            previousCenterPosition = centerPosition.Copy();
        }

        /// <summary>
        /// Method will reset animation if player doesn't move
        /// </summary>
        public override void UpdateFrame()
        {
            TicksFromLastFrame++;
            if (TicksFromLastFrame > FramePeriodInTicks)
            {
                TicksFromLastFrame = 0;
                CurrentFrame++;
                if (CurrentFrame > EndFrame || centerPosition.Equals(previousCenterPosition))
                    CurrentFrame = StartFrame;
            }
            previousCenterPosition = centerPosition.Copy();
        }
    }
}
