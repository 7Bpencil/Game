using System.Collections.Generic;
using System.Xml;
using App.Model.Entities;
using App.Model.Factories;

namespace App.Model.DataParser
{
    public static class BotTypesParser
    {
        public static Dictionary<string, EntityFactory.BotType> LoadBotTypes()
        {
            var botTypes = new Dictionary<string, EntityFactory.BotType>();

            var doc = new XmlDocument();
            doc.Load(@"assets\BotBank.xml");
            var root = doc.DocumentElement;

            foreach (XmlNode node in root)
            {
                if (node.Name != "type") continue;
                var typeName = node.Attributes.GetNamedItem("name").Value;
                botTypes.Add(typeName, LoadType(node));
            }

            return botTypes;
        }

        private static EntityFactory.BotType LoadType(XmlNode typeNode)
        {
            var health = int.Parse(typeNode.Attributes.GetNamedItem("health").Value);
            var armor = int.Parse(typeNode.Attributes.GetNamedItem("armor").Value);
            var clothesTileMapPath = typeNode.Attributes.GetNamedItem("clothesTileMap").Value;
            var weaponsTileMapPath = typeNode.Attributes.GetNamedItem("weaponsTileMap").Value;
            var deadBodyTileMapPath = typeNode.Attributes.GetNamedItem("deathTileMap").Value;
            WeaponInfo weapon = null;
            foreach (XmlNode node in typeNode.ChildNodes)
            {
                if (node.Name == "weapon") weapon = LevelParser.LoadWeapon(node);
            }

            return new EntityFactory.BotType(health, armor, weapon, clothesTileMapPath, weaponsTileMapPath, deadBodyTileMapPath);
        }
    }
}
