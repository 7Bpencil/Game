using System.Drawing;
using App.Engine.Particles;
using App.Engine.Physics;

namespace App.Engine.ParticleUnits
{
    public class ExpiringStaticParticleUnit : AbstractParticleUnit
    {
        private StaticParticle content;
        public override AbstractParticle Content => content;
        public override Rectangle CurrentFrame => content.CurrentFrame;
        public override Vector CenterPosition { get; }
        public override float Angle { get; }
        public override bool IsExpired { get; set; }
        public override bool ShouldBeBurned { set; get; }

        public ExpiringStaticParticleUnit(StaticParticle content, Vector position, float angle)
        {
            this.content = content;
            CenterPosition = position;
            Angle = angle;
        }

        public override void UpdateFrame()
        {
        }
        
        public override void ClearContent()
        {
            content = null;
        }
    }
}