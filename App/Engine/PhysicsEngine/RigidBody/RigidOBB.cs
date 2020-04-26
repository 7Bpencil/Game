using System;

namespace App.Engine.PhysicsEngine.RigidBody
{
    public class RigidOBB : RigidShape
    {
        private Vector center;
        public override Vector Center => center;
        
        public Vector TopLeft
        {
            get
            {
                UpdateVertexesIfNeeded();
                return vertexes[0];
            }
        }

        public readonly float Width;
        public readonly float Height;

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

        public override bool IsCollided { get; set; }

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
        public RigidOBB(Vector center, float width, float height, float angle, bool isStatic, bool canCollide)
        {
            this.center = center;
            Width = width;
            Height = height;
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
            vertexes[0] = new Vector(center.X - Width / 2, center.Y - Height / 2);
            vertexes[1] = new Vector(center.X + Width / 2, center.Y - Height / 2);
            vertexes[2] = new Vector(center.X + Width / 2, center.Y + Height / 2);
            vertexes[3] = new Vector(center.X - Width / 2, center.Y + Height / 2);
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

        public override void MoveBy(Vector delta)
        {
            center.X += delta.X;
            center.Y += delta.Y;
            vertexesVersion ++;
        }

        public override RigidShape Copy()
        {
            return new RigidOBB(center.Copy(), Width, Height, angle, isStatic, canCollide);
        }
    }
}