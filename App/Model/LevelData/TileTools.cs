using System.Drawing;
using App.Engine.PhysicsEngine;

namespace App.Model.LevelData
{
    public static class TileTools
    {
        public static Rectangle GetSourceRectangle(int tileID, int columnsInTileMap, int tileSize)
        {
            var sourceX = tileID % columnsInTileMap * tileSize;
            var sourceY = tileID / columnsInTileMap * tileSize;
            return new Rectangle(sourceX, sourceY, tileSize - 1, tileSize - 1);
        }
        
        public static int GetTileIndex(int cameraX, int cameraY, int levelHeightInTiles)
        {
            var sx = cameraX;
            var sy = cameraY;
            return sy * levelHeightInTiles + sx;
        }
    }
}