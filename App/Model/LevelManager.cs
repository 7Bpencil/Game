using System.Collections.Generic;
using System.Drawing;
using System.IO;
using App.Engine.PhysicsEngine.RigidBody;
using App.Model.LevelData;
using App.Model.Parser;

namespace App.Model
{
    public class LevelManager
    {
        private Dictionary<string, Bitmap> tileMaps;
        private List<Level> levels;
        private int currentLevelIndex;
        public Level CurrentLevel => levels[currentLevelIndex];

        public void MoveNextLevel()
        {
            currentLevelIndex++;
        }

        public LevelManager()
        {
            tileMaps = LoadTileMaps();
            levels = LevelParser.LoadLevels();
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

        
        public Rectangle GetSourceRectangle(int tileID, int columnsInTileMap, int tileSize)
        {
            var sourceX = tileID % columnsInTileMap * tileSize;
            var sourceY = tileID / columnsInTileMap * tileSize;
            return new Rectangle(sourceX, sourceY, tileSize - 1, tileSize - 1);
        }

    }
}