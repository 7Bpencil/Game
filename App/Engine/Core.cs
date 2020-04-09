using System;
using System.Collections.Generic;
using System.Windows.Forms;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.Collision;
using App.Engine.PhysicsEngine.RigidBody;
using App.Model;
using App.View;

namespace App.Engine
{
    public class Core : ContractCore
    {
        private ContractView view;
        private KeyStates keyState;
        private List<RigidShape> sceneObjects;
        private CollisionDetection collisionDetection;
        private List<CollisionInfo> collisions;
        private RigidShape player;
        private RigidShape cursor;
        
        class KeyStates
        {
            public bool Up, Down, Left, Right;
        }

        public Core(ContractView view)
        {
            this.view = view;
            var objectManager = new ObjectFactory();
            collisionDetection = new CollisionDetection();
            sceneObjects = objectManager.GetSceneObjects(out player, out cursor, view.ClientSize.Width, view.ClientSize.Height);
            keyState = new KeyStates();
        }

        public override void GameLoop(object sender, EventArgs args)
        {
            UpdatePlayer();
            UpdateObjects();
            view.Render();
        }

        protected override void UpdateObjects()
        {
            collisions = collisionDetection.CalculateCollisions(sceneObjects);
            foreach (var formObject in sceneObjects)
                formObject.Update();
        }

        protected override void UpdatePlayer()
        {
            var deltaX = 0;
            var deltaY = 0;

            if (keyState.Right) deltaX += 4;
            if (keyState.Up) deltaY -= 4;
            if (keyState.Left) deltaX -= 4;
            if (keyState.Down) deltaY += 4;

            player.Move(new Vector(deltaX, deltaY));
        }
        
        public override void OnMouseMove(Vector newPosition)
        {
            cursor.Center = newPosition;
        }
        
        public override void OnKeyDown(Keys keyPressed)
        {
            switch (keyPressed)
            {
                case Keys.Up:
                case Keys.W:
                    keyState.Up = true;
                    break;

                case Keys.Down:
                case Keys.S:
                    keyState.Down = true;
                    break;

                case Keys.Left:
                case Keys.A:
                    keyState.Left = true;
                    break;

                case Keys.Right:
                case Keys.D:
                    keyState.Right = true;
                    break;
            }
        }

        public override void OnKeyUp(Keys keyPressed)
        {
            switch (keyPressed)
            {
                case Keys.Escape:
                    Application.Exit();
                    break;

                case Keys.Up:
                case Keys.W:
                    keyState.Up = false;
                    break;

                case Keys.Down:
                case Keys.S:
                    keyState.Down = false;
                    break;

                case Keys.Left:
                case Keys.A:
                    keyState.Left = false;
                    break;

                case Keys.Right:
                case Keys.D:
                    keyState.Right = false;
                    break;
            }
        }

        public override List<RigidShape> GetSceneObjects()
        {
            return sceneObjects;
        }
        
        public override List<CollisionInfo> GetCollisions()
        {
            return collisions;
        }
    }
}