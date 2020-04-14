using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace App.Model.LevelData
{
    public class Level
    {
        public int[] allFirstgid;
        public Dictionary<int, TileSet> tileSetFromFirstgid;
        public List<Layer> Layers;
        public Size LevelSizeInTiles;

        public Level(Dictionary<int, TileSet> tileSetFromFirstgid, List<Layer> layers)
        {
            this.tileSetFromFirstgid = tileSetFromFirstgid;
            allFirstgid = tileSetFromFirstgid.Keys.ToArray();
            Layers = layers;
            LevelSizeInTiles = new Size(layers[0].WidthInTiles, layers[0].HeightInTiles);
        }
    }
}