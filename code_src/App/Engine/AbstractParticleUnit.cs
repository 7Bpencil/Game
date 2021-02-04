using System.Drawing;
using App.Engine.Physics;

namespace App.Engine
{
    public abstract class AbstractParticleUnit
    {
        public abstract AbstractParticle Content { get; }
        public abstract Rectangle CurrentFrame { get; }
        public abstract Vector CenterPosition { get; }
        public abstract float Angle { get; }
        public abstract void UpdateFrame();
        public abstract bool IsExpired { get; set; }
        public abstract bool ShouldBeBurned { set; get; }
        public abstract void ClearContent();
    }
}