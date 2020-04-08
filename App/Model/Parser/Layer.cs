namespace App.Model.Parser
{
    public class Layer
    {
        public int Id;
        public string Name;
        public int Width;
        public int Height;
        public int[] Tiles;

        public Layer(int id, string name, int width, int height)
        {
            Id = id;
            Name = name;
            Width = width;
            Height = height;
            Tiles = new int[width * height];
        }

        public Layer()
        {
        }
    }
}