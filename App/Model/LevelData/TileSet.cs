using System.Collections.Generic;

namespace App.Model.LevelData
{
    public class TileSet
    {
        public Dictionary<int, Tile> tiles;
        public string tileSetName;
        public int tileWidth;
        public int tileHeight;
        public int tileCount;
        public int columns;
        public string imageSource;
    }
}