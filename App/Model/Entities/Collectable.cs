using App.Engine.Physics.RigidShapes;

namespace App.Model.Entities
{
    public abstract class Collectable
    {
        public abstract RigidShape CollisionShape { get; }
        public abstract SpriteContainer SpriteContainer { get; }
        public abstract bool IsPicked { get; set; }
        public abstract void Pick(Player player);
    }
}