using System;
using System.Collections.Generic;
using System.Drawing;
using App.Engine;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using App.Engine.Sprites;
using App.Model.Entities.Weapons;

namespace App.Model.Entities.Factories
{
    public static class EntityCreator
    {
        private static Dictionary<string, Bitmap> cachedBitmaps = new Dictionary<string, Bitmap>();
        private static readonly Dictionary<Type, int> WeaponFramesId = new Dictionary<Type, int>
        {
            {typeof(AK303), 0},
            {typeof(Shotgun), 1},
            {typeof(MP6), 2},
            {typeof(SaigaFA), 3},
        };
        
        public static Player CreatePlayer(PlayerInfo info)
        {
            var meleeWeapon = new MeleeWeaponSprite(
                GetBitmap(info.MeleeWeaponTileMapPath),
                1, 0, 5, new Size(170, 170));
            
            var weaponSprites = new Dictionary<Type, Sprite>();
            foreach (var weaponType in WeaponFramesId.Keys)
            {
                weaponSprites.Add(
                    weaponType, new StaticSprite(
                        GetBitmap(info.WeaponsTileMapPath),
                        WeaponFramesId[weaponType], 
                        new Size(79, 57))); 
            }

            var weapons = new List<Weapon>();
            foreach (var weaponInfo in info.Weapons)
                weapons.Add(AbstractWeaponFactory.CreateGun(weaponInfo));

            return new Player(
                info.Health, info.Armor, info.Position, info.Angle, 
                new PlayerBodySprite(info.Position, GetBitmap(info.ClothesTileMapPath),1, 14, 27, new Size(64, 64)),
                new RigidCircle(info.Position, 32, false, true), 
                weapons, weaponSprites, meleeWeapon);
        }

        public static Bot CreateBot(BotInfo info)
        {
            return new Bot(
                info.Health, info.Armor, info.Position, info.Angle,
                new PlayerBodySprite(info.Position, GetBitmap(info.ClothesTileMapPath),1, 14, 27, new Size(64, 64)),
                new StaticSprite(GetBitmap(info.WeaponsTileMapPath), WeaponFramesId[info.weapon.WeaponType], new Size(79, 57)),
                new RigidCircle(info.Position, 32, false, true),
                AbstractWeaponFactory.CreateGun(info.weapon));
        }

        private static Bitmap GetBitmap(string path)
        {
            if (cachedBitmaps.ContainsKey(path)) return cachedBitmaps[path];
            var newBitmap = new Bitmap(path);
            cachedBitmaps.Add(path, newBitmap);
            return newBitmap;
        }

        public class PlayerInfo
        {
            public readonly int Health;
            public readonly int Armor;
            public readonly Vector Position;
            public readonly float Angle;
            public readonly List<WeaponInfo> Weapons;
            public readonly string ClothesTileMapPath;
            public readonly string WeaponsTileMapPath;
            public readonly string MeleeWeaponTileMapPath;

            public PlayerInfo(
                int health, int armor, Vector position, float angle, List<WeaponInfo> weapons, 
                string clothesTileMapPath, string weaponsTileMapPath, string meleeWeaponTileMapPath)
            {
                Health = health;
                Armor = armor;
                Position = position;
                Angle = angle;
                Weapons = weapons;
                ClothesTileMapPath = clothesTileMapPath;
                WeaponsTileMapPath = weaponsTileMapPath;
                MeleeWeaponTileMapPath = meleeWeaponTileMapPath;
            }
        }

        public class BotInfo
        {
            public readonly int Health;
            public readonly int Armor;
            public readonly Vector Position;
            public readonly float Angle;
            public readonly WeaponInfo weapon;
            public readonly string ClothesTileMapPath;
            public readonly string WeaponsTileMapPath;

            public BotInfo(
                int health, int armor, Vector position, float angle, WeaponInfo weapon, 
                string clothesTileMapPath, string weaponsTileMapPath)
            {
                Health = health;
                Armor = armor;
                Position = position;
                Angle = angle;
                this.weapon = weapon;
                ClothesTileMapPath = clothesTileMapPath;
                WeaponsTileMapPath = weaponsTileMapPath;
            }
        }
    }
}