using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using App.Model;
using App.Physics_Engine.RigidBody;

namespace App.Physics_Engine
{
    public class Core
    {
        private PlaygroundPhysEngine view;
        private HashSet<Keys> pressedKeys;
        private Keys keyPressed;
        private List<RigidShape> sceneObjects;
        private RigidShape player;
        private RigidShape playerCenter;
        private RigidShape cursor;

        public Core(PlaygroundPhysEngine view)
        {
            this.view = view;
            var objectManager = new ObjectManager();
            sceneObjects = objectManager.GetSceneObjects(out player, out playerCenter, out cursor, view.ClientSize.Width, view.ClientSize.Height);
            pressedKeys = new HashSet<Keys>();
        }

        public void GameLoop(object sender, EventArgs args)
        {
            UpdatePlayer();
            UpdateObjects();
            view.Render();
        }

        public void UpdateObjects()
        {
            foreach (var formObject in sceneObjects)
                formObject.Update();
        }

        public void UpdatePlayer()
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
        
        public void OnMouseMove(Vector newPosition)
        {
            cursor.Center = newPosition;
        }

        public void OnKeyDown(Keys keyPressed)
        {
            pressedKeys.Add(keyPressed);
            this.keyPressed = keyPressed;
        }

        public void OnKeyUp(Keys keyPressed)
        {
            pressedKeys.Remove(keyPressed);
            this.keyPressed = pressedKeys.Any() ? pressedKeys.Min() : Keys.None;
        }
        
        public List<RigidShape> GetSceneObjects()
        {
            return sceneObjects;
        }
    }
}