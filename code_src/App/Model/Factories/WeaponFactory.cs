using System.Drawing;
using App.Engine.Physics;
using App.Model.Entities;
using App.Model.Entities.Collectables;

namespace App.Model.Factories
{
    public abstract class WeaponFactory
    {
        public abstract Weapon CreateGun(int ammoAmount);
        public abstract CollectableWeapon CreateCollectable(CollectableWeaponInfo info);
        public abstract CollectableWeapon CreateRuntimeCollectable(Vector position, int ammo, int angle);
        public abstract Bitmap GetHUDicon();
    }
}