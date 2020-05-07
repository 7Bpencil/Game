using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using App.Engine;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using App.Engine.Sprites;
using App.Model.LevelData;

namespace App.Model.Entities
{
    
    
    
    public class Graph<Location>
    {
        // Если вы всегда используете для точек типы string,
        // то здесь разумной альтернативой будет NameValueCollection
        public Dictionary<Location, Location[]> edges
            = new Dictionary<Location, Location[]>();

        public Location[] Neighbors(Location id)
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

    public class Location
    {
        // Примечания по реализации: я использую Equals по умолчанию,
        // но это может быть медленно. Возможно, в реальном проекте стоит
        // заменить Equals и GetHashCode.
        public override bool Equals(object obj) => 
            obj is Location mys
            && mys.x == this.x
            && mys.y == this.y;
        
        public readonly int x, y;
        public Location(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public interface WeightedGraph<L>
    {
        double Cost(Location a, Location b);
        IEnumerable<Location> Neighbors(Location id);
    }


    public class SquareGrid : WeightedGraph<Location>
    {

        public static readonly Location[] DIRS = new[]
        {
            new Location(1, 0),
            new Location(0, -1),
            new Location(-1, 0),
            new Location(0, 1)
        };

        public int width, height;
        public HashSet<Location> walls = new HashSet<Location>();
        public HashSet<Location> forests = new HashSet<Location>();

        public SquareGrid(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public bool InBounds(Location id)
        {
            return 0 <= id.x && id.x < width
                             && 0 <= id.y && id.y < height;
        }

        public bool Passable(Location id)
        {
            return !walls.Contains(id);
        }

        public double Cost(Location a, Location b)
        {
            return 1;
        }

        public IEnumerable<Location> Neighbors(Location id)
        {
            foreach (var dir in DIRS)
            {
                Location next = new Location(id.x + dir.x, id.y + dir.y);
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
        public Dictionary<Location, Location> cameFrom
            = new Dictionary<Location, Location>();
        public Dictionary<Location, double> costSoFar
            = new Dictionary<Location, double>();

        // Примечание: обобщённая версия A* абстрагируется от Location
        // и Heuristic
        static public double Heuristic(Location a, Location b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        public AStarSearch(WeightedGraph<Location> graph, Location start, Location goal)
        {
            var frontier = new PriorityQueue<Location>();
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

        public List<Location> ReconstructPath(AStarSearch astar, Location start, Location goal)
        {
            var current = goal;
            var path = new List<Location>();
            while (!current.Equals(start))
            {
                path.Add(current);
                current = astar.cameFrom[current];
            }
            path.Add(start);
            path.Reverse();
            return path;
        }

        public List<Vector> GetVelocityPath(List<Location> path, Level level)
        {
            Location prev = path[0];
            List<Vector> result = new List<Vector>();
            result.Add(new Vector(Center.X % level.TileSet.tileWidth, 0));
            result.Add(new Vector(0, Center.Y % level.TileSet.tileHeight));
            bool horizontalState = false;
            bool verticalState = false;
            for (var i = 1; i < path.Count; ++i)
            {
                var dx = path[i].x - prev.x;
                var dy = path[i].y - prev.y;
                if (dx != 0)
                    horizontalState = true;
                if (dy != 0)
                    verticalState = true;
                if (horizontalState && verticalState)
                {
                    result.Add(new Vector(10 * dy, 10 * dx));
                    if (dx == 0)
                        horizontalState = false;
                    if (dy == 0)
                        verticalState = false;
                }
                //for (var count = level.TileSet.tileHeight / 6; count > 0; --count)
                result.Add(new Vector(32 * dy, 32 * dx));
                prev = path[i];
            }
            return result;
        }

        private List<Vector> GetCurrentPath(List<Vector> pathVelocity)
        {
            var current = Center;
            var result = new List<Vector>();
            result.Add(current);
            foreach (var velocity in pathVelocity)
            {
                current += velocity;
                result.Add(current);
            }
            return result;
        }

        private void AddWals(Level level)
        {
            var walls = new HashSet<int>(new List<int>(){316, 356, 320, 354, 323, 319, 469, 468, 392, 358, 318});
            foreach (var layer in level.Layers)
            {
                for (var j = 0; j <= layer.WidthInTiles; ++j)
                for (var i = 0; i <= layer.HeightInTiles; ++i)
                {
                    var tileIndex = i * layer.WidthInTiles + j;
                    if (tileIndex > layer.Tiles.Length - 1) break;
                
                    var tileID = layer.Tiles[tileIndex];
                    if (tileID == 0) continue;
                    if (walls.Contains(tileID))
                        grid.walls.Add(new Location(i, j));
                }   
            }
        }

        static void DrawGrid(SquareGrid grid, AStarSearch astar) {
            // Печать массива cameFrom
            var ptr = new Location(0, 0);
            for (var x = 0; x < grid.width; x++)
            {
                
                for (var y = 0; y < grid.height; y++)
                {
                    Location id = new Location(x, y);
                    if (grid.walls.Contains(id)) { Console.Write("##"); }
                    else { Console.Write("* "); }
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
            

            var radius = collisionShape.Radius;
            var start = new Location((int) centerPosition.X / level.TileSet.tileWidth,
                (int) centerPosition.Y / level.TileSet.tileHeight);
            var goal = new Location((int) patrolPathTiles[0].X, (int) patrolPathTiles[0].Y);
            var astar = new AStarSearch(grid, start, goal);
            var path = ReconstructPath(astar, start, goal);
            pathVelocity = GetVelocityPath(path, level);
            pathVelocity.Add(new Vector(0, 0));
            currentPath = GetCurrentPath(pathVelocity);
            appointmentIndex = 0;
            DrawGrid(grid, astar);
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
                //MoveBy(pathVelocity[Math.Min(tick, pathVelocity.Count - 1)]);
            }
            
        }
    }
}