using System;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;

namespace App.Model.Entities.Collectables
{
    public class CollectableWeapon : Collectable
    {
        private readonly Weapon item;
        public override RigidShape CollisionShape { get; }
        public override SpriteContainer SpriteContainer { get; }
        public override bool IsPicked { get; set; }

        public CollectableWeapon(Weapon item, RigidShape collisionShape, SpriteContainer spriteContainer)
        {
            this.item = item;
            CollisionShape = collisionShape;
            SpriteContainer = spriteContainer;
        }
        
        public override void Pick(Player player)
        {
            IsPicked = true;
            SpriteContainer.ClearContent();
            player.AddWeapon(item);
        }
    }

    public class CollectableWeaponInfo
    {
        public readonly WeaponInfo WeaponInfo;
        public readonly Vector Position;
        public readonly float Angle;

        public CollectableWeaponInfo(WeaponInfo weaponInfo, Vector position, float angle)
        {
            WeaponInfo = weaponInfo;
            Position = position;
            Angle = angle;
        }
    }
}