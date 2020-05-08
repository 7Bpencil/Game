using System.Collections.Generic;
using App.Engine.Physics.RigidShapes;
using App.Model.LevelData;

namespace App.Engine.Physics.Collision
{
    public class CollisionSolver
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
                    
                    var collisionInfo = CollisionDetector.GetCollisionInfo(sceneObjects[i], sceneObjects[k]);
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
        
        private static void ClearColliding(ShapesIterator sceneObjects)
        {
            for (var i = 0; i < sceneObjects.Length; i++)
                sceneObjects[i].IsCollided = false;
        }
    }
}