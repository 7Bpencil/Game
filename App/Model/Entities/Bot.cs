using System;
using System.Collections.Generic;
using App.Engine;
using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidShapes;
using App.Model.Factories;
using App.Model.LevelData;

namespace App.Model.Entities
{
    public class Bot : LivingEntity
    {
        private readonly Weapon CurrentWeapon;

        private float speed = 6;
        private float speedAngular = 12;
        private Vector sight;
        private float sightAngle = 60 / 2;
        private readonly float collisionAvoidanceFactor;

        private readonly List<Vector> patrolPoints;
        private int patrolPointIndex;
        private List<Vector> currentPath;
        private int currentPathPointIndex;

        public Bot(
            int health, int armor, SpriteContainer legsContainer, SpriteContainer torsoContainer, 
            Vector sight, RigidCircle collisionShape, Weapon weapon, string deadBodyPath, List<Vector> patrolPoints) 
            : base(health, armor, collisionShape, legsContainer, torsoContainer, deadBodyPath)
        {
            CurrentWeapon = weapon;
            this.sight = sight;
            collisionAvoidanceFactor = collisionShape.Diameter * 2;
            this.patrolPoints = patrolPoints;
            patrolPointIndex = 0;
            currentPathPointIndex = 0;
        }
        
        public void Update(
            Vector playerPosition, Vector playerVelocity, List<Bullet> sceneBullets, List<AbstractParticleUnit> particles, 
            ShapesIterator shapes, List<List<Vector>> botPaths, List<Edge> walls)
        {
            CurrentWeapon.IncrementTick();
            AvoidCollision(shapes);
            if (IsInView(playerPosition, walls))
            {
                currentPath = null;
                Fire(playerPosition + playerVelocity, sceneBullets, particles);
                var v = playerPosition - Position;
                var radius = CollisionShape.Diameter * 4;
                if (Vector.ScalarProduct(v, v) < radius * radius)
                {
                    MoveCirclesAround(playerPosition, 2);
                }
                else
                {
                    ChasePrey(playerPosition);
                }
            }
            else
            {
                if (currentPath == null || currentPathPointIndex == currentPath.Count || currentPath.Count == 0)
                {
                    currentPath = AStarSearch.SearchPath(Position, playerPosition);
                    currentPathPointIndex = 0;
                }
                ChasePrey(currentPath[currentPathPointIndex]);
                var distVector = currentPath[currentPathPointIndex] - Position;
                if (Vector.ScalarProduct(distVector, distVector) < 32 * 32)
                {
                    currentPathPointIndex++;
                }
            }

            if (currentPath != null && currentPath.Count != 0) botPaths.Add(currentPath);
            Velocity = sight * speed;
        }

        private void MoveCirclesAround(Vector target, float angle)
        {
            var direction = Position - target;
            MoveTo(target + direction.Rotate(angle, Vector.ZeroVector));
        }

        private void Fire(Vector aim, List<Bullet> sceneBullets, List<AbstractParticleUnit> particles)
        {
            RotateToPrey(aim);
            if (CurrentWeapon.IsReady)
            {
                sceneBullets.AddRange(CurrentWeapon.Fire(Position, sight));
                particles.Add(ParticleFactory.CreateShell(Position, sight, CurrentWeapon));
            }
        }

        /// <summary>
        /// Bot will move from point to point
        /// </summary>
        /*private void Patrol()
        {
            if (patrolPointIndex == patrolPoints.Count)
            {
                patrolPointIndex = 0;
            }

            if (currentPath == null || currentPathPointIndex == currentPath.Count)
            {
                currentPathPointIndex = 0;
                currentPath = AStarSearch.SearchPath(Position, patrolPoints[patrolPointIndex]);
                patrolPointIndex++;
            }

            ChasePrey(currentPath[currentPathPointIndex]);
            var distVector = currentPath[currentPathPointIndex] - Position;
            if (Vector.ScalarProduct(distVector, distVector) < 32 * 32)
            {
                currentPathPointIndex++;
            }
        }*/

        private void ChasePrey(Vector preyPosition)
        {
            RotateToPrey(preyPosition);
            MoveTo(Position + sight * speed);
        }

        private void RotateToPrey(Vector playerPosition)
        {
            var vectorToPrey = (playerPosition - Position).Normalize();
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
                    if (circle.Center == Position) continue;
                    TryAvoidCircle(circle);
                }
            }
        }

        private void TryAvoidCircle(RigidCircle circle)
        {
            var vectorToShape = circle.Center - Position;
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

        private bool IsInView(Vector objectCenter, List<Edge> sceneEdges)
        {
            var vectorToObject = (objectCenter - Position).Normalize();
            var sightNormal = sight.GetNormal();
            var v = Vector.ScalarProduct(vectorToObject, sightNormal);
            var sc = Vector.ScalarProduct(vectorToObject, sight);
            if (sc < 0) return false;

            var sightAngleVector =
                v > 0 ? sight.Rotate(sightAngle, Vector.ZeroVector) : sight.Rotate(-sightAngle, Vector.ZeroVector);
            var sightAngleVectorProjection = Vector.ScalarProduct(sightAngleVector, sightNormal);
            if (!(Math.Abs(sightAngleVectorProjection) > Math.Abs(v))) return false;
            foreach (var wall in sceneEdges)
                if (CollisionDetector.AreCollide(Position, objectCenter, wall)) return false;
            return true;
        }

        public override Type GetWeaponType()
        {
            return CurrentWeapon.GetType();
        }
    }
}