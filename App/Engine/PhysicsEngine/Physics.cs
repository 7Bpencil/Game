using System.Collections.Generic;
using App.Engine.PhysicsEngine.RigidBody;

namespace App.Engine.PhysicsEngine
{
    public class Physics
    {
        public void CalculateCollisions(List<RigidShape> sceneObjects)
        {
            ClearColliding(sceneObjects);
            for (var i = 0; i < sceneObjects.Count; i++)
            {
                for (var k = i + 1; k < sceneObjects.Count; k++)
                {
                    if (sceneObjects[i] is RigidCircle 
                        && sceneObjects[k] is RigidCircle 
                        && AreColliding((RigidCircle) sceneObjects[i], (RigidCircle) sceneObjects[k]))
                    {
                        sceneObjects[i].IsCollided = sceneObjects[k].IsCollided = true;
                    }
                }
            }
        }

        private static bool AreColliding(RigidShape first, RigidShape second)
        {
            return true;
        }
        
        private static bool AreColliding(RigidCircle first, RigidCircle second)
        {
            var distance = (first.Center - second.Center).Length;
            var collisionLength = first.Radius + second.Radius - distance;
            return collisionLength > 0;
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