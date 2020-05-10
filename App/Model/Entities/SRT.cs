/*using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using App.Engine;
using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidShapes;
using App.Engine.Sprites;
using App.Model.LevelData;

namespace App.Model.Entities
{
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

        public AStarSearch(IWeightedGraph<Point> graph, Point start, Point goal)
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
                    double newCost = costSoFar[current] + graph.Cost(current, next);
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) //change || and &&
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
        public static NavMesh grid;

        public List<Vector> currentPath;

        public List<Vector> pathVelocity;
        //imported
        public readonly SpriteContainer TorsoContainer;
        public readonly SpriteContainer LegsContainer;
        private bool statePassive;
        private bool stateAggressive;
        private bool stateDefensive;
        private Level currentLevel;
        private HashSet<Vector> visitedPath;
        private List<Point> patrolPathTilesCoords;
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
        /*private List<List<Vector>> paths;
        private List<List<Vector>> pathsVelocity;#1#
        public Vector Velocity => pathVelocity[tick];
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
            result.Add(Center);
            foreach (var point in path)
            {
                result.Add(new Vector(point.Y, point.X) * 32);
            }
            return result;
        }

        private List<Vector> GetVelocityPath(List<Vector> currentPath)
        {
            var result = new List<Vector>();
            /*result.Add(new Vector(-Center.X % 32, 0));
            result.Add(new Vector(0, -Center.Y % 32));#1#
            for (var i = 0; i < currentPath.Count - 1; i++)
            {
                result.Add(currentPath[i + 1] - currentPath[i]);
            }

            return result;
        }
        
        static void DrawGrid(NavMesh grid) {
            // Печать массива cameFrom
            var ptr = new Point(0, 0);
            for (var y = 0; y < grid.Height; y++)
            {
                for (var x = 0; x < grid.Width; x++)
                {
                    Point id = new Point(x, y);
                    if (grid.walls.Contains(id)) { Console.Write("##"); }
                    else { Console.Write("* "); }
                }
                Console.WriteLine();
            }
        }
        static void DrawPath(NavMesh grid, AStarSearch astar, List<Point> path) {
            // Печать массива cameFrom
            var pathPoints = new HashSet<Point>(path);
            var ptr = new Point(0, 0);
            for (var y = 0; y < grid.Height; y++)
            {
                for (var x = 0; x < grid.Width; x++)
                {
                    var id = new Point(x, y);
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
            /*paths = new List<List<Vector>>();
            pathsVelocity = new List<List<Vector>>();#1#
            collisionShape = new RigidCircle(centerPosition, 32, false, true);
            //included
            currentLevel = level;
            grid = new NavMesh(45 - 2, 40 - 2);
            aim = null;
            player = _player;
            this.weapon = weapon;
            statePassive = true;
            patrolPathTilesCoords = new List<Point>
            {
                new Point(11, 16),
                new Point(34, 13), 
                new Point(27, 21),
                new Point(10, 26),
                
            } ;
            AddWals(level);
            //DrawGrid(grid);
            appointmentIndex = 0;
            UpdatePatrolPath();
            //previous
            this.sceneBullets = sceneBullets;
            Health = health;
            Armour = armour;
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
        //public void ChangeVelocity(Vector newVelocity) => velocity = newVelocity;

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

        public void UpdatePatrolPath()
        {
            Point start = new Point(
                ((int) Center.X / currentLevel.TileSet.tileWidth) - 1,
                ((int) Center.Y / currentLevel.TileSet.tileHeight) - 1);
            var goal = new Point((int) patrolPathTilesCoords[appointmentIndex].X - 1 , (int) patrolPathTilesCoords[appointmentIndex].Y - 1); 
            var astar = new AStarSearch(grid, start, goal);
            var path = ReconstructPath(astar, start, goal);
            currentPath = GetCurrentPath(path);
            pathVelocity = GetVelocityPath(currentPath);
            pathVelocity.Add(new Vector(0, 0));
            /*paths.Add(currentPath);
            pathsVelocity.Add(pathVelocity);#1# 
            DrawPath(grid, astar, path);
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
                }#1#
                
                /*if (weapon.IsReady() && !(aim == null))
                {
                    var direction = aim - collisionShape.Center;
                    sceneBullets.AddRange(weapon.Fire(
                        collisionShape.Center, 
                        direction.Normalize(), 
                        collisionShape.Center));
                }#1#
                
                //
                /*if (tick > ticksForMovement)
                {
                    tick = 0;
                    velocity = -velocity;
                }#1#
                
                //viewVector = velocity.Normalize();
                if (tick == pathVelocity.Count) // переходим к следующей точке, когда дойдем до точки 
                {
                    appointmentIndex++;
                    if (appointmentIndex >= patrolPathTilesCoords.Count)
                        appointmentIndex = 0;
                    tick = 0;
                    UpdatePatrolPath();
                }
                else
                {
                    MoveBy(pathVelocity[tick]);
                }
            }
        }
    }
}*/