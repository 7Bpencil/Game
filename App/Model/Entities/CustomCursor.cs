using App.Engine;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;

namespace App.Model.Entities
{
    public class CustomCursor
    {
        private readonly RigidCircle shape;
        public readonly SpriteContainer SpriteContainer;

        public Vector Position => shape.Center;

        public CustomCursor(Vector position, Sprite sprite)
        {
            shape = new RigidCircle(position, 3, false, true);
            SpriteContainer = new SpriteContainer(sprite, position, 0);
        }
        
        public void MoveBy(Vector delta) => shape.MoveBy(delta);
    }
}