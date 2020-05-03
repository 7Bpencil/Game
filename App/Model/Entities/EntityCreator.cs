using System;
using System.Collections.Generic;
using System.Drawing;
using App.Engine;
using App.Engine.Physics;
using App.Engine.Sprites;
using App.Model.Entities.Weapons;
using App.Model.LevelData;

namespace App.Model.Entities
{
    public static class EntityCreator
    {
        private static readonly Dictionary<Type, int> WeaponFramesId = new Dictionary<Type, int>
        {
            {typeof(AK303), 0},
            {typeof(Shotgun), 1},
            {typeof(MP6), 2},
            {typeof(SaigaFA), 3},
        };
        
        public static Player CreatePlayer(Vector startPosition, float angle, List<Weapon> startWeapons, Level currentLevel)
        {
            var weaponSprites = new Dictionary<Type, Sprite>();
            foreach (var weaponName in WeaponFramesId.Keys)
            {
                weaponSprites.Add(weaponName, 
                    new StaticSprite(
                        currentLevel.PlayerWeaponsTileMap,
                        WeaponFramesId[weaponName], 
                        new Size(79, 57))); 
            }

            return new Player(
                startPosition,
                angle,
                new PlayerBodySprite(
                    startPosition, currentLevel.PlayerClothesTileMap,1, 14, 27,
                    new Size(64, 64)),
                startWeapons,
                weaponSprites);
        }
    }
}