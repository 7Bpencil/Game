using App.Engine.Physics.RigidShapes;

namespace App.Model.Entities
{
    public class Collectable<T>
    {
        public readonly RigidShape CollisionShape;
        private readonly T item;
        public readonly SpriteContainer SpriteContainer;
        public bool IsPicked;

        public Collectable(T item, RigidShape collisionShape, SpriteContainer iconContainer)
        {
            CollisionShape = collisionShape;
            this.item = item;
            SpriteContainer = iconContainer;
            IsPicked = false;
        }

        public T GetItem()
        {
            IsPicked = true;
            return item;
        }
    }
}