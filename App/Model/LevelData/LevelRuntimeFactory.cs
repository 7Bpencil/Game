using System.Collections.Generic;
using App.Model.Entities;
using App.Model.Entities.Collectables;
using App.Model.Entities.Factories;

namespace App.Model.LevelData
{
    public static class LevelRuntimeFactory
    {
        public static Player CreatePlayer(EntityCreator.PlayerInitializationInfo playerInfo)
        {
            return EntityCreator.CreatePlayer(playerInfo);
        }

        public static List<Bot> CreateBots(List<EntityCreator.BotInitializationInfo> botsInfo)
        {
            var bots = new List<Bot>();
            foreach (var info in botsInfo)
                bots.Add(EntityCreator.CreateBot(info));
            return bots;
        }

        public static List<Collectable> CreateCollectables(List<CollectableWeaponInitializationInfo> collectablesInfo)
        {
            var collectables = new List<Collectable>();
            foreach (var info in collectablesInfo)
                collectables.Add(AbstractWeaponFactory.CreateCollectable(info));
            return collectables;
        }
    }
}