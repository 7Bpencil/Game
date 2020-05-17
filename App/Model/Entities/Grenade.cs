using System.Collections.Generic;
using App.Engine;
using App.Engine.Physics;
using App.Model.Factories;

namespace App.Model.Entities
{
    public class Grenade : AbstractProjectile
    {
        private readonly Vector expectedDetonationPlace;
        public readonly int DamageRadius;
        private readonly AbstractParticleUnit grenadeWarheadParticleUnit;
        private int ticksBeforeDetonation;
        

        public Grenade(
            Vector position, Vector startVelocity, int damage, Vector expectedDetonationPlace, int damageRadius, int ticksBeforeDetonation) : 
            base(position, startVelocity, damage)
        {
            this.expectedDetonationPlace = expectedDetonationPlace;
            StaticPenetrations = new List<float[]> {Capacity = 4};
            DamageRadius = damageRadius;
            grenadeWarheadParticleUnit = ParticleFactory.CreateGrenadeWarhead(position, startVelocity.Normalize());
            this.ticksBeforeDetonation = ticksBeforeDetonation;
        }

        public override void Update()
        {
            var canMove = true;
            foreach (var distanceBeforeCollision in StaticPenetrations)
            {
                distanceBeforeCollision[0] -= Speed; distanceBeforeCollision[1] -= Speed;
                var v = Position - expectedDetonationPlace;
                if (Vector.ScalarProduct(v, v) <= 32 * 32)
                {
                    canMove = false;
                    ticksBeforeDetonation--;
                    if (ticksBeforeDetonation == 0)
                    {
                        ClosestPenetrationPoint = expectedDetonationPlace;
                        grenadeWarheadParticleUnit.IsExpired = true;
                    }
                }
                else if (distanceBeforeCollision[0] <= 0)
                {
                    canMove = false;
                    ticksBeforeDetonation--;
                    if (ticksBeforeDetonation == 0)
                    {
                        ClosestPenetrationPoint = Position + Velocity.Normalize() * distanceBeforeCollision[1];
                        grenadeWarheadParticleUnit.IsExpired = true;
                    }
                }
            }
            
            if (StaticPenetrations[StaticPenetrations.Count - 1][1] < -500) IsStuck = true;
            if (canMove) Move();
        }
        
        private void Move()
        {
            Position.X += Velocity.X;
            Position.Y += Velocity.Y;
        }

        public AbstractParticleUnit GetWarheadParticle()
        {
            return grenadeWarheadParticleUnit;
        }
    }
}