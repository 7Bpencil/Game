using System.Collections.Generic;
using System.Drawing;
using System.IO;
using App.Model.DataParser;
using App.Model.LevelData;

namespace App.Model
{
    public class LevelManager
    {
        private Dictionary<string, Bitmap> tileMaps;
        private Dictionary<string, TileSet> tileSets;
        private Dictionary<int, string> levelList;
        private int currentLevelIndex;
        public Level CurrentLevel => levelList[currentLevelIndex];

        public void MoveNextLevel()
        {
            currentLevelIndex++;
        }

        public LevelManager()
        {
            tileMaps = LoadTileMaps();
            tileSets = TileSetParser.LoadTileSets(tileMaps);
            levelList = LevelParser.LoadLevelList();
            currentLevelIndex = 0;
        }

        private Dictionary<string, Bitmap> LoadTileMaps()
        {
            var bitmapFileNames = Directory.GetFiles("Assets/TileMaps");
            var tileMaps = new Dictionary<string, Bitmap>();
            foreach (var fileName in bitmapFileNames)
                tileMaps.Add(Path.GetFileName(fileName), new Bitmap(fileName));
            
            return tileMaps;
        }

        public Bitmap GetTileMap(string tileMapName)
        {
            return tileMaps[tileMapName];
        }
    }
}