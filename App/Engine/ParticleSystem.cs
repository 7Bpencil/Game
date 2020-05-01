using System.Collections.Generic;
using App.Engine.Physics;

namespace App.Engine
{
    public class ParticleSystem
    {
    }

    public class ParticleContainer
    {
        private Sprite particleSprite;
        private readonly List<ParticleUnit> particleUnits;

        public ParticleContainer(Sprite particleSprite, int startCapacity)
        {
            this.particleSprite = particleSprite;
            particleUnits = new List<ParticleUnit> {Capacity = startCapacity};
        }

        public void AddUnit(ParticleUnit newUnit) => particleUnits.Add(newUnit);
    }

    public struct ParticleUnit
    {
        public readonly Vector particlePosition;
        public readonly float particleAngle;
        public bool IsExpired;

        public ParticleUnit(Vector particlePosition, float particleAngle)
        {
            this.particlePosition = particlePosition;
            this.particleAngle = particleAngle;
            IsExpired = false;
        }
    }
}