using System.Drawing;
using App.Engine.Particles;
using App.Engine.Physics;

namespace App.Engine.ParticleUnits
{
    public class ExpiringAnimatedParticleUnit : AbstractParticleUnit
    {
        private AnimatedParticle content;
        private int currentFrame;
        private int ticksFromLastFrame;
        private readonly int framePeriodInTicks;
        private readonly int framesAmount;

        public override AbstractParticle Content => content;
        public override Rectangle CurrentFrame => content.GetFrame(currentFrame);
        public override Vector CenterPosition { get; }
        public override float Angle { get; }
        public override bool IsExpired { get; set; }
        public override bool ShouldBeBurned { set; get; }

        public ExpiringAnimatedParticleUnit(AnimatedParticle content, Vector position, float angle)
        {
            this.content = content;
            CenterPosition = position;
            Angle = angle;
            
            framesAmount = content.FramesAmount;
            framePeriodInTicks = content.FramePeriodInTicks;
            currentFrame = 0;
            ticksFromLastFrame = 0;
        }

        public override void UpdateFrame()
        {
            if (IsExpired) return;
            ticksFromLastFrame++;
            if (ticksFromLastFrame > framePeriodInTicks)
            {
                ticksFromLastFrame = 0;
                currentFrame++;
                if (currentFrame > framesAmount) IsExpired = true;
            }
        }
        
        public override void ClearContent()
        {
            content = null;
        }
    }
}