using System.Collections.Generic;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using App.Model.Entities;
using App.Model.Entities.Collectables;

namespace App.Model.Factories
{
    public static class LevelDynamicEntitiesFactory
    {
        public static Player CreatePlayer(EntityFactory.PlayerInfo playerInfo)
        {
            return EntityFactory.CreatePlayer(playerInfo);
        }

        public static List<Bot> CreateBots(List<EntityFactory.BotInfo> botsInfo)
        {
            var bots = new List<Bot>();
            foreach (var info in botsInfo)
                bots.Add(EntityFactory.CreateBot(info));
            return bots;
        }

        public static void SpawnBots(
            List<Vector> spawnPositions, Vector playerPosition, List<Bot> levelBots, 
            List<SpriteContainer> levelSprites, List<RigidShape> levelDynamicShapes)
        {
            var xAxisVector = new Vector(1, 0);
            foreach (var position in spawnPositions)
            {
                var angle = Vector.GetAngle(xAxisVector, playerPosition - position);
                var newBot = EntityFactory.CreateBot(new EntityFactory.BotInfo(position, angle, BotBank.GetRandomBotType()));
                levelBots.Add(newBot);
                levelDynamicShapes.Add(newBot.CollisionShape);
                levelSprites.Add(newBot.LegsContainer);
                levelSprites.Add(newBot.TorsoContainer);
            }
        }

        public static List<Collectable> CreateCollectables(List<CollectableWeaponInfo> collectablesInfo)
        {
            var collectables = new List<Collectable>();
            foreach (var info in collectablesInfo)
                collectables.Add(AbstractWeaponFactory.CreateCollectable(info));
            return collectables;
        }
    }
}