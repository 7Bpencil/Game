using System.Collections.Generic;
using System.Drawing;
using App.Engine;
using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidShapes;
using App.Engine.Render;
using App.Model.Entities;
using App.Model.Factories;

namespace App.Model.LevelData
{
    public class Level
    {
        private readonly LevelInfo levelInfo;
        
        public readonly Size LevelSizeInTiles;
        public readonly string Name;
        public readonly RigidAABB Exit;
        public readonly List<RigidShape> StaticShapes;
        public List<RigidShape> DynamicShapes { get; private set; }
        public ShapesIterator SceneShapes { get; private set; }
        public readonly List<Edge> RaytracingEdges;
        public Player Player { get; private set; }
        public List<Bot> Bots { get; private set; }
        public List<Collectable> Collectables { get; private set; }
        public List<Bullet> Bullets { get; private set; }
        public List<AbstractParticleUnit> Particles { get; private set; }
        public List<SpriteContainer> Sprites { get; private set; }
        public List<CollisionInfo> CollisionsInfo;
        public List<Raytracing.VisibilityRegion> VisibilityRegions;
        public bool IsCompleted;

        public Level(LevelInfo levelInfo)
        {
            this.levelInfo = levelInfo;
            LevelSizeInTiles = levelInfo.LevelSizeInTiles;
            Name = levelInfo.Name;

            Exit = levelInfo.Exit;
            StaticShapes = levelInfo.StaticShapes;
            RaytracingEdges = levelInfo.RaytracingEdges;

            SetDynamicEntities();
        }

        private void SetDynamicEntities()
        {
            DynamicShapes = new List<RigidShape> {Capacity = 50};
            SceneShapes = new ShapesIterator(StaticShapes, DynamicShapes);

            Player = LevelDynamicEntitiesFactory.CreatePlayer(levelInfo.PlayerInfo);
            Bots = LevelDynamicEntitiesFactory.CreateBots(levelInfo.BotsInfo);
            Collectables = LevelDynamicEntitiesFactory.CreateCollectables(levelInfo.CollectableWeaponsInfo);
            Bullets = new List<Bullet> {Capacity = 1000};
            Particles = new List<AbstractParticleUnit> {Capacity = 1000};
            Sprites = new List<SpriteContainer> {Capacity = 100};
            VisibilityRegions = new List<Raytracing.VisibilityRegion> {Capacity = 2};
            
            HookUpSprites();
            HookUpCollisions();
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
            DynamicShapes.AddRange(Player.MeleeWeapon.GetRangeShapes());
        }
        
        public void Reset()
        {
            SetDynamicEntities();
            RenderMachine.ResetLevelMap(levelInfo.LevelMap);
        }
    }
}