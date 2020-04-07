using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace App.View.Renderings
{
    public class Game
    {
        private System.ComponentModel.IContainer components = null;
        private Graphics device;
        private Bitmap surface;
        private PictureBox pb;
        private Form frm;

        public Game(PlaygroundDeprecated form, int width, int height)
        {
            Trace.WriteLine("this is game constructor");
            
            //set prop
            frm = form;
            frm.FormBorderStyle = FormBorderStyle.FixedSingle;
            frm.MaximizeBox = false;
            frm.Size = new Size(width, height);
            
            //create a picturebox
            pb = new PictureBox();
            pb.Parent = frm;
            pb.Dock = DockStyle.Fill;
            pb.BackColor = Color.Black;
            
            //create device
            surface = new Bitmap(frm.Size.Width, frm.Size.Height);
            pb.Image = surface;
            device = Graphics.FromImage(surface);
        }
        
        //not work without super
        /*protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }*/
        
        ~Game()//Здесь может все упасть без переопределенного метода dispose
        {
            Trace.WriteLine("Game class destructor");
            device.Dispose();
            surface.Dispose();
            pb.Dispose();
        }

        public Graphics Device
        {
            get => device;
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

        public void Update()
        {
            //refresh the drawing surface
            pb.Image = surface;
        }
    }
}