using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using App.Engine;

namespace App.View
{

    public partial class PlaygroundDeprecated : Form
    {
        private System.ComponentModel.IContainer components = null;
        private PictureBox pb;
        private Bitmap surface = null;
        private Graphics device;
        private Bitmap image = null;
        private Timer timer;
        private Random rand;
        private PictureBox pb1;

        public PlaygroundDeprecated()
        {
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Load += new System.EventHandler(this.Game_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Game_FormClosed);
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Game_Load(object sender, EventArgs e)
        {
            //set up the form
            this.Text = "Text drawing Demo";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Size = new Size(865, 706);

            pb = new PictureBox(); 
            pb.Parent = this;
            pb.Dock = DockStyle.Fill;
            pb.BackColor = Color.Black;

            surface = new Bitmap(this.Size.Width, this.Size.Height);
            device = Graphics.FromImage(surface);
            
            rand = new Random();

            timer = new Timer();
            timer.Interval = 20;
            timer.Enabled = true;
            timer.Tick += new System.EventHandler(TimerTick);
            
        }

        public Bitmap LoadBitmap(string filename)
        {
            Bitmap bmp = null;
            try
            {
                bmp = new Bitmap(filename);
            }
            catch (Exception ex) { }
            return bmp;
        }
        
        //demo for lines drawing
        public void drawLine()
        {
            //make a random color
            int A = rand.Next(0, 255);
            int R = rand.Next(0, 255);
            int G = rand.Next(0, 255);
            int B = rand.Next(0, 255);
            Color color = Color.FromArgb(A, R, G, B);

            //make pen out of color
            int width = rand.Next(2, 8);
            Pen pen = new Pen(color, width);

            //random line ends
            int x1 = rand.Next(1, this.Size.Width);
            int y1 = rand.Next(1, this.Size.Height);
            int x2 = rand.Next(1, this.Size.Width);
            int y2 = rand.Next(1, this.Size.Height);

            //draw the line
            device.DrawLine(pen, x1, y1, x2, y2);

            //refresh the drawing surface
            pb.Image = surface;

        }
        
        //demo for ellipse drawing
        private void drawEllipse()
        {
            //make a random color
            int A = rand.Next(0, 255);
            int R = rand.Next(0, 255);
            int G = rand.Next(0, 255);
            int B = rand.Next(0, 255);
            Color color = Color.FromArgb(A, R, G, B);

            //make pen out of color
            int width = rand.Next(2, 20);
            int height = rand.Next(2, 20);
            Pen pen = new Pen(color, width);

            //random line ends
            int x = rand.Next(1, this.Size.Width - 50);
            int y = rand.Next(1, this.Size.Height - 50);

            //draw the rectangle
            device.DrawEllipse(pen, x, y, width, height);

            //refresh the drawing surface
            pb.Image = surface;
        }

        public void TimerTick(object source, EventArgs e)
        {
            drawEllipse();
        }
        
        private void Game_FormClosed(object sender, EventArgs e)
        {
            device.Dispose();
            surface.Dispose();
            timer.Dispose();
        }
    }
}