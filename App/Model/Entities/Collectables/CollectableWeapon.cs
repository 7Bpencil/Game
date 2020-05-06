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
            player.AddWeapon(item);
        }
    }

    public class CollectableWeaponInitializationInfo
    {
        public readonly Type WeaponType;
        public readonly Vector Position;
        public readonly float Angle;
        public readonly int AmmoAmount;

        public CollectableWeaponInitializationInfo(Type weaponType, Vector position, float angle, int ammoAmount)
        {
            WeaponType = weaponType;
            Position = position;
            Angle = angle;
            AmmoAmount = ammoAmount;
        }
    }
}