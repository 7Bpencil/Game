using System.Drawing;
using App.Engine.Physics;

namespace App.Engine.Particles
{
    public class BloodSplashParticleUnit : AbstractParticleUnit
    {
        private readonly AnimatedParticle content;
        private int currentFrame;
        private readonly int frameToBurn;
        private int ticksFromLastFrame;
        private readonly int framePeriodInTicks;
        private readonly int framesAmount;


        public override AbstractParticle Content => content;
        public override Rectangle CurrentFrame => content.GetFrame(currentFrame);
        public override Vector CenterPosition { get; }
        public override float Angle { get; }
        public override bool IsExpired { get; set; }
        public override bool ShouldBeBurned { set; get; }

        public BloodSplashParticleUnit(AnimatedParticle content, Vector position, float angle, int frameToBurn)
        {
            this.content = content;
            CenterPosition = position;
            Angle = angle;
            
            framesAmount = content.FramesAmount;
            framePeriodInTicks = content.FramePeriodInTicks;
            currentFrame = 0;
            ticksFromLastFrame = 0;
            this.frameToBurn = frameToBurn;
        }

        public override void UpdateFrame()
        {
            if (IsExpired) return;
            ticksFromLastFrame++;
            if (ticksFromLastFrame > framePeriodInTicks)
            {
                ticksFromLastFrame = 0;
                currentFrame++;
                if (currentFrame == frameToBurn) ShouldBeBurned = true;
                if (currentFrame > framesAmount) IsExpired = true;
            }
        }
    }
}