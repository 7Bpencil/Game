using System.Drawing;

namespace App.Engine.PhysicsEngine.RigidBody
{
    public abstract class RigidShape
    {
        public abstract Vector Center { get; set; }
        public abstract Pen StrokePen { get; set; }
        public abstract void Draw(Graphics g);
        public abstract void DrawCollision(Graphics g, Pen collisionStrokePen);
        public abstract bool IsCollided { get; set; }
        public abstract void Update();
        public abstract void Move(Vector delta);
        public abstract void Rotate(float delta);
        public abstract void BoundTest(RigidShape other);
    }
}