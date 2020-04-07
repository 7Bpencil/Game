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
                    //if (!(sceneObjects[i].CanCollide && sceneObjects[k].CanCollide)) continue;
                    
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
            var a = FindSmallestSupportPoint(first, second);
            if (a == null) return false;
            var b = FindSmallestSupportPoint(second, first);
            if (b == null) return false;
            collisions.Add(SmallestCollision(a,b));
            return true;
        }

        private static CollisionInfo FindSmallestSupportPoint(RigidRectangle first, RigidRectangle second)
        {
            CollisionInfo collision = null;
            var minCollisionDepth = float.PositiveInfinity;
            var hasSupportPoint = true;
            for (var i = 0; i < second.FaceNormals.Length && hasSupportPoint; i++)
            {
                var collisionInfo = FindSupportPoint(-1 * first.FaceNormals[i], first.Vertexes[i], second);
                hasSupportPoint = collisionInfo != null;
                if (hasSupportPoint && collisionInfo.Depth < minCollisionDepth)
                {
                    minCollisionDepth = collisionInfo.Depth;
                    collision = collisionInfo;
                }
            }

            return hasSupportPoint ? collision : null;
        }
        
        private static CollisionInfo FindSupportPoint(Vector negativeFaceNormal, Vector pointOnFace, RigidRectangle rectangle)
        {
            var maxSupportDist = 0f;
            Vector supportPoint = null;
            foreach (var vertex in rectangle.Vertexes)
            {
                var projection = Vector.ScalarProduct(negativeFaceNormal, vertex - pointOnFace);
                if (projection > 0f && projection > maxSupportDist)
                {
                    maxSupportDist = projection;
                    supportPoint = vertex;
                }
            }

            return supportPoint == null
                ? null
                : new CollisionInfo(maxSupportDist, negativeFaceNormal, supportPoint - negativeFaceNormal * maxSupportDist);
        }

        private static CollisionInfo SmallestCollision(CollisionInfo first, CollisionInfo second)
        {
            return first.Depth >= second.Depth ? first : second;
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