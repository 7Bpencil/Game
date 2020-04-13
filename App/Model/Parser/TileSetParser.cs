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
        public static Dictionary<string, TileSet> LoadTileSets()
        {
            var tileSetFileNames = Directory.GetFiles("Assets/TileSets");
            var tileSets = new Dictionary<string, TileSet>();
            foreach (var fileName in tileSetFileNames)
            {
                var tileSet = ParseTileSet(fileName);
                tileSets.Add(tileSet.tileSetName, tileSet);
            }

            return tileSets;
        }
        
        private static TileSet ParseTileSet(string tileSetFilename)
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
                    tiles.Add(id, new Tile(ParseTileCollisions(node)));
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
        
        private static List<RigidShape> ParseTileCollisions(XmlNode tileNode)
        {
            var collisionShapes = new List<RigidShape>();
            foreach (XmlNode childnode in tileNode.ChildNodes)
            {
                if (childnode.Name != "objectgroup") continue;
                foreach (XmlNode collision in childnode.ChildNodes)
                    ParseCollision(collision, collisionShapes);
            }

            return collisionShapes;
        }

        private static void ParseCollision(XmlNode collision, List<RigidShape> collisionShapes)
        {
            var x = int.Parse(collision.Attributes.GetNamedItem("x").Value);
            var y = int.Parse(collision.Attributes.GetNamedItem("y").Value);
            var width = int.Parse(collision.Attributes.GetNamedItem("width").Value);
            var height = int.Parse(collision.Attributes.GetNamedItem("height").Value);
            var center = new Vector(x + width / 2, y + height / 2);
            
            if (collision.HasChildNodes)
                collisionShapes.Add(new RigidCircle(center, width, true));
            else
                collisionShapes.Add(new RigidRectangle(center, width, height, 0, true));
        }
    }
}