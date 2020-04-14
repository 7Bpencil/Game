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
        private Dictionary<string, TileSet> tileSets;
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
            tileSets = TileSetParser.LoadTileSets();
            levels = LevelParser.LoadLevels(tileSets);
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

        public Bitmap GetTileMap(TileSet tileSet)
        {
            return tileMaps[tileSet.imageSource];
        }
        
        public Bitmap GetTileMap(string tileMapName)
        {
            return tileMaps[tileMapName];
        }

        public TileSet GetTileSet(string tileSetName)
        {
            return tileSets[tileSetName];
        }

        public TileSet GetTileSet(int tileID, Level level)
        {
            TileSet tileSet = null;
            for (var i = 0; i < level.allFirstgid.Length - 1; i++)
            {
                if (tileID <= level.allFirstgid[i] || tileID >= level.allFirstgid[i + 1]) continue;
                tileSet = level.tileSetFromFirstgid[i];
            }
            
            return tileSet ?? level.tileSetFromFirstgid[level.allFirstgid[level.allFirstgid.Length - 1]];
        }
        
        public Rectangle GetSourceRectangle(int tileID, int columnsInTileMap, int tileSize)
        {
            var sourceX = tileID % columnsInTileMap * tileSize;
            var sourceY = tileID / columnsInTileMap * tileSize;
            return new Rectangle(sourceX, sourceY, tileSize - 1, tileSize - 1);
        }

        public List<RigidShape> GetStaticCollisionInfo(Level level) //TODO
        {
            return null;
        }
    }
}