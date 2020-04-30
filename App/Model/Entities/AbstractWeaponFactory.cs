using System.Drawing;
using App.Model.Entities.Weapons;

namespace App.Model.Entities
{
    public static class AbstractWeaponFactory
    {
        public static WeaponFactory<AK303> CreateAK303factory()
        {
            return new WeaponFactory<AK303>
            (new Bitmap(@"Assets\Sprites\Weapons\AK303_hud.png"),
                new Bitmap(@"Assets\Sprites\Weapons\AK303_icon.png"));
        }
        
        public static WeaponFactory<MP6> CreateMP6factory()
        {
            return new WeaponFactory<MP6>
            (new Bitmap(@"Assets\Sprites\Weapons\MP6_hud.png"),
                new Bitmap(@"Assets\Sprites\Weapons\MP6_icon.png"));
        }
        
        public static WeaponFactory<SaigaFA> CreateSaigaFAfactory()
        {
            return new WeaponFactory<SaigaFA>
            (new Bitmap(@"Assets\Sprites\Weapons\SaigaFA_hud.png"),
                new Bitmap(@"Assets\Sprites\Weapons\SaigaFA_icon.png"));
        }
        
        public static WeaponFactory<Shotgun> CreateShotgunFactory()
        {
            return new WeaponFactory<Shotgun>
            (new Bitmap(@"Assets\Sprites\Weapons\Shotgun_hud.png"),
                new Bitmap(@"Assets\Sprites\Weapons\Shotgun_icon.png"));
        }
    }
}