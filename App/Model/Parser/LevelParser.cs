using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using App.Model.LevelData;

namespace App.Model.Parser
{
    public static class LevelParser
    {
        public static List<Level> LoadLevels()
        {
            var levelsFileNames = Directory.GetFiles("Assets/TileSets");
            var levels = new List<Level>();
            foreach (var fileName in levelsFileNames)
            {
                var level = ParseLevel(fileName);
                levels.Add(level);
            }

            return levels;
        }
        public static Level ParseLevel(string levelFilename)
        {
            var separators = new[] {"\r\n", ","};
            var doc = new XmlDocument();
            doc.Load(levelFilename);
            var root = doc.DocumentElement;
            var tileSetFirstgidFromSource = new Dictionary<string, int>();
            var layers = new List<Layer>();

            foreach (XmlNode node in root)
            {
                if (node.Name == "tileset")
                {
                    var source = node.Attributes.GetNamedItem("source").Value;
                    var firstgid = int.Parse(node.Attributes.GetNamedItem("firstgid").Value);
                    tileSetFirstgidFromSource.Add(source, firstgid);
                }

                if (node.Name == "layer")
                {
                    layers.Add(ParseLayer(node, separators));
                }
            }

            return null;
        }

        private static Layer ParseLayer(XmlNode node, string[] separators)
        {
            var newLayer = new Layer
            (
                int.Parse(node.Attributes[0].Value),
                node.Attributes[1].Value,
                int.Parse(node.Attributes[2].Value),
                int.Parse(node.Attributes[3].Value)
            );

            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name != "data") continue;
                var layerData = childNode.InnerText.Split(separators, StringSplitOptions.None);
                newLayer.Tiles = ParseTiles(layerData, newLayer.WidthInTiles * newLayer.HeightInTiles);
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
}