using System;
using System.Collections.Generic;
using System.Drawing;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using App.Model.Entities;

namespace App.Model.LevelData
{
    public class Level
    {
        public readonly Size LevelSizeInTiles;
        public readonly TileSet TileSet;
        public readonly List<Layer> Layers;
        public readonly ShapesIterator Shapes;
        public readonly RigidShape Exit;
        public readonly List<RigidShape> StaticShapes;
        public readonly List<RigidShape> DynmaicShapes;
        public readonly List<Edge> RaytracingEdges;
        public readonly EntityCreator.PlayerInitializationInfo PlayerInfo;
        public readonly List<EntityCreator.BotInitializationInfo> BotsInfo;
        public readonly List<Collectable> Collectables;
        
        public Level(List<Layer> layers, List<RigidShape> staticShapes, List<Edge> raytracingEdges, RigidShape exit,
            TileSet tileSet, Vector playerStartPosition, Bitmap playerClothesTileMap, Bitmap playerWeaponsTileMap)
        {
            Layers = layers;
            TileSet = tileSet;
            RaytracingEdges = raytracingEdges;
            
            LevelSizeInTiles = new Size(layers[0].WidthInTiles, layers[0].HeightInTiles);
            
            
            StaticShapes = staticShapes;
            DynmaicShapes = new List<RigidShape>();
            Shapes = new ShapesIterator(StaticShapes, DynmaicShapes);
            Exit = exit;
        }
    }

    public class ShapesIterator
    {
        private readonly List<RigidShape> staticShapes;
        private readonly List<RigidShape> dynamicShapes;
        public int Length => staticShapes.Count + dynamicShapes.Count;

        public RigidShape this[int index]
        {
            get
            {
                if (index < 0 || index > Length - 1) throw new IndexOutOfRangeException();
                if (index < staticShapes.Count) return staticShapes[index];
                return dynamicShapes[index - staticShapes.Count];
            }
        }

        public ShapesIterator(List<RigidShape> staticShapes, List<RigidShape> dynamicShapes)
        {
            this.staticShapes = staticShapes;
            this.dynamicShapes = dynamicShapes;
        }
    }
}