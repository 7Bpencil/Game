namespace App.Model.LevelData
{
    public class Layer
    {
        public readonly int Id;
        public readonly string Name;
        public readonly int WidthInTiles;
        public readonly int HeightInTiles;
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