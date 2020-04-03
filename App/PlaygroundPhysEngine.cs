using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using App.Physics_Engine;
using App.Physics_Engine.RigidBody;

namespace App
{
    public class PlaygroundPhysEngine : Form
    {
        private readonly HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private static Keys keyPressed;
        private List<IRigidShape> sceneObjects;
        private RigidRectangle player;
        private RigidRectangle playerCenter;
        private RigidCircle cursor;

        public PlaygroundPhysEngine()
        {
            SetUpForm();
            SetUpSceneObjects();
            SetUpPlayer();

            var timer = new Timer();
            timer.Interval = 15;
            timer.Tick += MainLoop;
            timer.Start();
        }

        private void SetUpForm()
        {
            Cursor.Hide();
            DoubleBuffered = true;
            Size = new Size(854, 480);
            Text = "NEW GAME";
        }
        
        private void SetUpSceneObjects()
        {
            sceneObjects = new List<IRigidShape>();
            sceneObjects.Add(new RigidRectangle(new Vector(250, 450), 190, 100, -45));
            sceneObjects.Add(new RigidRectangle(new Vector(440, 110), 160, 100, -17));
            sceneObjects.Add(new RigidRectangle(new Vector(250, 150), 280, 110, 40));
            sceneObjects.Add(new RigidRectangle(new Vector(650, 350), 320, 150, 15));
            sceneObjects.Add(new RigidCircle(new Vector(100, 100), 35));
        }

        private void SetUpPlayer()
        {
            const float playerWidth = 50;
            const float playerHeight = 50;
            var positionPlayerCenter = (new Vector(ClientSize.Width, ClientSize.Height)
                                        + new Vector(playerWidth, playerHeight)) / 2;

            player = new RigidRectangle(positionPlayerCenter, playerWidth, playerHeight, 45);
            playerCenter = new RigidRectangle(positionPlayerCenter, 10, 10, 45);
            cursor = new RigidCircle(positionPlayerCenter, 5);
        }
        
        private void MainLoop(object sender, EventArgs args)
        {
            Move();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var pen = new Pen(Color.Crimson);

            foreach (var formObject in sceneObjects)
                formObject.Draw(g, pen);

            player.Draw(g, pen);
            playerCenter.Draw(g, pen);
            cursor.Draw(g, pen);
        }

        public void Move()
        {
            var deltaX = 0;
            var deltaY = 0;

            if (keyPressed == Keys.Down || keyPressed == Keys.S) deltaY = 4;
            else if (keyPressed == Keys.Left || keyPressed == Keys.A) deltaX = -4;
            else if (keyPressed == Keys.Up || keyPressed == Keys.W) deltaY = -4;
            else if (keyPressed == Keys.Right || keyPressed == Keys.D) deltaX = 4;

            player.Center += new Vector(deltaX, deltaY);
            playerCenter.Center += new Vector(deltaX, deltaY);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            cursor.Center = new Vector(e.X, e.Y);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            pressedKeys.Add(e.KeyCode);
            keyPressed = e.KeyCode;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            pressedKeys.Remove(e.KeyCode);
            keyPressed = pressedKeys.Any() ? pressedKeys.Min() : Keys.None;
        }
    }
}