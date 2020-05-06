using System;
using System.Collections.Generic;
using App.Engine.Physics.RigidShapes;
using App.Model.LevelData;

namespace App.Engine.Physics.Collision
{
    public static class CollisionSolver
    {
        public static List<CollisionInfo> ResolveCollisions(ShapesIterator sceneObjects)
        {
            ClearColliding(sceneObjects);
            var collisions = new List<CollisionInfo>();

            for (var i = 0; i < sceneObjects.Length; i++)
            {
                for (var k = i + 1; k < sceneObjects.Length; k++)
                {
                    if (sceneObjects[i].IsStatic && sceneObjects[k].IsStatic
                        || !(sceneObjects[i].CanCollide && sceneObjects[k].CanCollide)) continue;
                    
                    var collisionInfo = GetCollisionInfo(sceneObjects[i], sceneObjects[k]);
                    if (collisionInfo == null) continue;

                    sceneObjects[i].IsCollided = sceneObjects[k].IsCollided = true;
                    ResolveCollisionStatically(collisionInfo, sceneObjects[i], sceneObjects[k]);
                    collisions.Add(collisionInfo);
                }
            }

            return collisions;
        }

        private static void ResolveCollisionStatically(CollisionInfo info, RigidShape first, RigidShape second)
        {
            if (second.IsStatic) first.MoveBy(info.Normal * info.Depth);
            else if (first.IsStatic) second.MoveBy(info.Normal * info.Depth);
            else first.MoveBy(info.Normal * info.Depth);
        }

        public static CollisionInfo GetCollisionInfo(RigidShape first, RigidShape second)
        {
            if (first is RigidAABB && second is RigidCircle)
                return GetCollisionInfo((RigidAABB) first, (RigidCircle) second);
            if (first is RigidCircle && second is RigidAABB)
                return GetCollisionInfo((RigidAABB) second, (RigidCircle) first);
            
            if (first is RigidCircle && second is RigidCircle)
                return GetCollisionInfo((RigidCircle) first, (RigidCircle) second);
            
            return null;
        }

        private static CollisionInfo GetCollisionInfo(RigidCircle first, RigidCircle second)
        {
            var vectorFromFirstToSecond = second.Center - first.Center;
            var distance = vectorFromFirstToSecond.Length;
            var collisionDepth = first.Radius + second.Radius - distance;
            if (collisionDepth < 0)
                return null;
            if (collisionDepth != 0)
            {
                return new CollisionInfo(
                    collisionDepth,
                    -vectorFromFirstToSecond.Normalize(),
                    first.Center + vectorFromFirstToSecond.Normalize() * first.Radius);
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
        
        public static CollisionInfo GetCollisionInfo(RigidCircle circle, Vector a, Vector b, Vector c)
        {
            var q = GetClosestPoint(circle.Center, a, b, c);
            var v = q - circle.Center;
            return Vector.ScalarProduct(v,v) <= circle.Radius * circle.Radius 
                ? new CollisionInfo(v.Length, v, q) 
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
            
            // // Check if P in edge region of AB, if so return projection of P onto AB
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
        
        private static void ClearColliding(ShapesIterator sceneObjects)
        {
            for (var i = 0; i < sceneObjects.Length; i++)
                sceneObjects[i].IsCollided = false;
        }
    }
}