using System.Drawing;

namespace App.Engine.PhysicsEngine.RigidBody
{
    public abstract class RigidShape
    {
        public abstract Vector Center { get; set; }
        public abstract bool IsCollided { get; set; }
        public abstract void Update();
        public abstract void Move(Vector delta);
        public abstract void Rotate(float delta);
        public abstract void BoundTest(RigidShape other);
    }
}