using System.Collections.Generic;
using System.Drawing;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.RigidBody;

namespace App.Model
{
    public class ObjectFactory : ContractObjectFactory
    {
        public override List<RigidShape> GetSceneObjects(out RigidShape player, out RigidShape playerCenter, out RigidShape cursor, 
            int windowWidth, int windowHeight)
        {
            var standardStrokePen = new Pen(Color.Crimson, 3);
            const float playerWidth = 50;
            const float playerHeight = 50;
            var positionPlayerCenter = (new Vector(windowWidth, windowHeight)
                                        + new Vector(playerWidth, playerHeight)) / 2;

            player = new RigidRectangle(positionPlayerCenter, playerWidth, playerHeight, 45, standardStrokePen);
            playerCenter = new RigidRectangle(positionPlayerCenter, 10, 10, 45, standardStrokePen);
            cursor = new RigidCircle(positionPlayerCenter, 5, standardStrokePen);
            return new List<RigidShape>
            {
                new RigidRectangle(new Vector(250, 450), 190, 100, -45, standardStrokePen),
                new RigidRectangle(new Vector(440, 110), 160, 100, -17, standardStrokePen),
                new RigidRectangle(new Vector(250, 150), 280, 110, 40, standardStrokePen),
                new RigidRectangle(new Vector(650, 350), 320, 150, 15, standardStrokePen),
                new RigidCircle(new Vector(100, 100), 35, standardStrokePen),
                cursor,
                player,
                playerCenter,
            };
        }
    }
}