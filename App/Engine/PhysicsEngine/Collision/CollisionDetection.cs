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
                    
                    if (sceneObjects[i] is RigidCircle
                        && sceneObjects[k] is RigidCircle
                        && AreColliding((RigidCircle) sceneObjects[i], (RigidCircle) sceneObjects[k], collisions))
                    {
                        sceneObjects[i].IsCollided = sceneObjects[k].IsCollided = true;
                    }
                    
                    
                    if (sceneObjects[i] is RigidRectangle 
                        && sceneObjects[k] is RigidRectangle
                        && AreColliding((RigidRectangle) sceneObjects[i], (RigidRectangle) sceneObjects[k], collisions))
                    {
                        sceneObjects[i].IsCollided = sceneObjects[k].IsCollided = true;
                    }
                    
                }
            }

            return collisions;
        }

        private static bool AreColliding(RigidShape first, RigidShape second)
        {
            return true;
        }
        
        private static bool AreColliding(RigidCircle first, RigidCircle second, List<CollisionInfo> collisions) // TODO сделать красиво
        {
            var vectorFromFirstToSecond = second.Center - first.Center; 
            var distance = vectorFromFirstToSecond.Length;
            var collisionDepth = first.Radius + second.Radius - distance;
            if (collisionDepth < 0)
                return false;
            if (collisionDepth != 0)
            {
                collisions.Add(new CollisionInfo(
                    collisionDepth,
                    vectorFromFirstToSecond.Normalize(),
                    first.Center + vectorFromFirstToSecond * (1 - second.Radius / distance)));
            }
            else
            {
                var maxRadius = Math.Max(first.Radius, second.Radius);
                collisions.Add(new CollisionInfo(
                    maxRadius,
                    new Vector(0, -1), 
                    first.Center + new Vector(0,1) * maxRadius));
            }
            return true;
        }
        
        private static bool AreColliding(RigidRectangle first, RigidRectangle second, List<CollisionInfo> collisions)
        {
            var firstClosestSupportPoint = FindClosestSupportPoint(first, second);
            if (firstClosestSupportPoint == null) return false;
            var secondClosestSupportPoint = FindClosestSupportPoint(second, first);
            if (secondClosestSupportPoint == null) return false;
            collisions.Add(SmallestCollision(firstClosestSupportPoint, secondClosestSupportPoint));
            return true;
        }

        private static CollisionInfo FindClosestSupportPoint(RigidRectangle first, RigidRectangle second)
        {
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
        }
        
        private static CollisionInfo FindSupportPoint(Vector negativeFaceNormal, Vector pointOnFace, RigidRectangle rectangle)
        {
            Vector supportPoint = null;
            var maxSupportDistance = 0f;
            foreach (var vertex in rectangle.Vertexes)
            {
                var vectorFromPointToVertex = vertex - pointOnFace;
                var projectionLength = Vector.ScalarProduct(negativeFaceNormal, vectorFromPointToVertex);
                if (projectionLength < maxSupportDistance) continue;
                supportPoint = vertex;
                maxSupportDistance = projectionLength;
            }

            return supportPoint == null
                ? null
                : new CollisionInfo(maxSupportDistance, negativeFaceNormal, supportPoint - maxSupportDistance * negativeFaceNormal);
        }

        private static CollisionInfo SmallestCollision(CollisionInfo first, CollisionInfo second)
        {
            return first.Depth < second.Depth ? first : second;
        }

        private static bool AreColliding(RigidRectangle first, RigidCircle second)
        {
            return true;
        }

        private static void ClearColliding(List<RigidShape> sceneObjects)
        {
            foreach (var sceneObject in sceneObjects)
                sceneObject.IsCollided = false;
        }
    }
}