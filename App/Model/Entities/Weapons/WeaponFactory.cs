using System.Drawing;
using System.Reflection;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using App.Engine.Sprites;

namespace App.Model.Entities.Weapons
{
    public class WeaponFactory<TW> where TW : Weapon
    {
        private readonly Bitmap HUDicon;
        private readonly Bitmap CollectableIcon;
        private readonly ConstructorInfo Ctor;

        public WeaponFactory(Bitmap hudIcon, Bitmap collectableIcon)
        {
            HUDicon = hudIcon;
            CollectableIcon = collectableIcon;
            Ctor = typeof(TW).GetConstructor(new[] {typeof(int)});
        }
        
        public TW GetNewGun(int ammoAmount)
        {
            return (TW) Ctor.Invoke(new object[] {ammoAmount});
        }

        public Collectable GetCollectable(Vector position, float angle, int ammoAmount)
        {
            return new Collectable(
                GetNewGun(ammoAmount),
                new RigidCircle(position, 40, true, true),
                new SpriteContainer(
                    new StaticSprite(CollectableIcon,0, CollectableIcon.Size, 1), position, angle));
        }

        public Bitmap GetHUDicon()
        {
            return HUDicon;
        }
    }
}