using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

    public struct Location
    {
        // Примечания по реализации: я использую Equals по умолчанию,
        // но это может быть медленно. Возможно, в реальном проекте стоит
        // заменить Equals и GetHashCode.

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
            return forests.Contains(b) ? 5 : 1;
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
}

    public class ShootingRangeTarget
    {
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

        public ShootingRangeTarget(Player _player, Sprite legs, Sprite torso, float angle,
            int health, int armour, Vector centerPosition, Vector velocity, int ticksForMovement, Weapon weapon,
            List<Bullet> sceneBullets)
        {
            //included
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
            List<Vector> patrolPathCoords = patrolPathTiles.Select(x => x * 32).ToList();
            appointmentIndex = 0;
            //previous
            this.sceneBullets = sceneBullets;
            Health = health;
            Armour = armour;
            collisionShape = new RigidCircle(centerPosition, 32, false, true);
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
                
                if (weapon.IsReady() && !(aim == null))
                {
                    var direction = aim - collisionShape.Center;
                    sceneBullets.AddRange(weapon.Fire(
                        collisionShape.Center, 
                        direction.Normalize(), 
                        collisionShape.Center));
                }
                
                //
                if (tick > ticksForMovement)
                {
                    tick = 0;
                    velocity = -velocity;
                }
                viewVector = velocity.Normalize();
                MoveBy(velocity);
            }
            
        }
    }
}