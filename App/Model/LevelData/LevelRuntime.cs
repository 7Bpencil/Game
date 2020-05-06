using System.Collections.Generic;
using System.Drawing;
using App.Engine.Particles;
using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidShapes;
using App.Model.Entities;

namespace App.Model.LevelData
{
    public class LevelRuntime
    {
        public readonly Size LevelSizeInTiles;
        public readonly string Name;

        public readonly RigidShape Exit;
        public readonly List<RigidShape> StaticShapes;
        public readonly List<RigidShape> DynamicShapes;
        public readonly ShapesIterator SceneShapes;
        public readonly List<Edge> RaytracingEdges;
        
        public readonly Bitmap LevelMap;
        
        public readonly Player Player;
        public readonly List<Bot> Bots;
        public readonly List<Collectable> Collectables;
        public readonly List<Bullet> Bullets;
        public readonly List<AbstractParticleUnit> Particles;
        public readonly List<SpriteContainer> Sprites;
        public List<CollisionInfo> CollisionsInfo;
        

        private bool isLevelLoaded;

        public LevelRuntime(Level levelInfo)
        {
            LevelSizeInTiles = levelInfo.LevelSizeInTiles;
            Name = levelInfo.Name;

            Exit = levelInfo.Exit;
            StaticShapes = levelInfo.StaticShapes;
            DynamicShapes = new List<RigidShape> {Capacity = 50};
            SceneShapes = new ShapesIterator(StaticShapes, DynamicShapes);
            RaytracingEdges = levelInfo.RaytracingEdges;
            
            LevelMap = (Bitmap) levelInfo.LevelMap.Clone();
            
            Player = LevelRuntimeFactory.CreatePlayer(levelInfo.PlayerInfo);
            Bots = LevelRuntimeFactory.CreateBots(levelInfo.BotsInfo);
            Collectables = LevelRuntimeFactory.CreateCollectables(levelInfo.CollectableWeaponsInfo);
            Bullets = new List<Bullet> {Capacity = 1000};
            Particles = new List<AbstractParticleUnit> {Capacity = 1000};
            Sprites = new List<SpriteContainer> {Capacity = 100};

            HookUpSprites();
            HookUpCollisions();

            isLevelLoaded = true;
        }

        private void HookUpSprites()
        {
            foreach (var item in Collectables)
                Sprites.Add(item.SpriteContainer);
            foreach (var bot in Bots)
            {
                Sprites.Add(bot.LegsContainer);
                Sprites.Add(bot.TorsoContainer);
            }
            
            Sprites.Add(Player.LegsContainer);
            Sprites.Add(Player.TorsoContainer);
        }

        private void HookUpCollisions()
        {
            foreach (var bot in Bots)
                DynamicShapes.Add(bot.CollisionShape);
            DynamicShapes.Add(Player.CollisionShape);
        }
    }
}