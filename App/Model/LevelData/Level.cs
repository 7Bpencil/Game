using System.Collections.Generic;

namespace App.Model.LevelData
{
    public class Level
    {
        public TileSet TileSet;
        public List<Layer> Layers;

        public Level(TileSet tileSet, List<Layer> layers)
        {
            TileSet = tileSet;
            Layers = layers;
        }
    }
}