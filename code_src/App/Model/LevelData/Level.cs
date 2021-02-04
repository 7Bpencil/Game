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
        public readonly NavMesh NavMesh;
        public readonly List<Vector> BotSpawnPoints;
        public readonly List<Vector> BotPatrolPoints;
        public int WavesAmount;
        public Player Player { get; private set; }
        public List<Bot> Bots { get; private set; }
        public List<Collectable> Collectables { get; private set; }
        public List<Bullet> Bullets { get; private set; }
        public List<AbstractParticleUnit> Particles { get; private set; }
        public List<SpriteContainer> Sprites { get; private set; }
        public List<CollisionInfo> CollisionsInfo;
        public List<Raytracing.VisibilityRegion> VisibilityRegions;
        public List<List<Vector>> Paths;
        public bool IsCompleted;

        public Level(LevelInfo levelInfo)
        {
            this.levelInfo = levelInfo;
            LevelSizeInTiles = levelInfo.LevelSizeInTiles;
            Name = levelInfo.Name;

            Exit = levelInfo.Exit;
            StaticShapes = levelInfo.StaticShapes;
            RaytracingEdges = levelInfo.RaytracingEdges;
            NavMesh = levelInfo.NavMesh;
            BotSpawnPoints = levelInfo.BotSpawnPoints;
            BotPatrolPoints = levelInfo.BotPatrolPoints;

            SetDynamicEntities();
        }

        private void SetDynamicEntities()
        {
            DynamicShapes = new List<RigidShape> {Capacity = 50};
            SceneShapes = new ShapesIterator(StaticShapes, DynamicShapes);
            WavesAmount = levelInfo.WavesAmount;

            Player = LevelDynamicEntitiesFactory.CreatePlayer(levelInfo.PlayerInfo);
            Bots = LevelDynamicEntitiesFactory.CreateBots(levelInfo.BotsInfo, levelInfo.BotPatrolPoints);
            Collectables = LevelDynamicEntitiesFactory.CreateCollectables(levelInfo.CollectableWeaponsInfo);
            Bullets = new List<Bullet> {Capacity = 700};
            Particles = new List<AbstractParticleUnit> {Capacity = 700};
            Sprites = new List<SpriteContainer> {Capacity = 50};
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

        public void TryOptimize()
        {
            if (Particles.Count > 600)
            {
                var newParticles = new List<AbstractParticleUnit> {Capacity = 700};
                foreach (var particle in Particles)
                    if (!particle.IsExpired) newParticles.Add(particle);
                Particles = newParticles;
            }
            if (Bullets.Count > 600)
            {
                var newBullets = new List<Bullet> {Capacity = 700};
                foreach (var bullet in Bullets)
                    if (!bullet.IsStuck) newBullets.Add(bullet);
                Bullets = newBullets;
            }
            if (Collectables.Count > 20)
            {
                var newCollectables = new List<Collectable> {Capacity = 30};
                foreach (var collectable in Collectables)
                    if (!collectable.IsPicked) newCollectables.Add(collectable);
                Collectables = newCollectables;
            }
        }
    }
}
