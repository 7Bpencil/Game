using System;
using System.Collections.Generic;
using System.Drawing;
using App.Model.Entities;
using App.Model.Entities.Collectables;
using App.Model.Entities.Weapons;

namespace App.Model.Factories
{
    public static class AbstractWeaponFactory
    {
        private static Dictionary<Type, WeaponFactory> factories;

        public static void Initialize()
        {
            factories = new Dictionary<Type, WeaponFactory>
            {
                {typeof(AK303), CreateAK303factory()},
                {typeof(Shotgun), CreateShotgunFactory()},
                {typeof(SaigaFA), CreateSaigaFAfactory()},
                {typeof(MP6), CreateMP6factory()}
            };
        }

        public static Weapon CreateGun(WeaponInfo info)
        {
            if (factories == null || !factories.ContainsKey(info.WeaponType)) throw new ArgumentException();
            return factories[info.WeaponType].CreateGun(info.AmmoAmount);
        }

        public static Collectable CreateCollectable(CollectableWeaponInfo info)
        {
            if (factories == null || !factories.ContainsKey(info.WeaponInfo.WeaponType)) throw new ArgumentException();
            return factories[info.WeaponInfo.WeaponType].CreateCollectable(info);
        }
        
        public static Bitmap GetHUDicon(Type weaponType)
        {
            if (factories == null || !factories.ContainsKey(weaponType)) throw new ArgumentException();
            return factories[weaponType].GetHUDicon();
        }
        
        private static GenericWeaponFactory<AK303> CreateAK303factory()
        {
            return new GenericWeaponFactory<AK303>
            (new Bitmap(@"Assets\Sprites\Weapons\AK303_hud.png"),
                new Bitmap(@"Assets\Sprites\Weapons\AK303_icon.png"));
        }
        
        private static GenericWeaponFactory<MP6> CreateMP6factory()
        {
            return new GenericWeaponFactory<MP6>
            (new Bitmap(@"Assets\Sprites\Weapons\MP6_hud.png"),
                new Bitmap(@"Assets\Sprites\Weapons\MP6_icon.png"));
        }
        
        private static GenericWeaponFactory<SaigaFA> CreateSaigaFAfactory()
        {
            return new GenericWeaponFactory<SaigaFA>
            (new Bitmap(@"Assets\Sprites\Weapons\SaigaFA_hud.png"),
                new Bitmap(@"Assets\Sprites\Weapons\SaigaFA_icon.png"));
        }
        
        private static GenericWeaponFactory<Shotgun> CreateShotgunFactory()
        {
            return new GenericWeaponFactory<Shotgun>
            (new Bitmap(@"Assets\Sprites\Weapons\Shotgun_hud.png"),
                new Bitmap(@"Assets\Sprites\Weapons\Shotgun_icon.png"));
        }
    }
}