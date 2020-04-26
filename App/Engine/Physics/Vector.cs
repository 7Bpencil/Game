using System;
using System.Drawing;

namespace App.Engine.Physics
{
    public class Vector
    {
        public float X { get; set; }
        public float Y { get; set; }

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

        public Vector()
        {
        }

        public Vector(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector ZeroVector => new Vector(0, 0);

        public override string ToString()
        {
            return string.Format($"X: {X}, Y: {Y}");
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

        public Vector Rotate(float angleInDegrees)
        {
            var angle = angleInDegrees / 180 * Math.PI;
            return new Vector
            {
                X = (float) (X * Math.Cos(angle) - Y * Math.Sin(angle)),
                Y = (float) (X * Math.Sin(angle) + Y * Math.Cos(angle))
            };
        }

        public Vector Normalize()
        {
            return Length > 0 ? this * (1 / Length) : this;
        }

        public static float Distance(Vector a, Vector b)
        {
            var x = a.X - b.X;
            var y = a.Y - b.Y;
            return (float) Math.Sqrt(x * x + y * y);
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

        public Vector ConvertFromCameraToWorld(Vector cameraPosition)
        {
            return this + cameraPosition;
        }

        public Vector GetNormal()
        {
            if (Math.Abs(Y) < 0.01) return new Vector(0, 1);
            return new Vector(1, -X / Y).Normalize();
        }

        public Vector Copy() => new Vector(X, Y);

        public PointF GetPoint() => new PointF(X, Y);
    }
}