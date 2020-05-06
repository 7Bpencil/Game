using System.Collections.Generic;
using App.Engine.Particles;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidShapes;
using App.Model.Entities;

namespace App.Model.LevelData
{
    public class LevelRuntimeState
    {
        public readonly Player player;
        public readonly ShapesIterator SceneShapes;
        public readonly List<RigidShape> DynamicShapes;
        public readonly List<Collectable> Collectables;
        public readonly List<SpriteContainer> Sprites;
        public readonly List<AbstractParticleUnit> Particles;
        public readonly List<CollisionInfo> CollisionInfo;
        public readonly List<Bullet> Bullets;
        public readonly List<Bot> Bots;

        private bool isLevelLoaded;

        public LevelRuntimeState(Level levelInfo)
        {
            Bullets = new List<Bullet> {Capacity = 1000};
            Particles = new List<AbstractParticleUnit> {Capacity = 1000};
            Sprites = new List<SpriteContainer> {Capacity = 100};

            Collectables = LevelRuntimeFactory.CreateCollectables(levelInfo.CollectableWeaponsInfo);
            Bots = LevelRuntimeFactory.CreateBots(levelInfo.BotsInfo);
            player = LevelRuntimeFactory.CreatePlayer(levelInfo.PlayerInfo);
            HookUpSprites();
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
            Sprites.Add(player.LegsContainer);
            Sprites.Add(player.TorsoContainer);
        }
    }
}