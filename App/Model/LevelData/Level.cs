using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace App.Model.LevelData
{
    public class Level
    {
        public int[] allFirstgid;
        public Dictionary<int, string> tileSetFromFirstgid;
        public List<Layer> Layers;
        public Size levelSizeInTiles;

        public Level(Dictionary<int, string> tileSetFromFirstgid, List<Layer> layers)
        {
            this.tileSetFromFirstgid = tileSetFromFirstgid;
            allFirstgid = tileSetFromFirstgid.Keys.ToArray();
            Layers = layers;
            levelSizeInTiles = new Size(layers[0].WidthInTiles, layers[0].HeightInTiles);
        }
    }
}