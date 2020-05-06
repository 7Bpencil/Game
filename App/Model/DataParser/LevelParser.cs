using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using App.Model.Entities;
using App.Model.LevelData;

namespace App.Model.DataParser
{
    public static class LevelParser
    {
        public static Dictionary<int, string> LoadLevelList()
        {
            var levels = new Dictionary<int, string>();
            
            var doc = new XmlDocument();
            doc.Load(@"Assets\Levels\LevelList.xml");
            var root = doc.DocumentElement;

            foreach (XmlNode node in root)
            {
                if (node.Name != "level") continue;
                levels.Add(
                    int.Parse(node.Attributes.GetNamedItem("id").Value), 
                    node.Attributes.GetNamedItem("source").Value);
            }

            return levels;
        }

        private static Level LoadLevel(string sourcePath, Dictionary<string, TileSet> tileSets)
        {
            var separators = new[] {"\r\n", ",", "\n"};
            var doc = new XmlDocument();
            doc.Load(sourcePath);
            var root = doc.DocumentElement;
            
            var layers = new List<Layer>();
            var staticShapes = new List<RigidShape>();
            var raytracingEdges = new List<Edge>();
            string source = null;
            Vector playerStartPosition = null;
            Bitmap playerClothesTileMap = null;
            Bitmap playerWeaponsTileMap = null;

            foreach (XmlNode node in root)
            {
                if (node.Name == "properties")
                {
                    foreach (XmlNode property in node.ChildNodes)
                    {
                        if (property.Name != "playerInfo") continue;
                        foreach (XmlAttribute attribute in property.Attributes)
                        {
                            switch (attribute.Name)
                            {
                                case "clothesTileMap":
                                    playerClothesTileMap = new Bitmap(attribute.Value);
                                    break;
                                case "weaponsTileMap":
                                    playerWeaponsTileMap = new Bitmap(attribute.Value);
                                    break;
                                case "position":
                                {
                                    var vector = attribute.Value.Split(',');
                                    playerStartPosition = new Vector(int.Parse(vector[0]), int.Parse(vector[1]));
                                    break;
                                }
                            }
                        }
                    }
                }
                
                if (node.Name == "tileset")
                    source = node.Attributes.GetNamedItem("source").Value;

                if (node.Name == "layer")
                    layers.Add(LoadLayer(node, separators));
                
                if (node.Name == "objectgroup")
                {
                    var nodeContentName = node.Attributes.GetNamedItem("name").Value; 
                    if (nodeContentName == "StaticShapes")
                        staticShapes = LoadStaticShapes(node);
                    if (nodeContentName == "RaytracingPolygons")
                        raytracingEdges = LoadRaytracingEdges(node);
                }
            }

            return new Level(
                layers, staticShapes, raytracingEdges, tileSets[source],
                playerStartPosition, playerClothesTileMap, playerWeaponsTileMap);
        }

        private static EntityCreator.PlayerInitializationInfo ParsePlayerInfo()
        {
            
        }
        
        private static EntityCreator.BotInitializationInfo ParseBotInfo()
        {
            
        }

        private static List<RigidShape> LoadStaticShapes(XmlNode staticShapeNode)
        {
            var staticShapes = new List<RigidShape>();
            foreach (XmlNode shape in staticShapeNode.ChildNodes)
            {
                var x = int.Parse(shape.Attributes.GetNamedItem("x").Value);
                var y = int.Parse(shape.Attributes.GetNamedItem("y").Value);
                var width = int.Parse(shape.Attributes.GetNamedItem("width").Value);
                var height = int.Parse(shape.Attributes.GetNamedItem("height").Value);
                
                staticShapes.Add(new RigidAABB(
                        new Vector(x, y), 
                        new Vector(x + width, y + height), 
                        true, 
                        true));
            }

            return staticShapes;
        }

        private static List<Edge> LoadRaytracingEdges(XmlNode raytracingPolygonsNode)
        {
            var edges = new List<Edge>();
            var separators = new[] {" ", ","};
            
            foreach (XmlNode polygonRaw in raytracingPolygonsNode.ChildNodes)
            {
                var x = int.Parse(polygonRaw.Attributes.GetNamedItem("x").Value);
                var y = int.Parse(polygonRaw.Attributes.GetNamedItem("y").Value);
                var points = polygonRaw.ChildNodes[0].Attributes.GetNamedItem("points").Value; 
                edges.AddRange(LoadPolygon(points, x, y, separators));
            }
            
            return edges;
        }

        private static List<Edge> LoadPolygon(string points, int offsetX, int offsetY, string[] separators)
        {
            var p = points.Split(separators, StringSplitOptions.None);
            var edges = new List<Edge>();
            for (var i = 0; i < p.Length - 3; i += 2)
            {
                edges.Add(new Edge(
                    int.Parse(p[i]), int.Parse(p[i + 1]),
                    int.Parse(p[i + 2]), int.Parse(p[i + 3])));
            }
            
            edges.Add(new Edge(
                int.Parse(p[p.Length - 2]), int.Parse(p[p.Length - 1]),
                int.Parse(p[0]), int.Parse(p[1])));
            
            edges.MoveBy(new Vector(offsetX, offsetY));
            return edges;
        }

        private static Layer LoadLayer(XmlNode node, string[] separators)
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
                newLayer.Tiles = LoadTiles(layerData, newLayer.WidthInTiles * newLayer.HeightInTiles);
            }

            return newLayer;
        }

        private static int[] LoadTiles(string[] layerData, int tilesAmount)
        {
            var newTiles = new int[tilesAmount];
            var k = 0;
            foreach (var tileIndex in layerData)
                if (tileIndex != "") newTiles[k++] = int.Parse(tileIndex);
            return newTiles;
        }

        private static void MoveBy(this List<Edge> edges, Vector delta)
        {
            foreach (var edge in edges)
                edge.MoveBy(delta);
        }
    }
}