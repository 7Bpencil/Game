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
        public readonly List<RigidShape> Shapes;
        public readonly List<Edge> RaytracingEdges;
        public readonly Bitmap PlayerClothesTileMap;
        public readonly Bitmap PlayerWeaponsTileMap;

        public Level(List<Layer> layers, List<RigidShape> shapes, List<Edge> raytracingEdges,
            TileSet tileSet, Vector playerStartPosition, Bitmap playerClothesTileMap, Bitmap playerWeaponsTileMap)
        {
            Layers = layers;
            TileSet = tileSet;
            Shapes = shapes;
            RaytracingEdges = raytracingEdges;
            PlayerStartPosition = playerStartPosition;
            LevelSizeInTiles = new Size(layers[0].WidthInTiles, layers[0].HeightInTiles);
            PlayerClothesTileMap = playerClothesTileMap;
            PlayerWeaponsTileMap = playerWeaponsTileMap;
        }
    }
}