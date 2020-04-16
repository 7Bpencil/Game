using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.RigidBody;
using App.Model.LevelData;

namespace App.Model.Parser
{
    public static class LevelParser
    {
        public static List<Level> LoadLevels(Dictionary<string, TileSet> tileSets)
        {
            var levelsFileNames = Directory.GetFiles("Assets/Levels");
            var levels = new List<Level>();
            foreach (var fileName in levelsFileNames)
            {
                var level = ParseLevel(fileName, tileSets);
                levels.Add(level);
            }

            return levels;
        }

        private static Level ParseLevel(string levelFileName, Dictionary<string, TileSet> tileSets)
        {
            var separators = new[] {"\r\n", ","};
            var doc = new XmlDocument();
            doc.Load(levelFileName);
            var root = doc.DocumentElement;
            
            var layers = new List<Layer>();
            var staticShapes = new List<RigidShape>();
            var raytracingShapes = new List<Polygon>();
            string source = null;
            Vector playerStartPosition = null;

            foreach (XmlNode node in root)
            {
                if (node.Name == "properties")
                {
                    foreach (XmlNode property in node.ChildNodes)
                    {
                        if (property.Attributes.GetNamedItem("name").Value == "player position")
                        {
                            var vector = property.Attributes.GetNamedItem("value").Value.Split(',');
                            playerStartPosition = new Vector(int.Parse(vector[0]), int.Parse(vector[1]));                            
                        }
                    }
                }
                
                if (node.Name == "tileset")
                {
                    source = node.Attributes.GetNamedItem("source").Value;
                }

                if (node.Name == "layer")
                {
                    layers.Add(ParseLayer(node, separators));
                }

                if (node.Name == "objectgroup")
                {
                    var nodeContentName = node.Attributes.GetNamedItem("name").Value; 
                    if (nodeContentName == "StaticShapes")
                        staticShapes = ParseStaticShapes(node);
                    if (nodeContentName == "RaytracingShapes")
                        raytracingShapes = ParseRaytracingShapes(node);
                }
            }

            return new Level(layers, staticShapes, raytracingShapes, tileSets[source], playerStartPosition);
        }

        private static List<RigidShape> ParseStaticShapes(XmlNode staticShapeNode)
        {
            var staticShapes = new List<RigidShape>();
            foreach (XmlNode shape in staticShapeNode.ChildNodes)
            {
                var x = int.Parse(shape.Attributes.GetNamedItem("x").Value);
                var y = int.Parse(shape.Attributes.GetNamedItem("y").Value);
                var width = int.Parse(shape.Attributes.GetNamedItem("width").Value);
                var height = int.Parse(shape.Attributes.GetNamedItem("height").Value);
                var center = new Vector(x + width / 2, y + height / 2);
                
                staticShapes.Add(new RigidRectangle(center, width, height, 0, true, true));
            }

            return staticShapes;
        }

        private static List<Polygon> ParseRaytracingShapes(XmlNode raytracingShapesNode)
        {
            var polygons = new List<Polygon>();
            var separators = new[] {" ", ","};
            
            foreach (XmlNode polygonRaw in raytracingShapesNode.ChildNodes)
            {
                var x = int.Parse(polygonRaw.Attributes.GetNamedItem("x").Value);
                var y = int.Parse(polygonRaw.Attributes.GetNamedItem("y").Value);
                var points = polygonRaw.ChildNodes[0].Attributes.GetNamedItem("points").Value; 
                polygons.Add(ParsePolygon(points, x, y, separators));
            }
            
            return polygons;
        }

        private static Polygon ParsePolygon(string points, int offsetX, int offsetY, string[] separators)
        {
            var p = points.Split(separators, StringSplitOptions.None);
            var edges = new List<Edge>();
            for (var i = 0; i < p.Length - 3; i+=2)
            {
                edges.Add(new Edge(
                    int.Parse(p[i]), int.Parse(p[i + 1]),
                    int.Parse(p[i + 2]), int.Parse(p[i + 3])));
            }
            
            edges.Add(new Edge(
                int.Parse(p[p.Length - 2]), int.Parse(p[p.Length - 1]),
                int.Parse(p[0]), int.Parse(p[1])));

            var polygon = new Polygon(edges);
            polygon.Move(new Vector(offsetX, offsetY));
            return polygon;
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