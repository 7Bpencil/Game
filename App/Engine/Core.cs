using System;
using System.Collections.Generic;
using System.Linq;
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
        private HashSet<Keys> pressedKeys;
        private Keys keyPressed;
        private List<RigidShape> sceneObjects;
        private CollisionDetection collisionDetection;
        private List<CollisionInfo> collisions;
        private RigidShape player;
        private RigidShape playerCenter;
        private RigidShape cursor;

        public Core(ContractView view)
        {
            this.view = view;
            var objectManager = new ObjectFactory();
            collisionDetection = new CollisionDetection();
            sceneObjects = objectManager.GetSceneObjects(out player, out playerCenter, out cursor, view.ClientSize.Width, view.ClientSize.Height);
            pressedKeys = new HashSet<Keys>();
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

            if (keyPressed == Keys.Down || keyPressed == Keys.S) deltaY = 4;
            else if (keyPressed == Keys.Left || keyPressed == Keys.A) deltaX = -4;
            else if (keyPressed == Keys.Up || keyPressed == Keys.W) deltaY = -4;
            else if (keyPressed == Keys.Right || keyPressed == Keys.D) deltaX = 4;

            player.Move(new Vector(deltaX, deltaY));
            playerCenter.Move(new Vector(deltaX, deltaY));
        }
        
        public override void OnMouseMove(Vector newPosition)
        {
            cursor.Center = newPosition;
        }

        public override void OnKeyDown(Keys keyPressed)
        {
            pressedKeys.Add(keyPressed);
            this.keyPressed = keyPressed;
        }

        public override void OnKeyUp(Keys keyPressed)
        {
            pressedKeys.Remove(keyPressed);
            this.keyPressed = pressedKeys.Any() ? pressedKeys.Min() : Keys.None;
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