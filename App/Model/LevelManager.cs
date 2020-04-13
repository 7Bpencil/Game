using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using App.Model.LevelData;
using App.Model.Parser;

namespace App.Model
{
    public class LevelManager
    {
        private Dictionary<string, Bitmap> tileMaps;
        private List<Level> levels;
        private Dictionary<string, TileSet> tileSets;
        private int currentLevelIndex;
        public Level CurrentLevel => levels[currentLevelIndex];

        public void MoveNextLevel()
        {
            currentLevelIndex++;
        }

        public LevelManager()
        {
            levels = LevelParser.LoadLevels();
            tileSets = TileSetParser.LoadTileSets();
            tileMaps = LoadTileMaps();
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

        public Bitmap GetTileMap(string tileSetName)
        {
            return tileMaps[tileSets[tileSetName].imageSource];
        }

        public string GetTileSetName(int tileID, Level level)
        {
            string tileSetName = null;
            for (var i = 0; i < level.allFirstgid.Length - 1; i++)
            {
                if (tileID <= level.allFirstgid[i] || tileID >= level.allFirstgid[i + 1]) continue;
                tileSetName = level.tileSetFromFirstgid[i];
            }
            
            return tileSetName ?? level.tileSetFromFirstgid[level.allFirstgid[level.allFirstgid.Length - 1]];
        }
    }
}