using System.Drawing;

namespace App.Physics_Engine.RigidBody
{
    public class RigidCircle : RigidShape
    {
        private float radius;

        public float Radius
        {
            get => radius;
            set => radius = value;
        }

        private Vector center;

        public override Vector Center
        {
            get => center;
            set => center = value;
        }

        public RigidCircle(Vector center, float radius)
        {
            this.radius = radius;
            this.center = center;
        }

        public override void Draw(Graphics g, Pen pen)
        {
            var stateBefore = g.Save();
            if (!center.Equals(Vector.ZeroVector))
                g.TranslateTransform(center.X, center.Y);
            g.DrawEllipse(pen, -radius / 2, -radius / 2, radius, radius);
            g.Restore(stateBefore);
        }

        public override void Update()
        {
        }

        public override void Move(Vector delta)
        {
            center += delta;
        }

        public override void Rotate(float delta)
        {
        }

        public override bool BoundTest(RigidShape other)
        {
            return true;
        }
    }
}