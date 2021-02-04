using System;
using App.Engine.Physics.RigidShapes;

namespace App.Engine.Physics.Collision
{
    public static class CollisionDetector
    {
        public static CollisionInfo GetCollisionInfo(RigidShape first, RigidShape second)
        {
            switch (first)
            {
                case RigidCircle firstCircle when second is RigidCircle secondCircle:
                    return GetCollisionInfo(firstCircle, secondCircle);

                case RigidAABB aabb when second is RigidCircle circle:
                    return GetCollisionInfo(aabb, circle);
                case RigidCircle circle when second is RigidAABB aabb:
                    return GetCollisionInfo(aabb, circle);

                case RigidCircle circle when second is RigidCircleQuarter quarter:
                    return GetCollisionInfo(circle, quarter);
                case RigidCircleQuarter quarter when second is RigidCircle circle:
                    return GetCollisionInfo(circle, quarter);

                case RigidCircle circle when second is RigidTriangle triangle:
                    return GetCollisionInfo(circle, triangle);
                case RigidTriangle triangle when second is RigidCircle circle:
                    return GetCollisionInfo(circle, triangle);

                default:
                    return null;
            }
        }

        private static CollisionInfo GetCollisionInfo(RigidCircle first, RigidCircle second)
        {
            var vFromFirstToSecond = second.Center - first.Center;
            var vLengthSqrt = Vector.ScalarProduct(vFromFirstToSecond, vFromFirstToSecond);
            var sumRadii = first.Radius + second.Radius;

            if (sumRadii * sumRadii < vLengthSqrt) return null;
            if (Math.Abs(vLengthSqrt) > 0.01)
            {
                return new CollisionInfo(
                    sumRadii - vFromFirstToSecond.Length,
                    -vFromFirstToSecond.Normalize(),
                    first.Center + vFromFirstToSecond.Normalize() * first.Radius);
            }

            var maxRadius = Math.Max(first.Radius, second.Radius);
            return new CollisionInfo(
                maxRadius,
                new Vector(0, -1),
                first.Center + new Vector(0, 1) * maxRadius);
        }

        private static CollisionInfo GetCollisionInfo(RigidAABB rectangle, RigidCircle circle)
        {
            var closestPoint = GetClosestPoint(circle.Center, rectangle);
            var v = closestPoint - circle.Center;
            return Vector.ScalarProduct(v,v) <= circle.Radius * circle.Radius
                ? new CollisionInfo(circle.Radius - v.Length, -v.Normalize(), closestPoint)
                : null;
        }

        private static Vector GetClosestPoint(Vector point, RigidAABB rectangle)
        {
            var result = Vector.ZeroVector;
            for (var i = 0; i < 2; i++) {
                var v = point[i];
                if (v < rectangle.MinPoint[i]) v = rectangle.MinPoint[i];
                if (v > rectangle.MaxPoint[i]) v = rectangle.MaxPoint[i];
                result[i] = v;
            }

            return result;
        }

        private static CollisionInfo GetCollisionInfo(RigidCircle circle, RigidTriangle triangle)
        {
            var a = triangle.Points[0]; var b = triangle.Points[1]; var c = triangle.Points[2];
            var q = GetClosestPoint(circle.Center, a, b, c);
            var v = q - circle.Center;
            return Vector.ScalarProduct(v,v) <= circle.Radius * circle.Radius
                ? new CollisionInfo(v.Length, v, q)
                : null;
        }

        public static CollisionInfo GetCollisionInfo(RigidCircle circle, Vector a, Vector b, Vector c)
        {
            var q = GetClosestPoint(circle.Center, a, b, c);
            var v = q - circle.Center;
            return Vector.ScalarProduct(v,v) <= circle.Radius * circle.Radius
                ? new CollisionInfo(v.Length, v, q)
                : null;
        }

        private static Vector GetClosestPoint(Vector point, Vector a, Vector b, Vector c)
        {
            float v, w;

            // Check if P in vertex region outside A
            var ab = b - a;
            var ac = c - a;
            var ap = point - a;
            var d1 = Vector.ScalarProduct(ab, ap);
            var d2 = Vector.ScalarProduct(ac, ap);
            if (d1 <= 0.0f && d2 <= 0.0f) return a; // barycentric coordinates (1,0,0)

            // Check if P in vertex region outside B
            var bp = point - b;
            var d3 = Vector.ScalarProduct(ab, bp);
            var d4 = Vector.ScalarProduct(ac, bp);
            if (d3 >= 0.0f && d4 <= d3) return b; // barycentric coordinates (0,1,0)

            // Check if P in edge region of AB, if so return projection of P onto AB
            var vc = d1*d4 - d3*d2;
            if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f)
            {
                v = d1 / (d1 - d3);
                return a + v * ab; // barycentric coordinates (1-v,v,0)
            }

            // Check if P in vertex region outside C
            var cp = point - c;
            var d5 = Vector.ScalarProduct(ab, cp);
            var d6 = Vector.ScalarProduct(ac, cp);
            if (d6 >= 0.0f && d5 <= d6) return c; // barycentric coordinates (0,0,1)

            // Check if P in edge region of AC, if so return projection of P onto AC
            var vb = d5*d2 - d1*d6;
            if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f)
            {
                w = d2 / (d2 - d6);
                return a + w * ac; // barycentric coordinates (1-w,0,w)
            }

            // Check if P in edge region of BC, if so return projection of P onto BC
            var va = d3*d6 - d5*d4;
            if (va <= 0.0f && (d4 - d3) >= 0.0f && (d5 - d6) >= 0.0f)
            {
                w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
                return b + w * (c - b); // barycentric coordinates (0,1-w,w)
            }

            // P inside face region. Compute Q through its barycentric coordinates (u,v,w)
            var denom = 1.0f / (va + vb + vc);
            v = vb * denom;
            w = vc * denom;
            return a + ab * v + ac * w; // = u*a + v*b + w*c, u = va * denom = 1.0f-v-w
        }

        private static CollisionInfo GetCollisionInfo(RigidCircle circle, RigidCircleQuarter circleQuarter)
        {
            var info = GetCollisionInfo(circle, circleQuarter.WholeCircle);
            if (info == null) return null;
            var vector = info.Start - circleQuarter.Center;
            var vertical = Vector.ScalarProduct(vector, circleQuarter.Direction);
            var horizontal = Vector.ScalarProduct(vector, circleQuarter.DirectionNormal);
            switch (circleQuarter.QuarterIndex)
            {
                case 1 when vertical >= 0 && horizontal >= 0:
                    return info;
                case 2 when vertical >= 0 && horizontal <= 0:
                    return info;
                case 3 when vertical <= 0 && horizontal <= 0:
                    return info;
                case 4 when vertical <= 0 && horizontal >= 0:
                    return info;
                default:
                    return null;
            }
        }

        public static bool AreCollide(Edge first, Edge second)
        {
            return AreEdgesCross(first.Start, first.End, second.Start, second.End);
        }

        public static bool AreCollide(Vector firstStart, Vector firstEnd, Edge second)
        {
            return AreEdgesCross(firstStart, firstEnd, second.Start, second.End);
        }

        private static bool AreEdgesCross(Vector firstStart, Vector firstEnd, Vector secondStart, Vector secondEnd)
        {
            var firstSegmentVector = firstEnd - firstStart;
            var secondSegmentVector = secondEnd - secondStart;
            var firstCase = Vector.VectorProduct(secondStart - firstStart, firstSegmentVector) *
                Vector.VectorProduct(secondEnd - firstStart, firstSegmentVector) <= 0;
            var secondCase = Vector.VectorProduct(firstStart - secondStart, secondSegmentVector) *
                Vector.VectorProduct(firstEnd - secondStart, secondSegmentVector) <= 0;
            return AreBoundingBoxesCross(firstStart, firstEnd, secondStart, secondEnd) && firstCase && secondCase;
        }

        private static bool AreBoundingBoxesCross(Vector firstStart, Vector firstEnd, Vector secondStart, Vector secondEnd)
        {
            return Math.Max(firstStart.X, firstEnd.X) >= Math.Min(secondStart.X, secondEnd.X)
                   && Math.Max(secondStart.X, secondEnd.X) >= Math.Min(firstStart.X, firstEnd.X)
                   && Math.Max(firstStart.Y, firstEnd.Y) >= Math.Min(secondStart.Y, secondEnd.Y)
                   && Math.Max(secondStart.Y, secondEnd.Y) >= Math.Min(firstStart.Y, firstEnd.Y);
        }
    }
}
