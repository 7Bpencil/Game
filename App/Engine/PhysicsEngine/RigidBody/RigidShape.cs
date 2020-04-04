using System.Drawing;

namespace App.Engine.PhysicsEngine.RigidBody
{
    public abstract class RigidShape
    {
        public abstract Vector Center { get; set; }
        public abstract void Draw(Graphics g, Pen pen);
        public abstract void Update();
        public abstract void Move(Vector delta);
        public abstract void Rotate(float delta);
        public abstract bool BoundTest(RigidShape other);
    }
}