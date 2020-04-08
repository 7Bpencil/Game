using System;
using System.Collections.Generic;
using System.Xml;

namespace App.Model.Parser
{
    public static class LevelParser
    {
        public static Level ParseLevel(string levelFilename)
        {
            var layers = new List<Layer>();
            var tileSet = new TileSet();

            var separators = new[] {"\r\n", ","};
            var doc = new XmlDocument();
            doc.Load(levelFilename);
            var xRoot = doc.DocumentElement;

            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Name == "tileset")
                    tileSet.Source = xnode.Attributes[1].Value;
                else if (xnode.Name == "layer")
                    layers.Add(ParseLayer(xnode, separators));
            }

            return new Level(tileSet, layers);
        }

        private static Layer ParseLayer(XmlNode xnode, string[] separators)
        {
            var newLayer = new Layer
            {
                Id = int.Parse(xnode.Attributes[0].Value),
                Name = xnode.Attributes[1].Value,
                Width = int.Parse(xnode.Attributes[2].Value),
                Height = int.Parse(xnode.Attributes[3].Value)
            };

            foreach (XmlNode childnode in xnode.ChildNodes)
            {
                if (childnode.Name != "data") continue;
                var layerData = childnode.InnerText.Split(separators, StringSplitOptions.None);
                newLayer.Tiles = ParseTiles(layerData, newLayer.Width * newLayer.Height);
            }

            return newLayer;
        }

        private static int[] ParseTiles(string[] layerData, int tilesAmount)
        {
            var newTiles = new int[tilesAmount];
            var k = 0;
            foreach (var tileIndex in layerData)
                if (tileIndex != "")
                    newTiles[k++] = int.Parse(tileIndex);
            return newTiles;
        }
    }
}