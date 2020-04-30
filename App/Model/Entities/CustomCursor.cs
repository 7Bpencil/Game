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

        public CustomCursor(RigidCircle shape, Sprite sprite)
        {
            this.shape = shape;
            SpriteContainer = new SpriteContainer(sprite);
        }
        
        public void MoveBy(Vector delta) => shape.MoveBy(delta);
    }
}