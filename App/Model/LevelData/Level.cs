using System.Collections.Generic;
using System.Drawing;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.RigidBody;

namespace App.Model.LevelData
{
    public class Level
    {
        public Size LevelSizeInTiles;
        public TileSet TileSet;
        public List<Layer> Layers;
        public List<RigidShape> StaticShapes;
        public List<Polygon> RaytracingShapes;

        public Level(List<Layer> layers, List<RigidShape> staticShapes, List<Polygon> raytracingShapes, TileSet tileSet)
        {
            Layers = layers;
            TileSet = tileSet;
            StaticShapes = staticShapes;
            RaytracingShapes = raytracingShapes;
            LevelSizeInTiles = new Size(layers[0].WidthInTiles, layers[0].HeightInTiles);
        }
    }
}