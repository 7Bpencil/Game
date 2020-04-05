using System.Collections.Generic;
using App.Engine.PhysicsEngine.RigidBody;

namespace App.Engine.PhysicsEngine
{
    public class Physics
    {
        public void CalculateCollisions(List<RigidShape> sceneObjects)
        {
            for (var i = 0; i < sceneObjects.Count; i++)
            {
                for (var k = i + 1; k < sceneObjects.Count; k++)
                {
                    sceneObjects[i].IsCollided = sceneObjects[k].IsCollided = BoundTest(sceneObjects[i], sceneObjects[k]);
                }
            }
        }

        private static bool BoundTest(RigidShape first, RigidShape second)
        {
            return true;
        }
        
        private static bool BoundTest(RigidCircle first, RigidCircle second)
        {
            return true;
        }
        
        private static bool BoundTest(RigidRectangle first, RigidRectangle second)
        {
            return true;
        }
        
        private static bool BoundTest(RigidRectangle first, RigidCircle second)
        {
            return true;
        }
    }
}