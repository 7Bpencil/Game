using System.Drawing;

namespace App.Engine.Sprites
{
    public class MeleeWeaponSprite : Sprite
    {
        public bool inAction;
        public MeleeWeaponSprite(Bitmap bitmap, int framePeriodInTicks, int startFrame, int endFrame, Size size)
            : base(bitmap, framePeriodInTicks, startFrame, endFrame, size)
        {
        }

        public override void UpdateFrame()
        {
            if (!inAction) return;
            TicksFromLastFrame++;
            if (TicksFromLastFrame > FramePeriodInTicks)
            {
                TicksFromLastFrame = 0;
                CurrentFrame++;
                if (CurrentFrame > EndFrame)
                {
                    CurrentFrame = StartFrame;
                    inAction = false;
                }
            }
        }
    }
}