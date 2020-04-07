using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using App.View.Renderings;

namespace App.View
{

    public partial class PlaygroundDeprecated : Form
    {
        private System.ComponentModel.IContainer components = null;
        public Game game;
        public Bitmap planet;

        public PlaygroundDeprecated()
        {
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Load += new System.EventHandler(this.Game_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Playground_FormClosed);
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
            this.Text = "Game framework demo";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            //this.Size = new Size(865, 706);//this is size of image
            
            //create game object
            game = new Game(this, 600, 500);
            
            //load bitmap
            planet = game.LoadBitmap("planet.bmp");
            if (planet == null)
            {
                MessageBox.Show("Error loading planet.bmp");
                Environment.Exit(0);
            }
            
            //draw the bitmap 
            game.Device.DrawImage(planet, 10, 10);
            game.Device.DrawImage(planet, 400, 10, 100, 100);
            game.Update();
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
        
        
        private void Playground_FormClosed(object sender, EventArgs e)
        {
            game = null;
        }
    }
}