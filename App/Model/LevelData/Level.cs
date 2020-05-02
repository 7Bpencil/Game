using System;
using System.Collections.Generic;
using System.Drawing;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;

namespace App.Model.LevelData
{
    public class Level
    {
        public readonly Size LevelSizeInTiles;
        public readonly Vector PlayerStartPosition;
        public readonly TileSet TileSet;
        public readonly List<Layer> Layers;
        public readonly ShapesIterator Shapes;
        public readonly List<RigidShape> StaticShapes;
        public readonly List<RigidShape> DynmaicShapes;
        public readonly List<Edge> RaytracingEdges;
        public readonly Bitmap PlayerClothesTileMap;
        public readonly Bitmap PlayerWeaponsTileMap;

        public Level(List<Layer> layers, List<RigidShape> staticShapes, List<Edge> raytracingEdges,
            TileSet tileSet, Vector playerStartPosition, Bitmap playerClothesTileMap, Bitmap playerWeaponsTileMap)
        {
            Layers = layers;
            TileSet = tileSet;
            RaytracingEdges = raytracingEdges;
            PlayerStartPosition = playerStartPosition;
            LevelSizeInTiles = new Size(layers[0].WidthInTiles, layers[0].HeightInTiles);
            PlayerClothesTileMap = playerClothesTileMap;
            PlayerWeaponsTileMap = playerWeaponsTileMap;
            
            StaticShapes = staticShapes;
            DynmaicShapes = new List<RigidShape>();
            Shapes = new ShapesIterator(StaticShapes, DynmaicShapes);
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