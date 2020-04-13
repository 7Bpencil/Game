using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.RigidBody;
using App.View;

namespace App.Model
{
    public class Player
    {
        public RigidCircle Shape;
        public Sprite Torso;
        public Sprite Legs;
        private Vector viewVector;
        public Vector ViewVector
        { get => viewVector; set => viewVector = value; }

        public void Move(Vector delta)
        {
            Shape.Center += delta;
            Torso.Center += delta;
            Legs.Center += delta;
        }

        public Vector Center => Shape.Center;
        public float Radius => Shape.Radius;
    }
}