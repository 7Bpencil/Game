using System.Drawing;

namespace App.Engine.PhysicsEngine.RigidBody
{
    public class RigidCircle : RigidShape
    {
        private float radius;
        public float Radius
        { get => radius; set => radius = value; }
        
        public float Diameter => 2 * radius;

        private Vector center;
        public override Vector Center 
        { get => center; set => center = value; }

        private Pen strokePen;
        public override Pen StrokePen 
        { get => strokePen; set => strokePen = value; }
        
        private bool isCollided;
        public override bool IsCollided { get => isCollided; set => isCollided = value; }
        

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
            g.DrawEllipse(strokePen, -radius, -radius, Diameter, Diameter);
            g.Restore(stateBefore);
        }

        public override void DrawCollision(Graphics g, Pen collisionStrokePen)
        {
            var stateBefore = g.Save();
            if (!center.Equals(Vector.ZeroVector))
                g.TranslateTransform(center.X, center.Y);
            g.DrawEllipse(collisionStrokePen, -radius, -radius, Diameter, Diameter);
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

        public override void BoundTest(RigidShape other)
        {
            isCollided = true;
        }
    }
}