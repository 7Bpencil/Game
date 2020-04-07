using System.Collections.Generic;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.RigidBody;

namespace App.Model
{
    public class ObjectFactory : ContractObjectFactory
    {
        public override List<RigidShape> GetSceneObjects(out RigidShape player, out RigidShape cursor, 
            int windowWidth, int windowHeight)
        {
            const float playerRadius = 50;
            var positionPlayerCenter = (new Vector(windowWidth, windowHeight)
                                        + new Vector(playerRadius, playerRadius)) / 2;

            player = new RigidRectangle(positionPlayerCenter, playerRadius, playerRadius, 0, true);
            cursor = new RigidCircle(positionPlayerCenter, 5, false);
            return new List<RigidShape>
            {
                new RigidRectangle(new Vector(250, 450), 190, 100, -45, true),
                new RigidRectangle(new Vector(440, 110), 160, 100, -17, true),
                new RigidRectangle(new Vector(250, 150), 280, 110, 40, true),
                new RigidRectangle(new Vector(650, 350), 320, 150, 15, true),
                new RigidCircle(new Vector(100, 100), 35, true),
                cursor,
                player,
            };
        }
    }
}