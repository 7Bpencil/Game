using System.Drawing;

namespace App.Engine.PhysicsEngine.RigidBody
{
    public class RigidCircle : RigidShape
    {
        private float radius;
        public float Radius
        { get => radius; set => radius = value; }

        private Vector center;
        public override Vector Center 
        { get => center; set => center = value; }

        private Pen strokePen;
        public override Pen StrokePen 
        { get => strokePen; set => strokePen = value; }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="strokePen">Color and Width of stroke</param>
        public RigidCircle(Vector center, float radius, Pen strokePen)
        {
            this.radius = radius;
            this.center = center;
            this.strokePen = strokePen;
        }

        public override void Draw(Graphics g)
        {
            var stateBefore = g.Save();
            if (!center.Equals(Vector.ZeroVector))
                g.TranslateTransform(center.X, center.Y);
            g.DrawEllipse(strokePen, -radius / 2, -radius / 2, radius, radius);
            g.Restore(stateBefore);
        }

        public override void Update()
        {
        }

        public override void Move(Vector delta)
        {
            center += delta;
        }

        public override void Rotate(float delta) // Because it's meaningless
        {
        }

        public override bool BoundTest(RigidShape other)
        {
            return true;
        }
    }
}