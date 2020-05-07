using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using App.Engine;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using App.Engine.Sprites;
using App.Model.LevelData;

namespace App.Model.Entities
{
    
    
    
    public class Graph<Point>
    {
        // Если вы всегда используете для точек типы string,
        // то здесь разумной альтернативой будет NameValueCollection
        public Dictionary<Point, Point[]> edges
            = new Dictionary<Point, Point[]>();

        public Point[] Neighbors(Point id)
        {
            return edges[id];
        }
    };

    class BreadthFirstSearch
    {
        static void Search(Graph<string> graph, string start)
        {
            var frontier = new Queue<string>();
            frontier.Enqueue(start);

            var visited = new HashSet<string>();
            visited.Add(start);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                Console.WriteLine("Visiting {0}", current);
                foreach (var next in graph.Neighbors(current))
                {
                    if (!visited.Contains(next))
                    {
                        frontier.Enqueue(next);
                        visited.Add(next);
                    }
                }
            }
        }
    }

    /*public class Point
    {
        // Примечания по реализации: я использую Equals по умолчанию,
        // но это может быть медленно. Возможно, в реальном проекте стоит
        // заменить Equals и GetHashCode.
        public override bool Equals(object obj) => 
            obj is Point mys
            && mys.x == this.x
            && mys.y == this.y;
        
        public readonly int x, y;
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }*/

    public interface WeightedGraph<L>
    {
        double Cost(Point a, Point b);
        IEnumerable<Point> Neighbors(Point id);
    }


    public class SquareGrid : WeightedGraph<Point>
    {

        public static readonly Point[] DIRS = new[]
        {
            new Point(1, 0),
            new Point(0, -1),
            new Point(-1, 0),
            new Point(0, 1)
        };

        public int width, height;
        public HashSet<Point> walls = new HashSet<Point>();
        public HashSet<Point> forests = new HashSet<Point>();

        public SquareGrid(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public bool InBounds(Point id)
        {
            return 0 <= id.X && id.X < width && 0 <= id.Y && id.Y < height;
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
                Point next = new Point(id.X + dir.X, id.Y + dir.Y);
                if (InBounds(next) && Passable(next))
                {
                    yield return next;
                }
            }
        }
    }

    public class PriorityQueue<T>
    {
        private List<Tuple<T, double>> elements = new List<Tuple<T, double>>();

        public int Count
        {
            get { return elements.Count; }
        }

        public void Enqueue(T item, double priority)
        {
            elements.Add(Tuple.Create(item, priority));
        }

        public T Dequeue()
        {
            int bestIndex = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Item2 < elements[bestIndex].Item2)
                {
                    bestIndex = i;
                }
            }

            T bestItem = elements[bestIndex].Item1;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }
    
    public class AStarSearch
    {
        public Dictionary<Point, Point> cameFrom
            = new Dictionary<Point, Point>();
        public Dictionary<Point, double> costSoFar
            = new Dictionary<Point, double>();

        // Примечание: обобщённая версия A* абстрагируется от Point
        // и Heuristic
        static public double Heuristic(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        public AStarSearch(WeightedGraph<Point> graph, Point start, Point goal)
        {
            var frontier = new PriorityQueue<Point>();
            frontier.Enqueue(start, 0);

            cameFrom[start] = start;
            costSoFar[start] = 0;
            var current = frontier.Dequeue();

            while (!current.Equals(goal))
            {
                

                if (current.Equals(goal))
                {
                    break;
                }

                foreach (var next in graph.Neighbors(current))
                {
                    double newCost = costSoFar[current]
                                     + graph.Cost(current, next);
                    if (!costSoFar.ContainsKey(next)
                        || newCost < costSoFar[next])//change || and &&
                    {
                        costSoFar[next] = newCost;
                        double priority = newCost + Heuristic(next, goal);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
                current = frontier.Dequeue();
            }
        }
    }

    public class ShootingRangeTarget
    {
        public static SquareGrid grid;

        public List<Vector> currentPath;

        public List<Vector> pathVelocity;
        //imported
        public readonly SpriteContainer TorsoContainer;
        public readonly SpriteContainer LegsContainer;
        private bool statePassive;
        private bool stateAggressive;
        private bool stateDefensive;

        private HashSet<Vector> visitedPath;

        private int appointmentIndex;
        //previous
        public Vector viewVector;
        public float viewAngle = 180;
        public Vector aim;
        public int Health;
        public int Armour;
        public bool IsDead;
        private Weapon weapon;
        private List<Bullet> sceneBullets;
        public RigidCircle collisionShape;
        private readonly int ticksForMovement;
        private int tick;
        private Player player;
        
        private Vector velocity;
        public Vector Velocity => velocity;
        public Vector Center => collisionShape.Center;

        public List<Point> ReconstructPath(AStarSearch astar, Point start, Point goal)
        {
            var current = goal;
            var path = new List<Point>();
            while (!current.Equals(start))
            {
                path.Add(current);
                current = astar.cameFrom[current];
            }
            path.Add(start);
            path.Reverse();
            return path;
        }
        private List<Vector> GetCurrentPath(List<Point> path)
        {
            var result = new List<Vector>();
            foreach (var point in path)
            {
                result.Add(new Vector(point.Y, point.X) * 32);
            }
            return result;
        }

        private List<Vector> GetVelocityPath(List<Vector> currentPath)
        {
            var result = new List<Vector>();
            for (var i = 0; i < currentPath.Count - 1; i++)
            {
                result.Add(currentPath[i + 1] - currentPath[i]);
            }

            return result;
        }

        private void AddWals(Level level)
        {
            var wallTiles = new List<int>
            {
                316, 317, 318, 319, 320, 321, 322, 323, 
                354, 355, 356, 357, 358, 359, 360, 361, 
                392, 393, 394, 395, 396, 397, 
                430, 431, 432, 433, 434, 435, 
                468, 469, 470, 471
            };
            var walls = new HashSet<int>(wallTiles);
            foreach (var layer in level.Layers)
            {
                for (var j = 0; j <= layer.WidthInTiles; ++j)
                for (var i = 0; i <= layer.HeightInTiles; ++i)
                {
                    var tileIndex = i * layer.WidthInTiles + j;
                    if (tileIndex > layer.Tiles.Length - 1) break;
                
                    var tileID = layer.Tiles[tileIndex];
                    if (tileID == 0) continue;
                    if (walls.Contains(tileID - 1))
                        grid.walls.Add(new Point(i, j));
                }   
            }
        }

        static void DrawGrid(SquareGrid grid, AStarSearch astar) {
            // Печать массива cameFrom
            var ptr = new Point(0, 0);
            for (var y = 0; y < grid.height; y++)
            {
                for (var x = 0; x < grid.width; x++)
                {
                    Point id = new Point(y, x);
                    if (grid.walls.Contains(id)) { Console.Write("##"); }
                    else { Console.Write("* "); }
                }
                Console.WriteLine();
            }
        }
        static void DrawPath(SquareGrid grid, AStarSearch astar, List<Point> path) {
            // Печать массива cameFrom
            var pathPoints = new HashSet<Point>(path);
            var ptr = new Point(0, 0);
            for (var y = 0; y < grid.height; y++)
            {
                for (var x = 0; x < grid.width; x++)
                {
                    var id = new Point(y, x);
                    if (pathPoints.Contains(id)) Console.Write("$$");
                    else if (grid.walls.Contains(id)) Console.Write("##");
                    else Console.Write("* ");
                }
                Console.WriteLine();
            }
        }

        public ShootingRangeTarget(Level level, Player _player, Sprite legs, Sprite torso, float angle,
            int health, int armour, Vector centerPosition, Vector velocity, int ticksForMovement, Weapon weapon,
            List<Bullet> sceneBullets)
        {
            collisionShape = new RigidCircle(centerPosition, 32, false, true);
            //included
            grid = new SquareGrid(45, 40);
            aim = null;
            player = _player;
            this.weapon = weapon;
            statePassive = true;
            List<Vector> patrolPathTiles = new List<Vector>
            {
                new Vector(8, 10), 
                new Vector(10, 16), 
                new Vector(27, 12),
                new Vector(32, 20),
                new Vector(26, 27),
                new Vector(11, 30),

            } ;
            
            AddWals(level);
            
            var start = new Point(
                (int) centerPosition.X / level.TileSet.tileWidth,
                (int) centerPosition.Y / level.TileSet.tileHeight); 
            
            var goal = new Point((int) patrolPathTiles[0].X, (int) patrolPathTiles[0].Y);
            
            var astar = new AStarSearch(grid, start, goal);
            
            var path = ReconstructPath(astar, start, goal);
           
            
            
            currentPath = GetCurrentPath(path);
            pathVelocity = GetVelocityPath(currentPath);
            pathVelocity.Add(new Vector(0, 0));
            appointmentIndex = 0;
            //DrawPath(grid, astar, path);
            //previous
            this.sceneBullets = sceneBullets;
            Health = health;
            Armour = armour;
            
            this.velocity = velocity;
            IsDead = false;
            this.ticksForMovement = ticksForMovement;
            TorsoContainer = new SpriteContainer(torso, centerPosition, angle);
            LegsContainer = new SpriteContainer(legs, centerPosition, angle);
        }
        
        public void TakeHit(int damage)
        {
            if (IsDead) return;
            Armour -= damage;
            if (Armour < 0)
            {
                Health += Armour;
                Armour = 0;
            }

            if (Health <= 0) IsDead = true;
        }
        
        public void MoveBy(Vector delta) => collisionShape.MoveBy(delta);
        public void MoveTo(Vector newPosition) => collisionShape.MoveTo(newPosition);
        public void ChangeVelocity(Vector newVelocity) => velocity = newVelocity;

        public void Fire()
        {
            var direction = aim - collisionShape.Center;
            sceneBullets.AddRange(weapon.Fire(
                collisionShape.Center, 
                direction.Normalize(), 
                collisionShape.Center));
        }

        public List<Vector> GetDifference(Vector targetCoords)
        {
            List<Vector> result = new List<Vector>();
            if (collisionShape.Center.X > targetCoords.X) result.Add(new Vector(-1, 0));
            if (collisionShape.Center.X < targetCoords.X) result.Add(new Vector(1, 0));
            if (collisionShape.Center.Y > targetCoords.Y) result.Add(new Vector(0, -1));
            if (collisionShape.Center.Y < targetCoords.Y) result.Add(new Vector(0, 1));
            return result;
        }
        

        public List<Vector> FindPath()
        {
            return new List<Vector>();
        }
        
        public List<Vector> GetPathToAppointment(Vector targetCoords)//define where is target and give dx/dy list to FindPath function.
        {
            var difference = GetDifference(targetCoords);
            
            return new List<Vector>();
        } 
        
        public void FindEnemy()
        {
            throw new NotImplementedException();
        }

        public void FindCover()
        {
            throw new NotImplementedException();
        }

        public void MoveTo(Player player)
        {
               
        }
        
        public void Update()
        {
            tick++;
            weapon.IncrementTick();
            if (!IsDead)
            {
                //
                /*if (Health == 100 && aim == null)
                {
                    statePassive = true;
                    stateAggressive = false;
                    stateDefensive = false;
                }

                if (Health <= 100 || Health <= 20)
                {
                    statePassive = false;
                    stateAggressive = false;
                    stateDefensive = true;
                }
            
                if (Health > 20)
                {
                    statePassive = false;
                    stateAggressive = true;
                    stateDefensive = false;
                }

                if (!statePassive)
                {
                    if (stateAggressive) FindEnemy();
                    else if (stateDefensive) FindCover();
                }*/
                
                /*if (weapon.IsReady() && !(aim == null))
                {
                    var direction = aim - collisionShape.Center;
                    sceneBullets.AddRange(weapon.Fire(
                        collisionShape.Center, 
                        direction.Normalize(), 
                        collisionShape.Center));
                }*/
                
                //
                /*if (tick > ticksForMovement)
                {
                    tick = 0;
                    velocity = -velocity;
                }*/
                viewVector = velocity.Normalize();
                MoveBy(pathVelocity[Math.Min(tick, pathVelocity.Count - 1)]);
            }
            
        }
    }
}