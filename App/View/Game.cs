/*
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace App.View
{
    public class Game
    {
        private System.ComponentModel.IContainer components = null;
        private Graphics device;
        private Bitmap surface;
        private PictureBox pb;
        private Form frm;
        private Font font;
        private bool gameOver;

        public Game(ref Form form, int width, int height)
        {
            Trace.WriteLine("this is game constructor");
            
            //set prop
            frm = form;
            frm.FormBorderStyle = FormBorderStyle.FixedSingle;
            frm.MaximizeBox = false;
            //adjust size for window
            frm.Size = new Size(width + 6, height + 28);
            
            //create a picturebox
            pb = new PictureBox();
            pb.Parent = frm;
            //
            pb.Location = new Point(0, 0);
            pb.Size = new Size(width, height);
            pb.Dock = DockStyle.Fill;
            pb.BackColor = Color.Black;
            
            //create device
            surface = new Bitmap(frm.Size.Width, frm.Size.Height);
            pb.Image = surface;
            device = Graphics.FromImage(surface);
        }

        ~Game()//Здесь может все упасть без переопределенного метода dispose
        {
            device.Dispose();
            surface.Dispose();
            pb.Dispose();
        }

        public Graphics Device => device;

        public void DrawBitmap(ref Bitmap bmp, float x, float y, int width, int height)
        {
            device.DrawImageUnscaled(bmp, (int)x, (int)y, width, height);
        }

        public void Update()
        {
            pb.Image = surface;
        }
    }
}*/