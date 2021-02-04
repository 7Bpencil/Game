using System.Drawing;

namespace App.Model.LevelData
{
    public class TileSet
    {
        public readonly string Name;
        public readonly int TileSize;
        public readonly int TileCount;
        public readonly int Columns;
        public readonly Bitmap Image;

        public TileSet(string name, int tileSize, int tileCount, int columns, Bitmap image)
        {
            Name = name;
            TileSize = tileSize;
            TileCount = tileCount;
            Columns = columns;
            Image = image;
        }
    }
}