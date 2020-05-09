using System.Collections.Generic;
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

        public static List<Collectable> CreateCollectables(List<CollectableWeaponInfo> collectablesInfo)
        {
            var collectables = new List<Collectable>();
            foreach (var info in collectablesInfo)
                collectables.Add(AbstractWeaponFactory.CreateCollectable(info));
            return collectables;
        }
    }
}