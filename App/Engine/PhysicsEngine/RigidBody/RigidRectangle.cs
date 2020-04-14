namespace App.Engine.PhysicsEngine.RigidBody
{
    public class RigidRectangle : RigidShape
    {
        private Vector center;
        public override Vector Center => center;

        public override float BondRadius => (Center - TopLeft).Length;

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
        
        private bool isStatic;
        public override bool IsStatic { get => isStatic; set => isStatic = value; }
        
        private bool canCollide;
        public override bool CanCollide { get => canCollide; set => canCollide = value; }
        
        private bool isCollided;
        public override bool IsCollided { get => isCollided; set => isCollided = value; }

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
        /// <param name="canCollide">should a collision be calculated</param>
        public RigidRectangle(Vector center, float width, float height, float angle, bool isStatic, bool canCollide)
        {
            this.center = center;
            this.width = width;
            this.height = height;
            this.angle = angle;
            this.isStatic = isStatic;
            this.canCollide = canCollide;
            
            vertexes = new Vector[4];
            faceNormals = new Vector[4];
            
            vertexesVersion = 0;
            calculatedVertexesVersion = -1;
        }

        private void RecomputeVertexes()
        {
            vertexes[0] = new Vector(center.X - width / 2, center.Y - height / 2);
            vertexes[1] = new Vector(center.X + width / 2, center.Y - height / 2);
            vertexes[2] = new Vector(center.X + width / 2, center.Y + height / 2);
            vertexes[3] = new Vector(center.X - width / 2, center.Y + height / 2);
        }

        private void RecomputeFaceNormals()
        {
            faceNormals[0] = (vertexes[1] - vertexes[2]).Normalize();
            faceNormals[1] = (vertexes[2] - vertexes[3]).Normalize();
            faceNormals[2] = -1 * faceNormals[0];
            faceNormals[3] = -1 * faceNormals[1];
        }
        
        private void UpdateVertexesIfNeeded()
        {
            if (vertexesVersion == calculatedVertexesVersion) return;
            RecomputeVertexes();
            RecomputeFaceNormals();
            calculatedVertexesVersion = vertexesVersion;
        }

        public override void Move(Vector delta)
        {
            center += delta;
            vertexesVersion ++;
        }

        public override void Rotate(float delta)
        {
            angle += delta;
        }
        
        public override RigidShape DeepCopy()
        {
            return new RigidRectangle(center, width, height, angle, isStatic, canCollide);
        }
    }
}