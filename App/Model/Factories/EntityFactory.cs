using System;
using System.Collections.Generic;
using System.Drawing;
using App.Engine;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using App.Engine.Sprites;
using App.Model.Entities;
using App.Model.Entities.Weapons;

namespace App.Model.Factories
{
    public static class EntityFactory
    {
        private static readonly Dictionary<string, Bitmap> CachedBitmaps = new Dictionary<string, Bitmap>();
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

            var position = info.Position.Copy();
            return new Player(
                info.Health, info.Armor, position, info.Angle, 
                new PlayerBodySprite(position, GetBitmap(info.ClothesTileMapPath),1, 14, 27, new Size(64, 64)),
                new RigidCircle(position, 32, false, true), 
                weapons, weaponSprites, meleeWeapon);
        }

        public static Bot CreateBot(BotInfo info)
        {
            var position = info.Position.Copy();
            var type = BotBank.GetBotTypeInfo(info.Type);
            return new Bot(
                type.Health, type.Armor, position, info.Angle,
                new PlayerBodySprite(position, GetBitmap(type.ClothesTileMapPath),1, 14, 27, new Size(64, 64)),
                new StaticSprite(GetBitmap(type.WeaponsTileMapPath), WeaponFramesId[type.Weapon.WeaponType], new Size(79, 57)),
                new RigidCircle(position, 32, false, true),
                AbstractWeaponFactory.CreateGun(type.Weapon));
        }

        private static Bitmap GetBitmap(string path)
        {
            if (CachedBitmaps.ContainsKey(path)) return CachedBitmaps[path];
            var newBitmap = new Bitmap(path);
            CachedBitmaps.Add(path, newBitmap);
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
            public readonly Vector Position;
            public readonly float Angle;
            public readonly string Type;

            public BotInfo(Vector position, float angle, string type)
            {
                Position = position;
                Angle = angle;
                Type = type;
            }
        }

        public class BotType
        {
            public readonly int Health;
            public readonly int Armor;
            public readonly WeaponInfo Weapon;
            public readonly string ClothesTileMapPath;
            public readonly string WeaponsTileMapPath;

            public BotType(int health, int armor, WeaponInfo weapon, string clothesTileMapPath, string weaponsTileMapPath)
            {
                Health = health;
                Armor = armor;
                Weapon = weapon;
                ClothesTileMapPath = clothesTileMapPath;
                WeaponsTileMapPath = weaponsTileMapPath;
            }
        }
    }
}