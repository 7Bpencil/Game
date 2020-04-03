using System.Drawing;

namespace App.Physics_Engine.RigidBody
{
    public interface IRigidShape
    {
        void Draw(Graphics g, Pen pen);
        void Update();
        void Move(Vector delta);
        void Rotate(float delta);
    }
}