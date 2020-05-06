using System.Drawing;
using App.Model.Entities.Collectables;

namespace App.Model.Entities.Factories
{
    public abstract class WeaponFactory
    {
        public abstract Weapon CreateGun(int ammoAmount);
        public abstract CollectableWeapon CreateCollectable(CollectableWeaponInitializationInfo info);
        public abstract Bitmap GetHUDicon();
    }
}