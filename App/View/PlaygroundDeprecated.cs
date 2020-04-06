using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace App.View
{

    public partial class PlaygroundDeprecated : Form
    {
        private readonly HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private static Keys keyPressed;
        private Point positionMouse;
        private Point positionPlayer;
        private Point positionPlayerCenter;
        private Point playerWidthHeight;
        private System.ComponentModel.IContainer components = null;
        private PictureBox pb;
        private Bitmap surface = null;
        private Graphics device;
        private Bitmap image = null;
        private PictureBox pb1;

        public PlaygroundDeprecated()
        {
            this.pb1 = new System.Windows.Forms.PictureBox();
            this.pb1.Parent = this;//it is really need for drawing and create a scene
            this.pb1.BackColor = System.Drawing.Color.Black;
            this.pb1.Location = new System.Drawing.Point(0, 0);
            this.pb1.Name = "box";
            this.pb1.Size = new System.Drawing.Size(865, 706);
            this.pb1.TabStop = false; 
            ((System.ComponentModel.ISupportInitialize)(this.pb1)).BeginInit();
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
            this.Size = new Size(865, 706);//this is size of image

            surface = new Bitmap(this.Size.Width, this.Size.Height);
            pb1.Image = surface;
            device = Graphics.FromImage(surface);
            
            var path = System.IO.Path.GetFullPath(@"img/grass.bmp");
            image = LoadBitmap(path);
            device.DrawImage(image, 0, 0);
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
        
        
        private void Game_FormClosed(object sender, EventArgs e)
        {
            device.Dispose();
            surface.Dispose();
        }
    }
}