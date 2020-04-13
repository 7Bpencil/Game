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
        
        public static int GetTileIndex(int cameraX, int cameraY, Vector topLeftTileIndex, int levelHeightInTiles)
        {
            var sx = (int) topLeftTileIndex.X + cameraX;
            var sy = (int) topLeftTileIndex.Y + cameraY;
            return sy * levelHeightInTiles + sx;
        }
        
        public static Vector GetTopLeftTileIndex(Vector cameraPosition, int tileSize)
        {
            return new Vector((int) cameraPosition.X / tileSize, (int) cameraPosition.Y / tileSize);
        }
    }
}