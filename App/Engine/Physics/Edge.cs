namespace App.Engine.Physics
{
    public struct Edge
    {
        public Vector Start;
        public Vector End;

        public Edge(Vector start, Vector end)
        {
            Start = start;
            End = end;
        }

        public Edge(float x1, float y1, float x2, float y2)
        {
            Start = new Vector(x1, y1);
            End = new Vector(x2, y2);
        }

        public void Move(Vector delta)
        {
            Start += delta;
            End += delta;
        }
    }
}