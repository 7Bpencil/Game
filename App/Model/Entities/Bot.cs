using System;
using System.Collections.Generic;
using App.Engine;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using App.Model.Factories;
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
        private float speedAngular = 12;
        private Vector sight;
        private float sightAngle = 60 / 2;
        private readonly float collisionAvoidanceFactor;

        private List<Vector> patrolPoints;
        private int patrolPointIndex;
        private List<Vector> currentPath;
        private int currentPathPointIndex;

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
            patrolPoints = new List<Vector>
            {
                new Vector(11, 16) * 32,
                new Vector(34, 13) * 32,
                new Vector(27, 21) * 32,
                new Vector(10, 26) * 32,
            };
            patrolPointIndex = 0;
            currentPathPointIndex = 0;
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

        public List<Vector> Update(Vector playerPosition, List<Bullet> sceneBullets, List<AbstractParticleUnit> particles, ShapesIterator shapes)
        {
            weapon.IncrementTick();
            AvoidCollision(shapes);
            if (IsInView(playerPosition))
            {
                Fire(playerPosition, sceneBullets, particles);
            }
            else
            {
                Patrol();
            }

            return currentPath;
        }

        private void Fire(Vector aim, List<Bullet> sceneBullets, List<AbstractParticleUnit> particles)
        {
            RotateToPrey(aim);
            if (weapon.IsReady)
            {
                sceneBullets.AddRange(weapon.Fire(Center, sight));
                particles.Add(ParticleFactory.CreateShell(Center, sight, weapon));
            }
        }

        private void Patrol()
        {
            if (patrolPointIndex == patrolPoints.Count)
            {
                patrolPointIndex = 0;
            }

            if (currentPath == null || currentPathPointIndex == currentPath.Count)
            {
                currentPathPointIndex = 0;
                currentPath = AStarSearch.SearchPath(Center, patrolPoints[patrolPointIndex]);
                patrolPointIndex++;
            }

            ChasePrey(currentPath[currentPathPointIndex]);
            var distVector = currentPath[currentPathPointIndex] - Center;
            if (Vector.ScalarProduct(distVector, distVector) < 32 * 32)
            {
                currentPathPointIndex++;
            }
        }

        private void ChasePrey(Vector preyPosition)
        {
            RotateToPrey(preyPosition);
            MoveTo(Center + sight * speed);
        }

        private void RotateToPrey(Vector playerPosition)
        {
            var vectorToPrey = (playerPosition - Center).Normalize();
            var sightNormal = sight.GetNormal();
            var v = Vector.ScalarProduct(vectorToPrey, sightNormal);

            var sightAngleVector =
                v > 0 ? sight.Rotate(speedAngular, Vector.ZeroVector) : sight.Rotate(-speedAngular, Vector.ZeroVector);
            var sightAngleVectorProjection = Vector.ScalarProduct(sightAngleVector, sightNormal);
            if (Math.Abs(sightAngleVectorProjection) > Math.Abs(v)) RotateToVector(vectorToPrey);
            else RotateOnDegree(v > 0);
        }

        private void RotateOnDegree(bool isRightTurn)
        {
            var k = isRightTurn ? 1 : -1;
            sight = sight.Rotate(speedAngular * k, Vector.ZeroVector);
            TorsoContainer.Angle -= speedAngular * k;
            LegsContainer.Angle -= speedAngular * k;
        }

        private void RotateToVector(Vector vectorToPrey)
        {
            var angle = Vector.GetAngle(sight, vectorToPrey);
            sight = vectorToPrey.Normalize();
            TorsoContainer.Angle -= angle;
            LegsContainer.Angle -= angle;
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
                RotateOnDegree(u < 0);
            }
        }

        private bool IsInView(Vector objectCenter)
        {
            var vectorToObject = (objectCenter - Center).Normalize();
            var sightNormal = sight.GetNormal();
            var v = Vector.ScalarProduct(vectorToObject, sightNormal);
            var sc = Vector.ScalarProduct(vectorToObject, sight);
            if (sc < 0) return false;

            var sightAngleVector =
                v > 0 ? sight.Rotate(sightAngle, Vector.ZeroVector) : sight.Rotate(-sightAngle, Vector.ZeroVector);
            var sightAngleVectorProjection = Vector.ScalarProduct(sightAngleVector, sightNormal);
            return Math.Abs(sightAngleVectorProjection) > Math.Abs(v);
        }
    }
}