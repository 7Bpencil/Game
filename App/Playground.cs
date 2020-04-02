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
        private Point positionMouse;

        public Playground()
        {
            var timer = new Timer();
            timer.Interval = 15;
            timer.Tick += MainLoop;
            timer.Start();
        }
        
        private void MainLoop(object sender, EventArgs args)
        {
            Move();
            Invalidate();
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.FillRectangle(Brushes.Crimson, positionPlayer.X, positionPlayer.Y, 50, 50);
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

            if (keyPressed == Keys.Down) deltaY = 2;
            else if (keyPressed == Keys.Left) deltaX = -2;
            else if (keyPressed == Keys.Up) deltaY = -2;
            else if (keyPressed == Keys.Right) deltaX = 2;
            
            positionPlayer = new Point(positionPlayer.X + deltaX, positionPlayer.Y + deltaY);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            positionMouse = new Point(e.X, e.Y);
        }
    }
}