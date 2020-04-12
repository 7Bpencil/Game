using System.Collections.Generic;
using App.Engine.PhysicsEngine.RigidBody;

namespace App.Model.LevelData
{
    public class Tile
    {
        public List<RigidShape> collisionShapes;

        public Tile(List<RigidShape> collisionShapes)
        {
            this.collisionShapes = collisionShapes;
        }
    }
}