using System.Collections.Generic;
using App.Physics_Engine.RigidBody;

namespace App.Model
{
    public abstract class ContractObjectManager
    {
        public abstract List<RigidShape> GetSceneObjects(out RigidShape player, out RigidShape playerCenter, out RigidShape cursor,
            int windowWidth, int windowHeight);
    }
}