using System;
using System.Collections.Generic;
using App.Engine.PhysicsEngine.RigidBody;

namespace App.Engine.PhysicsEngine.Collision
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
                        || !(sceneObjects[i].CanCollide && sceneObjects[k].CanCollide) 
                        || !sceneObjects[i].CanBound(sceneObjects[k])) continue;
                    
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
            if (second.IsStatic) first.Move(info.Normal * info.Depth);
            else if (first.IsStatic) second.Move(info.Normal * info.Depth);
            else second.Move(info.Normal * info.Depth);
        }

        private static CollisionInfo GetCollisionInfo(RigidShape first, RigidShape second)
        {
            if (first is RigidCircle && second is RigidCircle)
                return GetCollisionInfo((RigidCircle) first, (RigidCircle) second);
            if (first is RigidRectangle && second is RigidRectangle)
                return GetCollisionInfo((RigidRectangle) first, (RigidRectangle) second);
            if (first is RigidRectangle && second is RigidCircle)
                return GetCollisionInfo((RigidRectangle) first, (RigidCircle) second);
            if (first is RigidCircle && second is RigidRectangle)
                return GetCollisionInfo((RigidRectangle) second, (RigidCircle) first);
            return null;
        }

        private static CollisionInfo GetCollisionInfo(RigidCircle first, RigidCircle second) // TODO сделать красиво
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

        private static CollisionInfo GetCollisionInfo(RigidRectangle first, RigidRectangle second)
        {
            var firstClosestSupportPoint = FindClosestSupportPoint(first, second);
            if (firstClosestSupportPoint == null) return null;
            var secondClosestSupportPoint = FindClosestSupportPoint(second, first);
            if (secondClosestSupportPoint == null) return null;
            return ShortestCollision(firstClosestSupportPoint, secondClosestSupportPoint);
        }

        private static CollisionInfo FindClosestSupportPoint(RigidRectangle first, RigidRectangle second)
        {
            SupportPointInfo closestSupportPoint = null;
            var bestDistance = float.MaxValue;
            for (var i = 0; i < first.FaceNormals.Length; i++)
            {
                var supportPoint = FindSupportPoint(-1 * first.FaceNormals[i], first.Vertexes[i], second);
                if (supportPoint == null) return null;
                if (supportPoint.SupportPointDistance >= bestDistance) continue;
                bestDistance = supportPoint.SupportPointDistance;
                closestSupportPoint = supportPoint;
            }

            var bestVec = closestSupportPoint.FaceNormal * closestSupportPoint.SupportPointDistance;
            return new CollisionInfo(bestDistance, closestSupportPoint.FaceNormal,
                closestSupportPoint.SupportPoint + bestVec);
        }

        private static SupportPointInfo FindSupportPoint(
            Vector negativeFaceNormal,
            Vector pointOnFace,
            RigidRectangle rectangle)
        {
            Vector supportPoint = null;
            var maxSupportDistance = 0f;
            foreach (var vertex in rectangle.Vertexes)
            {
                var vectorFromPointToVertex = vertex - pointOnFace;
                var projectionLength = Vector.ScalarProduct(vectorFromPointToVertex, negativeFaceNormal);
                if (projectionLength <= maxSupportDistance) continue;
                supportPoint = vertex;
                maxSupportDistance = projectionLength;
            }

            return supportPoint == null
                ? null
                : new SupportPointInfo(supportPoint, maxSupportDistance, -1 * negativeFaceNormal);
        }

        private static CollisionInfo ShortestCollision(CollisionInfo first, CollisionInfo second)
        {
            if (first.Depth >= second.Depth)
                return new CollisionInfo(second.Depth, -1 * second.Normal, second.Start);
            var depthVec = first.Normal * first.Depth;
            return new CollisionInfo(first.Depth, first.Normal, first.Start - depthVec);
        }

        private static CollisionInfo GetCollisionInfo(RigidRectangle rectangle, RigidCircle circle)
        {
            var indexNearestEdge = -1;
            var minProjection = float.PositiveInfinity;
            for (var i = 0; i < rectangle.Vertexes.Length; i++)
            {
                var projection = Vector.ScalarProduct(rectangle.Vertexes[i] - circle.Center, rectangle.FaceNormals[i]);
                if (projection < 0) return CaseCenterIsOutside(-1 * projection, i, rectangle, circle);
                if (projection >= minProjection) continue;
                minProjection = projection;
                indexNearestEdge = i;
            }
            
            return new CollisionInfo(
                circle.Radius + minProjection,
                rectangle.FaceNormals[indexNearestEdge],
                circle.Center - rectangle.FaceNormals[indexNearestEdge] * circle.Radius);
        }

        private static CollisionInfo CaseCenterIsOutside(float minProjection, int nearestEdgeIndex,
            RigidRectangle rectangle, RigidCircle circle)
        {
            var v1 = circle.Center - rectangle.Vertexes[nearestEdgeIndex];
            var v2 = rectangle.Vertexes[(nearestEdgeIndex + 1) % 4] - rectangle.Vertexes[nearestEdgeIndex];
            var v3 = circle.Center - rectangle.Vertexes[(nearestEdgeIndex + 1) % 4];

            if (Vector.ScalarProduct(v1, v2) < 0)
            {
                if (v1.Length >= circle.Radius) return null;
                return new CollisionInfo(
                    circle.Radius - v1.Length,
                    v1.Normalize(),
                    circle.Center - v1.Normalize() * circle.Radius);
            }

            if (Vector.ScalarProduct(v3, v2) > 0)
            {
                if (v3.Length >= circle.Radius) return null;
                return new CollisionInfo(
                    circle.Radius - v3.Length,
                    v3.Normalize(),
                    circle.Center - v3.Normalize() * circle.Radius);
            }

            if (minProjection >= circle.Radius) return null;
            var radiusVector = rectangle.FaceNormals[nearestEdgeIndex] * circle.Radius;
            return new CollisionInfo(
                circle.Radius - minProjection,
                rectangle.FaceNormals[nearestEdgeIndex],
                circle.Center - radiusVector);
        }

        private static void ClearColliding(List<RigidShape> sceneObjects)
        {
            foreach (var sceneObject in sceneObjects)
                sceneObject.IsCollided = false;
        }

        private class SupportPointInfo
        {
            public readonly Vector SupportPoint;
            public readonly float SupportPointDistance;
            public readonly Vector FaceNormal;

            public SupportPointInfo(Vector supportPoint, float supportPointDistance, Vector faceNormal)
            {
                SupportPoint = supportPoint;
                SupportPointDistance = supportPointDistance;
                FaceNormal = faceNormal;
            }
        }
        
        private static CollisionInfo GetCollisionInfo(RigidCircle circle, RigidAABB rectangle)
        {
            var closestPoint = GetClosestPoint(circle.Center, rectangle);
            var distanceVector = closestPoint - circle.Center;
            var distance = distanceVector.Length;
            return distance <= circle.Radius 
                ? new CollisionInfo(distance, distanceVector.Normalize(), closestPoint) 
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
    }
}