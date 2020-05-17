using System;
using System.Collections.Generic;
using System.Drawing;
using App.Engine;
using App.Engine.Particles;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using App.Engine.Sprites;
using App.Model.Entities;
using App.Model.Entities.Weapons;
using App.Model.LevelData;

namespace App.Model.Factories
{
    public static class EntityFactory
    {
        private static readonly Random r = new Random();
        private static readonly Dictionary<string, Bitmap> CachedBitmaps = new Dictionary<string, Bitmap>();
        private static readonly Dictionary<Type, int> WeaponFramesId = new Dictionary<Type, int>
        {
            {typeof(AK303), 0},
            {typeof(Shotgun), 1},
            {typeof(MP6), 2},
            {typeof(SaigaFA), 3},
            {typeof(GrenadeLauncher), 4},
        };

        public static Player CreatePlayer(PlayerInfo info)
        {
            var meleeWeaponSprite = new MeleeWeaponSprite(
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
            var angle = info.Angle;
            var legs = new PlayerBodySprite(position, GetBitmap(info.ClothesTileMapPath), 1, 14, 27, new Size(64, 64));
            var legsContainer = new SpriteContainer(legs, position, angle);
            var torsoContainer = new SpriteContainer(weaponSprites[weapons[0].GetType()], position, angle);
            var meleeWeapon = new MeleeWeapon(position, angle);

            return new Player(
                info.Health, info.Armor, new RigidCircle(position, 32, false, true), 
                legsContainer, torsoContainer, weapons, weaponSprites, meleeWeaponSprite, meleeWeapon, info.DeadBodyTileMapPath);
        }

        public static Bot CreateBot(BotInfo info, List<Vector> patrolPoints)
        {
            var position = info.Position.Copy();
            var angle = info.Angle;
            var type = BotBank.GetBotTypeInfo(info.Type);
            var legs = new PlayerBodySprite(position, GetBitmap(type.ClothesTileMapPath), 1, 14, 27, new Size(64, 64));
            var torso = new StaticSprite(GetBitmap(type.WeaponsTileMapPath), WeaponFramesId[type.Weapon.WeaponType], new Size(79, 57));
            var legsContainer = new SpriteContainer(legs, position, angle);
            var torsoContainer = new SpriteContainer(torso, position, angle);
            var sightVector = new Vector(1, 0).Rotate(-angle, Vector.ZeroVector).Normalize(); 
            return new Bot(
                type.Health, type.Armor, legsContainer, torsoContainer, sightVector,
                new RigidCircle(position, 32, false, true),
                AbstractWeaponFactory.CreateGun(type.Weapon), type.DeadBodyTileMapPath, patrolPoints);
        }

        private static Bitmap GetBitmap(string path)
        {
            if (CachedBitmaps.ContainsKey(path)) return CachedBitmaps[path];
            var newBitmap = new Bitmap(path);
            CachedBitmaps.Add(path, newBitmap);
            return newBitmap;
        }

        public static StaticParticle CreateDeadBody(string deadBodyPath)
        {
            return new StaticParticle(GetBitmap(deadBodyPath), r.Next(0, 4), new Size(135, 125));
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
            public readonly string DeadBodyTileMapPath;

            public PlayerInfo(
                int health, int armor, Vector position, float angle, List<WeaponInfo> weapons, 
                string clothesTileMapPath, string weaponsTileMapPath, string meleeWeaponTileMapPath, string deadBodyTileMapPath)
            {
                Health = health;
                Armor = armor;
                Position = position;
                Angle = angle;
                Weapons = weapons;
                ClothesTileMapPath = clothesTileMapPath;
                WeaponsTileMapPath = weaponsTileMapPath;
                MeleeWeaponTileMapPath = meleeWeaponTileMapPath;
                DeadBodyTileMapPath = deadBodyTileMapPath;
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
            public readonly string DeadBodyTileMapPath;

            public BotType(
                int health, int armor, WeaponInfo weapon, 
                string clothesTileMapPath, string weaponsTileMapPath, string deadBodyTileMapPath)
            {
                Health = health;
                Armor = armor;
                Weapon = weapon;
                ClothesTileMapPath = clothesTileMapPath;
                WeaponsTileMapPath = weaponsTileMapPath;
                DeadBodyTileMapPath = deadBodyTileMapPath;
            }
        }
    }
}