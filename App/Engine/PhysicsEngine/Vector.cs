using System;

namespace App.Engine.PhysicsEngine
{
    public class Vector
    {
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

        public float X { get; set; }
        public float Y { get; set; }

        public float Length => (float) Math.Sqrt(X * X + Y * Y);

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
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
            return new Vector(a.X / k, a.Y / k);
        }

        /// <summary>
        /// rotates vector counter-clockwise
        /// </summary>
        /// <param name="center"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Vector Rotate(Vector center, float angle)
        {
            var x = X - center.X;
            var y = Y - center.Y;
            return new Vector
            {
                X = (float) (x * Math.Cos(angle) - y * Math.Sin(angle) + center.X),
                Y = (float) (x * Math.Sin(angle) + y * Math.Cos(angle) + center.Y)
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

        /// <summary>
        /// x1 * y2 - y1 * x2
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public float Cross(Vector other)
        {
            return X * other.Y - Y * other.X;
        }

        public Vector ConvertMathToWindow()
        {
            return null;
        }
        
        public Vector ConvertFromWorldToCamera(Vector cameraPosition)
        {
            return this - cameraPosition;
        }
    }
}