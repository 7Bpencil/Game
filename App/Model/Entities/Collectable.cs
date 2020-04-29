using App.Engine;
using App.Engine.Physics.RigidShapes;

namespace App.Model.Entities
{
    public class Collectable
    {
        public readonly RigidShape CollisionShape;
        public readonly Weapon Item;
        public readonly Sprite Icon;

        public Collectable(RigidShape collisionShape, Weapon item)
        {
            CollisionShape = collisionShape;
            Item = item;
            Icon = new Sprite(
                collisionShape.Center, item.CollectableIcon,
                0, 0, 0,
                item.CollectableIcon.Size, 1);
        }
    }
}