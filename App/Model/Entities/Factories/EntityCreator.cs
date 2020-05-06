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
        private static Dictionary<string, Bitmap> cachedBitmaps;
        private static readonly Dictionary<Type, int> WeaponFramesId = new Dictionary<Type, int>
        {
            {typeof(AK303), 0},
            {typeof(Shotgun), 1},
            {typeof(MP6), 2},
            {typeof(SaigaFA), 3},
        };
        
        public static Player CreatePlayer(PlayerInitializationInfo info)
        {
            var meleeWeapon = new MeleeWeaponSprite(
                GetBitmap(info.MeleeWeaponTileMapPath),
                1, 0, 5, new Size(170, 170));
            
            var weaponSprites = new Dictionary<Type, Sprite>();
            foreach (var weaponName in WeaponFramesId.Keys)
            {
                weaponSprites.Add(
                    weaponName, 
                    new StaticSprite(
                        GetBitmap(info.WeaponsTileMapPath),
                        WeaponFramesId[weaponName], 
                        new Size(79, 57))); 
            }
            
            var collisionShape = new RigidCircle(info.Position, 32, false, true);

            return new Player(
                info.Position, info.Angle, 
                new PlayerBodySprite(info.Position, GetBitmap(info.ClothesTileMapPath),1, 14, 27, new Size(64, 64)),
                collisionShape, info.Weapons, weaponSprites, meleeWeapon);
        }

        public static Bot CreateBot(BotInitializationInfo info)
        {
            return null; //TODO
        }

        public static void Init()
        {
            cachedBitmaps = new Dictionary<string, Bitmap>();
        }

        private static Bitmap GetBitmap(string path)
        {
            if (cachedBitmaps.ContainsKey(path)) return cachedBitmaps[path];
            var newBitmap = new Bitmap(path);
            cachedBitmaps.Add(path, newBitmap);
            return newBitmap;
        }

        public class PlayerInitializationInfo : BotInitializationInfo
        {
            public readonly string MeleeWeaponTileMapPath;

            public PlayerInitializationInfo(
                Vector position, float angle, List<Weapon> weapons, 
                string clothesTileMapPath, string weaponsTileMapPath, string meleeWeaponTileMapPath)
                : base(position, angle, weapons, clothesTileMapPath, weaponsTileMapPath)
            {
                MeleeWeaponTileMapPath = meleeWeaponTileMapPath;
            }
        }

        public class BotInitializationInfo
        {
            public readonly Vector Position;
            public readonly float Angle;
            public readonly List<Weapon> Weapons;
            public readonly string ClothesTileMapPath;
            public readonly string WeaponsTileMapPath;
            
            public BotInitializationInfo(
                Vector position, float angle, List<Weapon> weapons, 
                string clothesTileMapPath, string weaponsTileMapPath)
            {
                Position = position;
                Angle = angle;
                Weapons = weapons;
                ClothesTileMapPath = clothesTileMapPath;
                WeaponsTileMapPath = weaponsTileMapPath;
            }
        }
    }
}