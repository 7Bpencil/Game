using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.RigidBody;
using App.Model.LevelData;

namespace App.Model.Parser
{
    public static class TileSetParser
    {
        public static TileSet ParseTileSet(string tileSetFilename)
        {
            var doc = new XmlDocument();
            doc.Load(tileSetFilename);
            var root = doc.DocumentElement;

            var tiles = new Dictionary<int, Tile>();
            string source = null;

            foreach (XmlNode node in root)
            {
                if (node.Name == "image")
                    source = node.Attributes.GetNamedItem("source").Value;
                if (node.Name == "tile")
                {
                    var id = int.Parse(node.Attributes.GetNamedItem("id").Value);
                    var collisionShapes = new List<RigidShape>();
                    foreach (XmlNode childnode in node.ChildNodes)
                    {
                        if (childnode.Name == "objectgroup")
                        {
                            foreach (XmlNode obj in childnode.ChildNodes)
                            {
                                Console.WriteLine(obj.Attributes.GetNamedItem("x").Value);
                                var x = int.Parse(obj.Attributes.GetNamedItem("x").Value);
                                var y = int.Parse(obj.Attributes.GetNamedItem("y").Value);
                                var width = int.Parse(obj.Attributes.GetNamedItem("width").Value);
                                var height = int.Parse(obj.Attributes.GetNamedItem("height").Value);
                                var center = new Vector(x + width / 2, y + height / 2);
                                
                                if (obj.HasChildNodes)
                                    collisionShapes.Add(new RigidCircle(center, width, true));
                                else
                                    collisionShapes.Add(new RigidRectangle(center, width, height, 0, true));
                            }
                        }
                    }
                    tiles.Add(id, new Tile(collisionShapes));
                }
            }
            
            return new TileSet
            {
                tileSetName = root.GetAttribute("name"),
                tileWidth = int.Parse(root.GetAttribute("tilewidth")),
                tileHeight = int.Parse(root.GetAttribute("tileheight")),
                tileCount = int.Parse(root.GetAttribute("tilecount")),
                columns = int.Parse(root.GetAttribute("columns")),
                imageSource = source,
                tiles = tiles
            };
        }

        public static void LoadTileSets()
        {
            var tileSetFileNames = Directory.GetFiles("Assets/TileSets");
            foreach (var fileName in tileSetFileNames)
            {
                Console.WriteLine(fileName);
                var a = ParseTileSet(fileName);
                Console.WriteLine("-END-");
                Console.WriteLine();
            }
        }
    }
}