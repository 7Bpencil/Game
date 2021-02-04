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
        private static Dictionary<int, LevelInfo> cachedLevelsInfo;
        private static int currentLevelId;

        public static void Initialize()
        {
            tileMaps = LoadTileMaps();
            tileSets = TileSetParser.LoadTileSets(tileMaps);
            levelList = LevelParser.LoadLevelList();
            cachedLevelsInfo = new Dictionary<int, LevelInfo>();
            currentLevelId = -1;
        }

        public static Level LoadLevel(int id)
        {
            return new Level(GetLevelInfo(id));
        }

        private static LevelInfo GetLevelInfo(int id)
        {
            if (cachedLevelsInfo.ContainsKey(id)) return cachedLevelsInfo[id];
            var levelInfo = LevelParser.LoadLevel(levelList[id], tileSets);
            cachedLevelsInfo.Add(id, levelInfo);
            return levelInfo;
        }

        private static Dictionary<string, Bitmap> LoadTileMaps()
        {
            var bitmapFileNames = Directory.GetFiles("assets/TileMaps");
            var tileMaps = new Dictionary<string, Bitmap>();
            foreach (var fileName in bitmapFileNames)
                tileMaps.Add(Path.GetFileName(fileName), new Bitmap(fileName));

            return tileMaps;
        }

        public static Bitmap GetTileMap(string tileMapName)
        {
            return tileMaps[tileMapName];
        }

        public static Level MoveNextLevel()
        {
            currentLevelId = (currentLevelId + 1) % levelList.Count;
            return LoadLevel(currentLevelId);
        }
    }
}
