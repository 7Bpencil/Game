using System;
using System.Collections.Generic;
using App.Engine.PhysicsEngine.RigidBody;

namespace App.Engine.PhysicsEngine.Collision
{
    public class CollisionDetection
    {
        public List<CollisionInfo> CalculateCollisions(List<RigidShape> sceneObjects)
        {
            ClearColliding(sceneObjects);
            var collisions = new List<CollisionInfo>();
            
            for (var i = 0; i < sceneObjects.Count; i++)
            {
                for (var k = i + 1; k < sceneObjects.Count; k++)
                {
                    if (!(sceneObjects[i].CanCollide && sceneObjects[k].CanCollide)) continue;
                    if (!sceneObjects[i].CanBound(sceneObjects[k])) continue;
                    var collisionInfo = GetCollisionInfo(sceneObjects[i], sceneObjects[k]);
                    if (collisionInfo == null) continue;
                    
                    sceneObjects[i].IsCollided = sceneObjects[k].IsCollided = true;
                    collisions.Add(collisionInfo);
                }
            }

            return collisions;
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
                    vectorFromFirstToSecond.Normalize(),
                    first.Center + vectorFromFirstToSecond * (1 - second.Radius / distance));
            }
            var maxRadius = Math.Max(first.Radius, second.Radius);
            return new CollisionInfo(
                maxRadius,
                new Vector(0, -1), 
                first.Center + new Vector(0,1) * maxRadius);
        }
        
        private static CollisionInfo GetCollisionInfo(RigidRectangle first, RigidRectangle second)
        {
            var firstClosestSupportPoint = FindClosestSupportPoint(first, second);
            if (firstClosestSupportPoint == null) return null;
            var secondClosestSupportPoint = FindClosestSupportPoint(second, first);
            if (secondClosestSupportPoint == null) return null;
            return ShortestCollision(firstClosestSupportPoint,secondClosestSupportPoint);
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
            return new CollisionInfo(bestDistance, closestSupportPoint.FaceNormal, closestSupportPoint.SupportPoint + bestVec);

            /*
            CollisionInfo closestSupportPoint = null;
            var minSupportDistance = float.PositiveInfinity;
            for (var i = 0; i < first.Vertexes.Length; i++)
            {
                var pointOnFace = first.Vertexes[i];
                var negativeFaceNormal = -1 * first.FaceNormals[i];
                var supportPoint = FindSupportPoint(negativeFaceNormal, pointOnFace, second);
                if (supportPoint == null) return null;
                if (supportPoint.Depth >= minSupportDistance) continue;
                minSupportDistance = supportPoint.Depth;
                closestSupportPoint = supportPoint;
            }

            return closestSupportPoint;
            */
        }
        
        private static SupportPointInfo FindSupportPoint(Vector negativeFaceNormal, Vector pointOnFace, RigidRectangle rectangle)
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
                return new CollisionInfo(second.Depth, -1 * second.Normal, second.CollisionStart);
            var depthVec = first.Normal * first.Depth;
            return new CollisionInfo(first.Depth, first.Normal, first.CollisionStart - depthVec);
        }

        private static CollisionInfo GetCollisionInfo(RigidRectangle first, RigidCircle second)
        {
            return null;
        }

        private static void ClearColliding(List<RigidShape> sceneObjects)
        {
            foreach (var sceneObject in sceneObjects)
                sceneObject.IsCollided = false;
        }

        private class SupportPointInfo
        {
            public Vector SupportPoint;
            public float SupportPointDistance;
            public Vector FaceNormal;

            public SupportPointInfo(Vector supportPoint, float supportPointDistance, Vector faceNormal)
            {
                SupportPoint = supportPoint;
                SupportPointDistance = supportPointDistance;
                FaceNormal = faceNormal;
            }
        }
    }
}