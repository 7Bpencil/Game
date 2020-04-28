using App.Engine.Physics.RigidShapes;

namespace App.Model.Entities
{
    public class Collectable
    {
        private readonly RigidShape collisionShape;
        private readonly Weapon item;

        public Collectable(RigidShape collisionShape, Weapon item)
        {
            this.collisionShape = collisionShape;
            this.item = item;
        }
    }
}