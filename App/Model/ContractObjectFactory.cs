using System.Collections.Generic;
using App.Engine.PhysicsEngine.RigidBody;

namespace App.Model
{
    public abstract class ContractObjectFactory
    {
        public abstract List<RigidShape> GetSceneObjects(out RigidShape player, out RigidShape playerCenter, out RigidShape cursor,
            int windowWidth, int windowHeight);
    }
}