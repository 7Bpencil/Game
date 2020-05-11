using App.Engine;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using App.Model.LevelData;

namespace App.Model.Entities
{
    public class Bot
    {
        public int Health;
        public int Armour;
        public bool IsDead;
        public readonly SpriteContainer TorsoContainer;
        public readonly SpriteContainer LegsContainer;
        public readonly RigidCircle CollisionShape;
        private readonly Weapon weapon;
        public Vector Velocity = Vector.ZeroVector;
        private float speed = 6;
        private float speedAngular = 6;
        private Vector sight;
        private readonly float collisionAvoidanceFactor;

        public Vector Center => CollisionShape.Center;
        
        public Bot(
            int health, int armour, Vector startPosition, float startAngle, 
            Sprite legs, Sprite torso, RigidCircle collisionShape, Weapon weapon)
        {
            Health = health;
            Armour = armour;
            CollisionShape = collisionShape;
            LegsContainer = new SpriteContainer(legs, startPosition, startAngle);
            TorsoContainer = new SpriteContainer(torso, startPosition, startAngle);
            this.weapon = weapon;
            sight = new Vector(1, 0).Rotate(-startAngle, Vector.ZeroVector).Normalize();
            collisionAvoidanceFactor = collisionShape.Diameter * 2;
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
        
        public void MoveTo(Vector newPosition) => CollisionShape.MoveTo(newPosition);
        
        public void Update(Vector playerPosition, ShapesIterator shapes) // Placeholder
        {
            AvoidCollision(shapes);
            RotateToPlayer(playerPosition);
            //MoveTo(Center + sight * speed);
        }

        private void RotateToPlayer(Vector playerPosition) // Placeholder
        {
            var vectorToPrey = (playerPosition - Center).Normalize();
            var sightNormal = sight.GetNormal();
            var v = Vector.ScalarProduct(vectorToPrey, sightNormal);
            Rotate(v > 0);
        }

        private void Rotate(bool isRightTurn)
        {
            var k = isRightTurn ? 1 : -1;
            sight = sight.Rotate(speedAngular * k, Vector.ZeroVector);
            TorsoContainer.Angle -= speedAngular * k;
            LegsContainer.Angle -= speedAngular * k;
        }

        private void AvoidCollision(ShapesIterator shapes)
        {
            for (var i = 0; i < shapes.Length; i++)
            {
                if (shapes[i] is RigidCircle)
                {
                    var circle = (RigidCircle) shapes[i];
                    if (circle.Center == Center) continue;
                    TryAvoidCircle(circle);
                }
            }
        }
        
        private void TryAvoidCircle(RigidCircle circle)
        {
            var vectorToShape = circle.Center - Center;
            var sc = Vector.ScalarProduct(vectorToShape, sight);
            if (sc < 0) return;
                    
            var p = sight * sc;
            var projection = p - vectorToShape;
            if (Vector.ScalarProduct(projection, projection) < circle.Radius * circle.Radius 
                && Vector.ScalarProduct(p, p) < collisionAvoidanceFactor * collisionAvoidanceFactor)
            {
                var sightNormal = sight.GetNormal();
                var u = Vector.ScalarProduct(vectorToShape, sightNormal);
                Rotate(u < 0);
            }
        }
    }
}