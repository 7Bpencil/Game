using System;
using System.Collections.Generic;
using System.Linq;
using App.Model.DataParser;

namespace App.Model.Factories
{
    public static class BotBank
    {
        private static Dictionary<string, EntityFactory.BotType> botTypes;
        private static Dictionary<string,EntityFactory.BotType>.KeyCollection keys;
        private static Random r;
        
        public static void Initialize()
        {
            botTypes = BotTypesParser.LoadBotTypes();
            keys = botTypes.Keys;
            r = new Random();
        }

        public static EntityFactory.BotType GetBotTypeInfo(string type)
        {
            return botTypes[type];
        }
        
        public static string GetRandomBotType()
        {
            return botTypes.ElementAt(r.Next(0, botTypes.Count)).Key;
        }
    }
}