using System.Drawing;

namespace App.Engine.Sprites
{
    public class MeleeWeaponSprite : Sprite
    {
        public bool InAction;
        public bool WasRaised => CurrentFrame == StartFrame + 1 && TicksFromLastFrame == 0;

        public MeleeWeaponSprite(Bitmap bitmap, int framePeriodInTicks, int startFrame, int endFrame, Size size)
            : base(bitmap, framePeriodInTicks, startFrame, endFrame, size)
        {
        }

        public MeleeWeaponSprite(
            Bitmap bitmap, int framePeriodInTicks, int startFrame, int endFrame, Size size, float destWidth, float destHeight)
            : base(bitmap, framePeriodInTicks, startFrame, endFrame, size, destWidth, destHeight)
        {
        }

        public override void UpdateFrame()
        {
            if (!InAction) return;
            TicksFromLastFrame++;
            if (TicksFromLastFrame > FramePeriodInTicks)
            {
                TicksFromLastFrame = 0;
                CurrentFrame++;
                if (CurrentFrame > EndFrame)
                {
                    CurrentFrame = StartFrame;
                    InAction = false;
                }
            }
        }
    }
}
