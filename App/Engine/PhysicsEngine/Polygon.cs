using System.Collections.Generic;

namespace App.Engine.PhysicsEngine
{
    public class Polygon
    {
        public List<Edge> Edges;

        public Polygon(List<Edge> edges)
        {
            Edges = edges;
        }

        public void Move(Vector delta)
        {
            foreach (var edge in Edges)
                edge.Move(delta);    
        }
    }
}