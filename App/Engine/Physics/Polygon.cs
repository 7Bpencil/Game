using System.Collections.Generic;

namespace App.Engine.Physics
{
    public class Polygon
    {
        public List<Edge> Edges;

        public Polygon(List<Edge> edges)
        {
            Edges = edges;
        }

        public void MoveBy(Vector delta)
        {
            foreach (var edge in Edges)
                edge.MoveBy(delta);    
        }
    }
}