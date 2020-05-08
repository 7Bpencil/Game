using System.Drawing;
using System.Reflection;
using App.Engine.Physics.RigidShapes;
using App.Engine.Sprites;
using App.Model.Entities;
using App.Model.Entities.Collectables;

namespace App.Model.Factories
{
    public class GenericWeaponFactory<TW> : WeaponFactory where TW : Weapon
    {
        private readonly Bitmap HUDicon;
        private readonly Bitmap collectableIcon;
        private readonly ConstructorInfo ctor;

        public GenericWeaponFactory(Bitmap hudIcon, Bitmap collectableIcon)
        {
            HUDicon = hudIcon;
            this.collectableIcon = collectableIcon;
            ctor = typeof(TW).GetConstructor(new[] {typeof(int)});
        }
        
        public override Weapon CreateGun(int ammoAmount)
        {
            return (TW) ctor.Invoke(new object[] {ammoAmount});
        }

        public override CollectableWeapon CreateCollectable(CollectableWeaponInfo info)
        {
            var position = info.Position.Copy();
            return new CollectableWeapon(
                CreateGun(info.WeaponInfo.AmmoAmount),
                new RigidCircle(position, 40, true, true),
                new SpriteContainer(
                    new StaticSprite(collectableIcon,0, collectableIcon.Size), position, info.Angle));
        }

        public override Bitmap GetHUDicon()
        {
            return HUDicon;
        }
    }
}