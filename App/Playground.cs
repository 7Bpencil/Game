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
        private Point position;

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
            g.FillRectangle(Brushes.Crimson, position.X, position.Y, 50, 50);
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
            
            position = new Point(position.X + deltaX, position.Y + deltaY);
        }
    }
}