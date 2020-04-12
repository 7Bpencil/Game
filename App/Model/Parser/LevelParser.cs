/*using System;
using System.Collections.Generic;
using System.Xml;
using App.Model.LevelData;

namespace App.Model.Parser
{
    public static class LevelParser
    {
        public static Level ParseLevel(string levelFilename)
        {
            

            var separators = new[] {"\r\n", ","};
            var doc = new XmlDocument();
            doc.Load(levelFilename);
            var root = doc.DocumentElement;

            foreach (XmlNode node in root)
            {
                switch (node.Name)
                {
                    case "tileset":
                        tileSet.Source = node.Attributes[1].Value;
                        break;
                    case "layer":
                        layers.Add(ParseLayer(node, separators));
                        break;
                }
            }

            return new Level(tileSet, layers);
        }

        private static Layer ParseLayer(XmlNode node, string[] separators)
        {
            var newLayer = new Layer
            {
                Id = int.Parse(node.Attributes[0].Value),
                Name = node.Attributes[1].Value,
                Width = int.Parse(node.Attributes[2].Value),
                Height = int.Parse(node.Attributes[3].Value)
            };

            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name != "data") continue;
                var layerData = childNode.InnerText.Split(separators, StringSplitOptions.None);
                newLayer.Tiles = ParseTiles(layerData, newLayer.Width * newLayer.Height);
            }

            return newLayer;
        }

        private static int[] ParseTiles(string[] layerData, int tilesAmount)
        {
            var newTiles = new int[tilesAmount];
            var k = 0;
            foreach (var tileIndex in layerData)
                if (tileIndex != "") newTiles[k++] = int.Parse(tileIndex);
            return newTiles;
        }
    }
}*/