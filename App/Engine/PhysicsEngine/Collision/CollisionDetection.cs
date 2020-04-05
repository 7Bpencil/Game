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
                    if (sceneObjects[i].CanCollide
                        && sceneObjects[k].CanCollide
                        && sceneObjects[i] is RigidCircle 
                        && sceneObjects[k] is RigidCircle 
                        && AreColliding((RigidCircle) sceneObjects[i], (RigidCircle) sceneObjects[k], collisions))
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
                    vectorFromFirstToSecond / distance * first.Radius));
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
        
        private static bool AreColliding(RigidRectangle first, RigidRectangle second)
        {
            return true;
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