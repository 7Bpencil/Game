using System.Collections.Generic;
using System.Drawing;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidShapes;

namespace App.Engine.Physics
{
    public class NavMesh
    {
        public readonly int Width;
        public readonly int Height;
        public readonly HashSet<Point> walls;
        public readonly NavMeshRenderForm RenderForm;

        private static readonly Point[] DIRS =
        {
            new Point(1, 0),
            new Point(0, 1),

            new Point(-1, 0),
            new Point(0, -1),

            new Point(1, 1),
            new Point(-1, -1),

            new Point(-1, 1),
            new Point(1, -1),
        };

        public NavMesh(Size levelSizeInTiles, List<RigidShape> staticShapes)
        {
            Width = levelSizeInTiles.Width - 2;
            Height = levelSizeInTiles.Height - 2;
            walls = new HashSet<Point>();
            AddWalls(staticShapes, levelSizeInTiles);
            RenderForm = new NavMeshRenderForm(this);
        }

        private void AddWalls(List<RigidShape> staticShapes, Size levelSizeInTiles)
        {
            var circle = new RigidCircle(Vector.ZeroVector, 32, true, true);

            for (var j = 1; j < levelSizeInTiles.Width - 1; ++j)
            for (var i = 1; i < levelSizeInTiles.Height - 1; ++i)
            {
                circle.MoveTo(new Vector(j, i) * 32);
                var canWalk = true;
                foreach (var staticShape in staticShapes)
                {
                    if (CollisionDetector.GetCollisionInfo(circle, staticShape) == null) continue;
                    canWalk = false;
                    break;
                }

                if (!canWalk) walls.Add(new Point(j, i));
            }
        }

        public bool InBounds(Point id)
        {
            return 0 <= id.X && id.X < Width && 0 <= id.Y && id.Y < Height;
        }

        public bool Passable(Point id)
        {
            return !walls.Contains(id);
        }

        public double Cost(Point a, Point b)
        {
            return 1;
        }

        public IEnumerable<Point> Neighbors(Point id)
        {
            foreach (var dir in DIRS)
            {
                var next = new Point(id.X + dir.X, id.Y + dir.Y);
                if (InBounds(next) && Passable(next))
                {
                    yield return next;
                }
            }
        }

        public class NavMeshRenderForm
        {
            public readonly HashSet<Edge> Edges;
            public readonly HashSet<Vector> Points;

            public NavMeshRenderForm(NavMesh mesh)
            {
                Edges = new HashSet<Edge>();
                Points = new HashSet<Vector>();

                for (var j = 1; j <= mesh.Width; ++j)
                for (var i = 1; i <= mesh.Height; ++i)
                {
                    var currentPoint = new Point(j, i);
                    var currentPointVector = Vector.GetVector(currentPoint) * 32;
                    if (mesh.walls.Contains(currentPoint)) continue;
                    Points.Add(currentPointVector);
                    foreach (var neighbor in mesh.Neighbors(currentPoint))
                    {
                        var neighborVector = Vector.GetVector(neighbor) * 32;
                        if (!mesh.walls.Contains(neighbor) && !Points.Contains(neighborVector))
                            Edges.Add(new Edge(currentPointVector, neighborVector));
                    }
                }
            }
        }
    }
}
