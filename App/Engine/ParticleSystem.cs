using App.Engine.Physics;
using App.Engine.Sprites;

namespace App.Engine
{
    public class ParticleSystem
    {
    }

    public class ParticleContainer
    {
        public ExpiringParticleSprite Content;
        public Vector CenterPosition;
        public float Angle;

        public ParticleContainer(ExpiringParticleSprite content, Vector centerPosition, float angle)
        {
            Content = content;
            CenterPosition = centerPosition;
            Angle = angle;
        }

        public void Reuse(Vector newCenterPosition, float newAngle)
        {
            Content.Reset();
            CenterPosition = newCenterPosition;
            Angle = newAngle;
        }
    }
}