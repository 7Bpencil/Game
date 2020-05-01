using System.Drawing;
using App.Engine.Physics;

namespace App.Engine.Sprites
{
    public class ExpiringParticleSprite : Sprite
    {
        public bool IsExpired;
        public ExpiringParticleSprite(Bitmap bitmap, int framePeriodInTicks, int startFrame, int endFrame, Size size, int columns)
            : base(bitmap, framePeriodInTicks, startFrame, endFrame, size, columns)
        {
            IsExpired = false;
        }
        
        public override void UpdateFrame()
        {
            if (IsExpired) return;
            TicksFromLastFrame++;
            if (TicksFromLastFrame > FramePeriodInTicks) IsExpired = true;
        }

        public void Reset()
        {
            IsExpired = false;
            CurrentFrame = StartFrame;
            TicksFromLastFrame = 0;
        }
    }
}