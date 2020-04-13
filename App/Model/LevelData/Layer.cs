using System.Data.Common;

namespace App.Model.LevelData
{
    public class Layer
    {
        public int Id;
        public string Name;
        public int WidthInTiles;
        public int HeightInTiles;
        public int[] Tiles;

        public Layer(int id, string name, int widthInTiles, int heightInTiles)
        {
            Id = id;
            Name = name;
            WidthInTiles = widthInTiles;
            HeightInTiles = heightInTiles;
            Tiles = new int[widthInTiles * heightInTiles];
        }
    }
}