using System;
using System.Collections.Generic;
using System.Drawing;
using App.Engine.Physics;

namespace App.Engine
{
    public static class AStarSearch
    {
        private static Dictionary<Point, Point> cameFrom;
        private static Dictionary<Point, float> costSoFar;
        private static NavMesh mesh;

        public static void SetMesh(NavMesh levelNavMesh)
        {
            mesh = levelNavMesh;
        }

        private static Point GetCorrectPoint(Point point)
        {
            if (!mesh.walls.Contains(point)) return point;
            for (var i = -1; i < 2; ++i)
            for (var j = -1; j < 2; ++j)
            {
                var probablyGoodPoint = new Point(point.X + i, point.Y + j);
                if (!mesh.walls.Contains(probablyGoodPoint)) return probablyGoodPoint;
            }

            return point;
        }

        public static List<Vector> SearchPath(Vector startVector, Vector goalVector)
        {
            cameFrom = new Dictionary<Point, Point>();
            costSoFar = new Dictionary<Point, float>();
            var frontier = new PriorityQueue<Point>();

            var start = GetCorrectPoint((startVector / 32).GetPoint());
            var goal = GetCorrectPoint((goalVector / 32).GetPoint());
            if (mesh.walls.Contains(start) || mesh.walls.Contains(goal)) return new List<Vector>();

            frontier.Enqueue(start, 0);

            cameFrom[start] = start;
            costSoFar[start] = 0;
            var current = frontier.Dequeue();
            
            while (current != goal)
            {
                if (current == goal) break;
                foreach (var next in mesh.Neighbors(current))
                {
                    var newCost = costSoFar[current] + mesh.Cost(current, next);
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = (float) newCost;
                        var priority = newCost + Heuristic(next, goal);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
                
                current = frontier.Dequeue();
            }

            return GetCurrentPath(ReconstructPath(start, goal));
        }
        
        private static double Heuristic(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private static List<Point> ReconstructPath(Point start, Point goal)
        {
            var current = goal;
            var path = new List<Point>();
            while (current != start)
            {
                path.Add(current);
                current = cameFrom[current];
            }
            path.Add(start);
            path.Reverse();
            return path;
        }
        
        private static List<Vector> GetCurrentPath(List<Point> path)
        {
            var result = new List<Vector>();
            foreach (var point in path)
                result.Add(new Vector(point.X, point.Y) * 32);
            
            return result;
        }
    }
    
    public class PriorityQueue<T>
    {
        private readonly List<Tuple<T, double>> elements = new List<Tuple<T, double>>();

        public int Count => elements.Count;
        
        public void Enqueue(T item, double priority)
        {
            elements.Add(Tuple.Create(item, priority));
        }

        public T Dequeue()
        {
            var bestIndex = 0;
            if (elements.Count == 0) throw new IndexOutOfRangeException();
            for (var i = 0; i < elements.Count; i++)
            {
                if (elements[i].Item2 < elements[bestIndex].Item2) bestIndex = i;
            }
            
            var bestItem = elements[bestIndex].Item1;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }
}