using System.Drawing;

namespace App.Model.LevelData
{
    public class Layer
    {
        public readonly int Id;
        public readonly string Name;
        public int[] Tiles;

        public Layer(int id, string name, Size levelSizeInTiles)
        {
            Id = id;
            Name = name;
            Tiles = new int[levelSizeInTiles.Width * levelSizeInTiles.Height];
        }
    }
}