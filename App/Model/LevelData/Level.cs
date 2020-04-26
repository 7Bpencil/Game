using System.Collections.Generic;
using System.Drawing;
using App.Engine.Physics;
using App.Engine.Physics.RigidBody;

namespace App.Model.LevelData
{
    public class Level
    {
        public Size LevelSizeInTiles;
        public Vector PlayerStartPosition;
        public TileSet TileSet;
        public List<Layer> Layers;
        public List<RigidShape> Shapes;
        public List<Edge> RaytracingEdges;

        public Level(List<Layer> layers, List<RigidShape> shapes, List<Edge> raytracingEdges, TileSet tileSet, Vector playerStartPosition)
        {
            Layers = layers;
            TileSet = tileSet;
            Shapes = shapes;
            RaytracingEdges = raytracingEdges;
            PlayerStartPosition = playerStartPosition;
            LevelSizeInTiles = new Size(layers[0].WidthInTiles, layers[0].HeightInTiles);
        }
    }
}