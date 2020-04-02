using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace App
{
    public class Playground : Form
    {
        private readonly HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private static Keys keyPressed;
        private Point positionPlayer;
        private Point positionPlayerCenter;

        private Point playerWidthHeight;
        private Point positionMouse;

        public Playground()
        {
            SetUpForm();
            SetUpPlayer();
            var timer = new Timer();
            timer.Interval = 15;
            timer.Tick += MainLoop;
            timer.Start();
        }

        private void SetUpForm()
        {
            DoubleBuffered = true;
            Size = new Size(854, 480);
            Text = "NEW GAME";
        }

        private void SetUpPlayer()
        {
            playerWidthHeight = new Point(50, 50);
            positionPlayer = new Point(ClientSize.Width / 2, ClientSize.Height / 2);
        }
        
        private void MainLoop(object sender, EventArgs args)
        {
            Move();
            Invalidate();
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            
            g.FillRectangle(Brushes.Crimson, -100, -100, 200, 200);
            g.FillRectangle(Brushes.Crimson, 300, 300, 50, 50);
            g.FillRectangle(Brushes.Crimson, 100, 300, 50, 50);
            g.FillRectangle(Brushes.Crimson, 300, 100, 1000, 1000);
            
            g.FillRectangle(Brushes.Cyan, positionPlayer.X, positionPlayer.Y, playerWidthHeight.X, playerWidthHeight.Y);
            g.FillEllipse(Brushes.Blue, positionPlayerCenter.X - 5, positionPlayerCenter.Y - 5, 10, 10);
            g.FillEllipse(Brushes.Black, positionMouse.X, positionMouse.Y, 10, 10);
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
        
        public void Move()
        {
            var deltaX = 0;
            var deltaY = 0;

            if (keyPressed == Keys.Down) deltaY = 4;
            else if (keyPressed == Keys.Left) deltaX = -4;
            else if (keyPressed == Keys.Up) deltaY = -4;
            else if (keyPressed == Keys.Right) deltaX = 4;
            
            positionPlayer = new Point(positionPlayer.X + deltaX, positionPlayer.Y + deltaY);
            positionPlayerCenter = new Point(positionPlayer.X + playerWidthHeight.X / 2,positionPlayer.Y + playerWidthHeight.Y / 2);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            positionMouse = new Point(e.X, e.Y);
        }
    }
}