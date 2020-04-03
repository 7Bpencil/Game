﻿using System.Drawing;

namespace App.Physics_Engine.RigidBody
{
    public class RigidRectangle
    {
        private Vector center;
        public Vector Center
        {
            get { return center; }
            set
            {
                center = value;
                vertexesVersion++;
            }
        }

        public Vector topLeft
        {
            get
            {
                UpdateVectorIfNeeded();
                return vertexes[0];
            }
        }

        private float width;
        public float Width
        {
            get { return width; }
            set
            {
                width = value;
                vertexesVersion++;
            }
        }

        private float height;
        public float Height
        {
            get { return height; }
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
                UpdateVectorIfNeeded();
                return vertexes;
            }
        }


        private readonly Vector[] faceNormals;
        public Vector[] FaceNormals
        {
            get
            {
                UpdateVectorIfNeeded();
                return faceNormals;
            }
        }
        
        private long vertexesVersion;
        private long calculatedVertexesVersion;

        /// <summary>
        /// Vertexes are count clockwise from topLeft
        /// Face Normals are count clockwise from "12 o'clock"
        /// </summary>
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="angle"></param>
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

        public void Draw(Graphics g, Pen pen)
        {
            var stateBefore = g.Save();
            if (!topLeft.Equals(Vector.ZeroVector))
                g.TranslateTransform(topLeft.X, topLeft.Y);
            if (angle != 0)
                g.RotateTransform(angle);
            g.DrawRectangle(pen, 0, 0, width, height);
            g.Restore(stateBefore);
        }

        private void UpdateVectorIfNeeded()
        {
            if (vertexesVersion == calculatedVertexesVersion) return;
            RecomputeVertexes();
            RecomputeFaceNormals();
            calculatedVertexesVersion = vertexesVersion;
        }
    }
}