using System;
using System.Drawing;

namespace App.Engine.Physics
{
    public class Vector
    {
        public float X;
        public float Y;

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public Vector(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector ZeroVector => new Vector(0, 0);

        public override string ToString()
        {
            return "X=" + X + " Y=" + Y;
        }

        private bool Equals(Vector other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Vector) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public float Length => (float) Math.Sqrt(X * X + Y * Y);

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }

        public static Vector operator -(Vector a)
        {
            return new Vector(-a.X, -a.Y);
        }

        public static Vector operator *(Vector a, float k)
        {
            return new Vector(a.X * k, a.Y * k);
        }

        public static Vector operator *(float k, Vector a)
        {
            return a * k;
        }

        public static Vector operator /(Vector a, float k)
        {
            var d = 1 / k;
            return a * d;
        }

        public Vector Rotate(float angleInDegrees, Vector rotationCenter)
        {
            var angle = angleInDegrees / 180 * Math.PI;
            var x = X - rotationCenter.X;
            var y = Y - rotationCenter.Y;
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            return new Vector
            (
                (float) (rotationCenter.X + x * cos - y * sin),
                (float) (rotationCenter.Y + x * sin + y * cos)
            );
        }
        
        public float Angle => (float) Math.Atan2(-Y, X);

        public Vector Normalize()
        {
            var thisLength = Length;
            return thisLength > 0 ? this / thisLength : this;
        }

        public static float ScalarProduct(Vector first, Vector second)
        {
            return first.X * second.X + first.Y * second.Y;
        }

        public static float VectorProduct(Vector first, Vector second)
        {
            return first.X * second.Y - first.Y * second.X;
        }

        public Vector ConvertFromWorldToCamera(Vector cameraPosition)
        {
            return this - cameraPosition;
        }

        /// <summary>
        /// returns clock-wise normal
        /// </summary>
        /// <returns></returns>
        public Vector GetNormal()
        {
            return - new Vector(Y, -X);
        }

        public Vector Copy() => new Vector(X, Y);

        public PointF GetPoint() => new PointF(X, Y);
    }
}