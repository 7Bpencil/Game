using System.Collections.Generic;

namespace App.Model.LevelData
{
    public class Level
    {
        public Dictionary<string, int> tileSetFirstgidFromSource;
        public List<Layer> Layers;

        public Level(Dictionary<string, int> tileSetFirstgidFromSource, List<Layer> layers)
        {
            this.tileSetFirstgidFromSource = tileSetFirstgidFromSource;
            Layers = layers;
        }
    }
}