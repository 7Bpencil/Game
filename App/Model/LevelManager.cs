using System.Collections.Generic;
using App.Model.LevelData;
using App.Model.Parser;

namespace App.Model
{
    public class LevelManager
    {
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
            currentLevelIndex = 0;
        }
    }
}