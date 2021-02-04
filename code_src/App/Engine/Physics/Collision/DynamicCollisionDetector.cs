using System;
using App.Engine.Physics.RigidShapes;
using App.Model.Entities;

namespace App.Engine.Physics.Collision
{
    public static class DynamicCollisionDetector
    {
        public static float[] AreCollideWithStatic(Bullet bullet, RigidShape staticBody)
        {
            switch (staticBody)
            {
                case RigidAABB aabb: return AreCollide(bullet.Position, bullet.Velocity, aabb);
                case RigidCircle circle: return AreCollide(bullet.Position, bullet.Velocity, circle);
                default: return null;
            }
        }

        public static float[] AreCollideWithStatic(Vector objectPosition, Vector objectVelocity, RigidShape staticBody)
        {
            switch (staticBody)
            {
                case RigidAABB aabb: return AreCollide(objectPosition, objectVelocity, aabb);
                case RigidCircle circle: return AreCollide(objectPosition, objectVelocity, circle);
                default: return null;
            }
        }

        private static float[] AreCollide(Vector objectPosition, Vector objectVelocity, RigidAABB rectangle)
        {
            var tMin = 0f;
            var tMax = float.PositiveInfinity;
            for (var i = 0; i < 2; i++)
            {
                if (Math.Abs(objectVelocity[i]) < 0.01
                    && (objectPosition[i] < rectangle.MinPoint[i]
                        || objectPosition[i] > rectangle.MaxPoint[i]))
                    return null;

                var ood = 1.0f / objectVelocity[i];
                var t1 = (rectangle.MinPoint[i] - objectPosition[i]) * ood;
                var t2 = (rectangle.MaxPoint[i] - objectPosition[i]) * ood;
                if (t1 > t2)
                {
                    var buf = t1;
                    t1 = t2;
                    t2 = buf;
                }

                tMin = Math.Max(tMin, t1);
                tMax = Math.Min(tMax, t2);
                if (tMin > tMax) return null;
            }

            return new[] {tMin, tMax};
        }

        private static float[] AreCollide(Vector objectPosition, Vector objectVelocity, RigidCircle circle)
        {
            return GetPenetrationTimeWithMovingCircle(objectPosition, objectVelocity, circle, Vector.ZeroVector);
        }

        public static float[] AreCollideWithDynamic(Bullet bullet, RigidCircle circle, Vector circleVelocity)
        {
            var time = GetPenetrationTimeWithMovingCircle(bullet.Position, bullet.Velocity, circle, circleVelocity);
            if (time == null) return null;
            var t1 = time[0];
            var t2 = time[1];

            // TODO there could be great optimization but it's really case specific
            if (t1 > 1) return null;

            var ist1 = t1 > 0;
            var ist2 = t2 > 0 && t2 < 1;

            if (ist1 && ist2) return new[] {t1, t2};
            if (ist1) return new[] {t1};
            if (ist2) return new[] {t2};
            return null;
        }

        private static float[] GetPenetrationTimeWithMovingCircle(
            Vector objectPosition, Vector objectVelocity, RigidCircle circle, Vector circleVelocity)
        {
            var dX = objectPosition.X - circle.Center.X;
            var dY = objectPosition.Y - circle.Center.Y;
            var dVx = objectVelocity.X - circleVelocity.X;
            var dVy = objectVelocity.Y - circleVelocity.Y;
            // t^2 * (dVx^2 + dVy^2) + 2t(dX * dVx + dY * dVy) + (dX^2 + dY^2 - R^2) = 0

            var a = dVx * dVx + dVy * dVy;
            var b = dX * dVx + dY * dVy;
            var D = 4 * (b * b - a * (dX * dX + dY * dY - circle.Radius * circle.Radius));
            if (D < 0) return null;

            var delA = 1 / (2 * a);
            var sqrtD = (float) Math.Sqrt(D);

            return new[]
            {
                (-2 * b - sqrtD) * delA,
                (-2 * b + sqrtD) * delA
            };
        }
    }
}
