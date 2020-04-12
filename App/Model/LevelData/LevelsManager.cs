using System.Collections.Generic;

namespace App.Model.LevelData
{
    public class LevelsManager
    {
        private int currentLevelIndex;
        public Level CurrentLevel => levels[currentLevelIndex];

        private List<Level> levels;
        private Dictionary<string, int> nameToIndex;

        public bool MoveNextLevel()
        {
            currentLevelIndex++;
            return currentLevelIndex == levels.Count;
        } 

        /// <summary>
        /// Function to get any level by it's name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Level GetLevel(string name) => levels[nameToIndex[name]];
    }
}