using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using App.Engine.Render;
using App.Model.Entities;
using App.Model.Entities.Collectables;
using App.Model.Factories;
using App.Model.Entities.Weapons;
using App.Model.LevelData;

namespace App.Model.DataParser
{
    public static class LevelParser
    {
        private static Dictionary<string, Type> weaponTypesTable = new Dictionary<string, Type>
        {
            {"AK303", typeof(AK303)},
            {"Shotgun", typeof(Shotgun)},
            {"MP6", typeof(MP6)},
            {"SaigaFA", typeof(SaigaFA)}
        };
        
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

        public static LevelInfo LoadLevel(string sourcePath, Dictionary<string, TileSet> tileSets)
        {
            var separators = new[] {"\r\n", ",", "\n"};
            var doc = new XmlDocument();
            doc.Load(sourcePath);
            var root = doc.DocumentElement;
            
            var levelSizeInTiles = new Size(
                int.Parse(root.GetAttribute("width")), int.Parse(root.GetAttribute("height")));
            var name = root.GetAttribute("name");
            var tilesInLayerAmount = 0;
            TileSet tileSet = null;
            var layers = new List<Layer>();
            List<RigidShape> staticShapes = null;
            List<Edge> raytracingEdges = null;
            var collectableWeapons = new List<CollectableWeaponInfo> {Capacity = 8};
            EntityCreator.PlayerInfo playerInfo = null;
            List<EntityCreator.BotInfo> botsInfo = null; 
            RigidAABB exit = null;

            foreach (XmlNode node in root.ChildNodes)
            {
                switch (node.Name)
                {
                    case "properties":
                    {
                        foreach (XmlNode property in node.ChildNodes)
                        {
                            if (property.Name == "exit") exit = LoadRigidAABB(property);
                            if (property.Name == "collectables")
                            {
                                foreach (XmlNode collectable in property.ChildNodes)
                                {
                                    if (collectable.Name == "collectableWeapon") collectableWeapons.Add(LoadCollectableWeapon(collectable));
                                }
                            }

                            if (property.Name == "playerInfo") playerInfo = LoadPlayerInfo(property);
                            if (property.Name == "bots") botsInfo = loadBots(property);
                        }

                        break;
                    }
                    case "tileset":
                        tileSet = tileSets[node.Attributes.GetNamedItem("source").Value];
                        tilesInLayerAmount = levelSizeInTiles.Width * levelSizeInTiles.Height;
                        break;
                    case "layer":
                        layers.Add(LoadLayer(node, separators, tilesInLayerAmount));
                        break;
                    case "objectgroup":
                    {
                        var nodeContentName = node.Attributes.GetNamedItem("name").Value; 
                        if (nodeContentName == "StaticShapes") staticShapes = LoadStaticShapes(node);
                        if (nodeContentName == "RaytracingPolygons") raytracingEdges = LoadRaytracingEdges(node);
                        break;
                    }
                }
            }

            var levelMap = RenderPipeline.RenderLevelMap(layers, tileSet, tileSet.TileSize, levelSizeInTiles);
            return new LevelInfo(
                levelSizeInTiles, name, exit, staticShapes, levelMap, raytracingEdges, playerInfo, botsInfo, collectableWeapons);
        }

        private static EntityCreator.PlayerInfo LoadPlayerInfo(XmlNode playerNode)
        {
            var health = int.Parse(playerNode.Attributes.GetNamedItem("health").Value);
            var armor = int.Parse(playerNode.Attributes.GetNamedItem("armor").Value);
            var position = LoadVector(playerNode.Attributes.GetNamedItem("position").Value);
            var angle = int.Parse(playerNode.Attributes.GetNamedItem("angle").Value);
            var clothesTileMapPath = playerNode.Attributes.GetNamedItem("clothesTileMap").Value;
            var weaponsTileMapPath = playerNode.Attributes.GetNamedItem("weaponsTileMap").Value;
            var meleeWeaponTileMapPath = playerNode.Attributes.GetNamedItem("meleeWeaponTileMap").Value;
            var weapons = new List<WeaponInfo>();
            foreach (XmlNode node in playerNode.ChildNodes)
            {
                if (node.Name == "weapon") weapons.Add(LoadWeapon(node));
            }

            return new EntityCreator.PlayerInfo
                (health, armor, position, angle, weapons, clothesTileMapPath, weaponsTileMapPath, meleeWeaponTileMapPath);
        }

        private static List<EntityCreator.BotInfo> loadBots(XmlNode botsNode)
        {
            var bots = new List<EntityCreator.BotInfo> {Capacity = 8};
            foreach (XmlNode bot in botsNode.ChildNodes)
                bots.Add(LoadBotInfo(bot));
            return bots;
        }
        
        private static EntityCreator.BotInfo LoadBotInfo(XmlNode botNode)
        {
            var health = int.Parse(botNode.Attributes.GetNamedItem("health").Value);
            var armor = int.Parse(botNode.Attributes.GetNamedItem("armor").Value);
            var position = LoadVector(botNode.Attributes.GetNamedItem("position").Value);
            var angle = int.Parse(botNode.Attributes.GetNamedItem("angle").Value);
            var clothesTileMapPath = botNode.Attributes.GetNamedItem("clothesTileMap").Value;
            var weaponsTileMapPath = botNode.Attributes.GetNamedItem("weaponsTileMap").Value;
            WeaponInfo weapon = null;
            foreach (XmlNode node in botNode.ChildNodes)
            {
                if (node.Name == "weapon") weapon = LoadWeapon(node);
            }
            
            return new EntityCreator.BotInfo
                (health, armor, position, angle, weapon, clothesTileMapPath, weaponsTileMapPath);
        }

        private static List<RigidShape> LoadStaticShapes(XmlNode staticShapeNode)
        {
            var staticShapes = new List<RigidShape>();
            foreach (XmlNode shape in staticShapeNode.ChildNodes)
            {
                if (shape.Name != "aabb") continue;
                staticShapes.Add(LoadRigidAABB(shape));
            }

            return staticShapes;
        }

        private static CollectableWeaponInfo LoadCollectableWeapon(XmlNode node)
        {
            var weaponInfo = LoadWeapon(node);
            return new CollectableWeaponInfo(
                weaponInfo,
                LoadVector(node.Attributes.GetNamedItem("position").Value),
                int.Parse(node.Attributes.GetNamedItem("angle").Value));
        }

        private static WeaponInfo LoadWeapon(XmlNode weaponNode)
        {
            return new WeaponInfo(
                weaponTypesTable[weaponNode.Attributes.GetNamedItem("type").Value],
                int.Parse(weaponNode.Attributes.GetNamedItem("ammo").Value));
        }

        private static RigidAABB LoadRigidAABB(XmlNode shape)
        {
            var x = int.Parse(shape.Attributes.GetNamedItem("x").Value);
            var y = int.Parse(shape.Attributes.GetNamedItem("y").Value);
            var width = int.Parse(shape.Attributes.GetNamedItem("width").Value);
            var height = int.Parse(shape.Attributes.GetNamedItem("height").Value);
                
            return new RigidAABB(
                new Vector(x, y), 
                new Vector(x + width, y + height), 
                true, 
                true);
        }

        private static List<Edge> LoadRaytracingEdges(XmlNode raytracingPolygonsNode)
        {
            var edges = new List<Edge>();
            var separators = new[] {" ", ","};
            
            foreach (XmlNode polygonRaw in raytracingPolygonsNode.ChildNodes)
            {
                var offsetX = int.Parse(polygonRaw.Attributes.GetNamedItem("offsetX").Value);
                var offsetY = int.Parse(polygonRaw.Attributes.GetNamedItem("offsetY").Value);
                var points = polygonRaw.Attributes.GetNamedItem("points").Value; 
                edges.AddRange(LoadPolygon(points, offsetX, offsetY, separators));
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

        private static Layer LoadLayer(XmlNode node, string[] separators, int tilesInLayerAmount)
        {
            var tiles = new int[tilesInLayerAmount];
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name != "data") continue;
                var layerData = childNode.InnerText.Split(separators, StringSplitOptions.None);
                LoadTiles(tiles, layerData);
            }

            return new Layer(
                int.Parse(node.Attributes.GetNamedItem("id").Value),
                node.Attributes.GetNamedItem("name").Value,
                tiles);
        }

        private static void LoadTiles(int[] tiles, string[] layerData)
        {
            var k = 0;
            foreach (var tileIndex in layerData)
                if (tileIndex != "") tiles[k++] = int.Parse(tileIndex);
        }

        private static Vector LoadVector(string vectorRaw)
        {
            var vector = vectorRaw.Split(',');
            return new Vector(int.Parse(vector[0]), int.Parse(vector[1]));
        }

        private static void MoveBy(this List<Edge> edges, Vector delta)
        {
            foreach (var edge in edges)
                edge.MoveBy(delta);
        }
    }
}