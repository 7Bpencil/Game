using System.Collections.Generic;
using App.Physics_Engine;
using App.Physics_Engine.RigidBody;

namespace App.Model
{
    public class ObjectManager
    {
        public ObjectManager()
        {
        }

        public List<RigidShape> GetSceneObjects(out RigidShape player, out RigidShape playerCenter, out RigidShape cursor, 
            int windowWidth, int windowHeight)
        {
            const float playerWidth = 50;
            const float playerHeight = 50;
            var positionPlayerCenter = (new Vector(windowWidth, windowHeight)
                                        + new Vector(playerWidth, playerHeight)) / 2;

            player = new RigidRectangle(positionPlayerCenter, playerWidth, playerHeight, 45);
            playerCenter = new RigidRectangle(positionPlayerCenter, 10, 10, 45);
            cursor = new RigidCircle(positionPlayerCenter, 5);
            return new List<RigidShape>
            {
                player,
                playerCenter,
                cursor,
                new RigidRectangle(new Vector(250, 450), 190, 100, -45),
                new RigidRectangle(new Vector(440, 110), 160, 100, -17),
                new RigidRectangle(new Vector(250, 150), 280, 110, 40),
                new RigidRectangle(new Vector(650, 350), 320, 150, 15),
                new RigidCircle(new Vector(100, 100), 35)
            };
        }
    }
}