using System.Drawing;
using App.Engine.Physics;
using App.Engine.Sprites;

namespace App.Model.Entities
{
    public class ParticleUnit
    {
        public readonly Particle Content;
        public Vector CenterPosition;
        public float Angle;

        private readonly int framesAmount;
        private readonly int framePeriodInTicks;
        private int currentFrame;
        private int ticksFromLastFrame;
        public bool IsExpired;

        public ParticleUnit(Particle content, Vector centerPosition, float angle)
        {
            Content = content;
            CenterPosition = centerPosition;
            Angle = angle;
            IsExpired = false;

            framesAmount = content.FramesAmount;
            framePeriodInTicks = content.FramePeriodInTicks;
            currentFrame = 0;
            ticksFromLastFrame = 0;
        }

        public void UpdateFrame()
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

        public Rectangle CurrentFrame => Content.GetFrame(currentFrame);
    }
}