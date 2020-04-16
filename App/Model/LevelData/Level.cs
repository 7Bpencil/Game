using System.Collections.Generic;
using System.Drawing;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.RigidBody;

namespace App.Model.LevelData
{
    public class Level
    {
        public Size LevelSizeInTiles;
        public Vector PlayerStartPosition;
        public TileSet TileSet;
        public List<Layer> Layers;
        public List<RigidShape> Shapes;
        public List<Polygon> RaytracingShapes;

        public Level(List<Layer> layers, List<RigidShape> shapes, List<Polygon> raytracingShapes, TileSet tileSet, Vector playerStartPosition)
        {
            Layers = layers;
            TileSet = tileSet;
            Shapes = shapes;
            RaytracingShapes = raytracingShapes;
            PlayerStartPosition = playerStartPosition;
            LevelSizeInTiles = new Size(layers[0].WidthInTiles, layers[0].HeightInTiles);
        }
    }
}