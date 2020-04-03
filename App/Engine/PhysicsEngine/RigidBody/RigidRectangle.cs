using System.Drawing;

namespace App.Engine.PhysicsEngine.RigidBody
{
    public class RigidRectangle : RigidShape
    {
        private Vector center;

        public override Vector Center
        {
            get => center;
            set
            {
                center = value;
                vertexesVersion++;
            }
        }

        public Vector TopLeft
        {
            get
            {
                UpdateVertexesIfNeeded();
                return vertexes[0];
            }
        }

        private float width;

        public float Width
        {
            get => width;
            set
            {
                width = value;
                vertexesVersion++;
            }
        }

        private float height;

        public float Height
        {
            get => height;
            set
            {
                height = value;
                vertexesVersion++;
            }
        }

        public float angle { get; set; }


        private readonly Vector[] vertexes;

        public Vector[] Vertexes
        {
            get
            {
                UpdateVertexesIfNeeded();
                return vertexes;
            }
        }


        private readonly Vector[] faceNormals;

        public Vector[] FaceNormals
        {
            get
            {
                UpdateVertexesIfNeeded();
                return faceNormals;
            }
        }

        private long vertexesVersion;
        private long calculatedVertexesVersion;

        /// <summary>
        /// Rotation is counter-clockwise.  
        /// Vertexes are count clockwise from topLeft. 
        /// FaceNormals are count clockwise from "12 o'clock"
        /// </summary>
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="angle">Angle in degrees</param>
        public RigidRectangle(Vector center, float width, float height, float angle)
        {
            this.center = center;
            this.width = width;
            this.height = height;
            this.angle = angle;
            vertexes = new Vector[4];
            faceNormals = new Vector[4];
            vertexesVersion = 0;
            calculatedVertexesVersion = -1;
        }

        public void RecomputeVertexes()
        {
            vertexes[0] = new Vector(center.X - width / 2, center.Y - height / 2);
            vertexes[1] = new Vector(center.X + width / 2, center.Y - height / 2);
            vertexes[2] = new Vector(center.X + width / 2, center.Y + height / 2);
            vertexes[3] = new Vector(center.X - width / 2, center.Y + height / 2);
        }

        public void RecomputeFaceNormals()
        {
            faceNormals[0] = (vertexes[1] - vertexes[2]).Normalize();
            faceNormals[1] = (vertexes[2] - vertexes[3]).Normalize();
            faceNormals[2] = -1 * faceNormals[0];
            faceNormals[3] = -1 * faceNormals[1];
        }

        public override void Draw(Graphics g, Pen pen)
        {
            var stateBefore = g.Save();
            if (!center.Equals(Vector.ZeroVector))
                g.TranslateTransform(center.X, center.Y);
            if (angle != 0)
                g.RotateTransform(-angle);
            g.DrawRectangle(pen, -width / 2, -height / 2, width, height);
            g.Restore(stateBefore);
        }

        private void UpdateVertexesIfNeeded()
        {
            if (vertexesVersion == calculatedVertexesVersion) return;
            RecomputeVertexes();
            RecomputeFaceNormals();
            calculatedVertexesVersion = vertexesVersion;
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
            angle += delta;
        }

        public override bool BoundTest(RigidShape other)
        {
            return true;
        }
    }
}