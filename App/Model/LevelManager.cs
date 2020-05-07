using System.Collections.Generic;
using System.Drawing;
using System.IO;
using App.Model.DataParser;
using App.Model.LevelData;

namespace App.Model
{
    public static class LevelManager
    {
        private static Dictionary<string, Bitmap> tileMaps;
        private static Dictionary<string, TileSet> tileSets;
        private static Dictionary<int, string> levelList;

        public static void Init()
        {
            tileMaps = LoadTileMaps();
            tileSets = TileSetParser.LoadTileSets(tileMaps);
            levelList = LevelParser.LoadLevelList();
        }

        private static Dictionary<string, Bitmap> LoadTileMaps()
        {
            var bitmapFileNames = Directory.GetFiles("Assets/TileMaps");
            var tileMaps = new Dictionary<string, Bitmap>();
            foreach (var fileName in bitmapFileNames)
                tileMaps.Add(Path.GetFileName(fileName), new Bitmap(fileName));
            
            return tileMaps;
        }

        public static Bitmap GetTileMap(string tileMapName)
        {
            return tileMaps[tileMapName];
        }
    }
}