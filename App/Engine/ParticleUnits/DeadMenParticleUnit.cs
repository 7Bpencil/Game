using System.Drawing;
using App.Engine.Particles;
using App.Engine.Physics;

namespace App.Engine.ParticleUnits
{
    public class DeadMenParticleUnit : AbstractParticleUnit
    {
        private StaticParticle content;
        public override AbstractParticle Content => content;
        public override Rectangle CurrentFrame => Content.CurrentFrame;
        public override Vector CenterPosition { get; }
        public override float Angle { get; }
        public override bool IsExpired { get; set; }
        public override bool ShouldBeBurned { get; set; }

        public DeadMenParticleUnit(StaticParticle content, Vector startPosition, float angle)
        {
            this.content = content;
            CenterPosition = startPosition;
            Angle = angle;
        }

        public override void UpdateFrame()
        {
            ShouldBeBurned = true;
        }
        
        public override void ClearContent()
        {
            content = null;
        }
    }
}