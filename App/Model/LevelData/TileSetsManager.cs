using System.Collections.Generic;
using System.IO;

namespace App.Model.LevelData
{
    public class TileSetsManager
    {
        private Dictionary<string, TileSet> assets;
        public TileSet GetTileSet(string tileSetName) => assets[tileSetName];

        public TileSetsManager()
        {
            assets = new Dictionary<string, TileSet>();
            var tilesNames = Directory.GetFiles("Tiles/");
        }
    }
}