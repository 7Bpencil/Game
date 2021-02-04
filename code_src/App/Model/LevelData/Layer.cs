namespace App.Model.LevelData
{
    public class Layer
    {
        public readonly int Id;
        public readonly string Name;
        public readonly int[] Tiles;

        public Layer(int id, string name, int[] tiles)
        {
            Id = id;
            Name = name;
            Tiles = tiles;
        }
    }
}