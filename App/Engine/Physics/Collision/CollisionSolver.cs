using System;
using System.Collections.Generic;
using App.Engine.Physics.RigidShapes;

namespace App.Engine.Physics.Collision
{
    public static class CollisionSolver
    {
        public static List<CollisionInfo> ResolveCollisions(List<RigidShape> sceneObjects)
        {
            ClearColliding(sceneObjects);
            var collisions = new List<CollisionInfo>();

            for (var i = 0; i < sceneObjects.Count; i++)
            {
                for (var k = i + 1; k < sceneObjects.Count; k++)
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
            var distanceVector = closestPoint - circle.Center;
            var distance = distanceVector.Length;
            return distance <= circle.Radius 
                ? new CollisionInfo(circle.Radius - distance, -distanceVector.Normalize(), closestPoint) 
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
        
        private static void ClearColliding(List<RigidShape> sceneObjects)
        {
            foreach (var sceneObject in sceneObjects)
                sceneObject.IsCollided = false;
        }
    }
}