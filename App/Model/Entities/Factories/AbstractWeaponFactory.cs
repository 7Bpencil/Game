using System;
using System.Collections.Generic;
using System.Drawing;
using App.Model.Entities.Collectables;
using App.Model.Entities.Weapons;

namespace App.Model.Entities.Factories
{
    public static class AbstractWeaponFactory
    {
        private static Dictionary<Type, AWF> factories;

        public static void Init()
        {
            factories = new Dictionary<Type, AWF>
            {
                {typeof(AK303), CreateAK303factory()},
                {typeof(Shotgun), CreateShotgunFactory()},
                {typeof(SaigaFA), CreateSaigaFAfactory()},
                {typeof(MP6), CreateMP6factory()}
            };
        }

        public static Weapon CreateGun(Type weaponType, int ammoAmount)
        {
            if (factories == null || !factories.ContainsKey(weaponType)) throw new ArgumentException();
            return factories[weaponType].CreateGun(ammoAmount);
        }

        public static Collectable CreateCollectable(CollectableWeaponInitializationInfo info)
        {
            if (factories == null || !factories.ContainsKey(info.WeaponType)) throw new ArgumentException();
            return factories[info.WeaponType].CreateCollectable(info);
        }
        
        public static Bitmap GetHUDicon(Type weaponType)
        {
            if (factories == null || !factories.ContainsKey(weaponType)) throw new ArgumentException();
            return factories[weaponType].GetHUDicon();
        }
        
        private static WeaponFactory<AK303> CreateAK303factory()
        {
            return new WeaponFactory<AK303>
            (new Bitmap(@"Assets\Sprites\Weapons\AK303_hud.png"),
                new Bitmap(@"Assets\Sprites\Weapons\AK303_icon.png"));
        }
        
        private static WeaponFactory<MP6> CreateMP6factory()
        {
            return new WeaponFactory<MP6>
            (new Bitmap(@"Assets\Sprites\Weapons\MP6_hud.png"),
                new Bitmap(@"Assets\Sprites\Weapons\MP6_icon.png"));
        }
        
        private static WeaponFactory<SaigaFA> CreateSaigaFAfactory()
        {
            return new WeaponFactory<SaigaFA>
            (new Bitmap(@"Assets\Sprites\Weapons\SaigaFA_hud.png"),
                new Bitmap(@"Assets\Sprites\Weapons\SaigaFA_icon.png"));
        }
        
        private static WeaponFactory<Shotgun> CreateShotgunFactory()
        {
            return new WeaponFactory<Shotgun>
            (new Bitmap(@"Assets\Sprites\Weapons\Shotgun_hud.png"),
                new Bitmap(@"Assets\Sprites\Weapons\Shotgun_icon.png"));
        }
    }
}