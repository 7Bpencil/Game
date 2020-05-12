using System.Drawing;
using App.Engine.Particles;
using App.Engine.Physics;

namespace App.Engine.ParticleUnits
{
    public class GunShellParticleUnit : AbstractParticleUnit
    {
        private StaticParticle content;
        private readonly Vector shellDirectionVector;
        private Vector position;
        private int frame;
        
        public override AbstractParticle Content => content;
        public override Rectangle CurrentFrame => content.CurrentFrame;
        public override Vector CenterPosition => position;
        public override float Angle { get; }

        public override bool IsExpired { get; set; }
        public override bool ShouldBeBurned { get; set; }

        public GunShellParticleUnit(StaticParticle content, Vector startPosition, Vector directionVector, float angle)
        {
            this.content = content;
            position = startPosition;
            shellDirectionVector = directionVector.GetNormal().Normalize() * 5 + new Vector(angle, -angle) / 10;
            Angle = angle;
        }

        public override void UpdateFrame()
        {
            if (IsExpired) return;
            position += shellDirectionVector;
            frame++;
            if (frame > 10)
                ShouldBeBurned = true;
        }
        
        public override void ClearContent()
        {
            content = null;
        }
    }
}