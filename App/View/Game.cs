using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace App.View.Renderings
{
    public class Game
    {
        private System.ComponentModel.IContainer components = null;
        public Graphics device;
        public Bitmap surface;
        public PictureBox pb; 
        public Form frm;
        public Font font;
        public bool gameOver;

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
            //pb.Location = new Point(0, 0);
            pb.Size = new Size(width, height);
            pb.Dock = DockStyle.Fill;
            pb.BackColor = Color.Black;
            
            //create device
            surface = new Bitmap(frm.Size.Width, frm.Size.Height);
            pb.Image = surface;
            device = Graphics.FromImage(surface);
            
            //set the default font
            SetFont("Arial", 18, FontStyle.Regular);
        }
        
        /*
         * font support with several Print variations
         */
        public void SetFont(string name, int size, FontStyle style)
        {
            font = new Font(name, size, style, GraphicsUnit.Pixel);
        }

        public void Print(int x, int y, string text, Brush color)
        {
            
            Device.DrawString(text, font, color, (float)x, (float)y);
        }

        public void Print(Point pos, string text, Brush color)
        {
            Print(pos.X, pos.Y, text, color);
        }

        public void Print(int x, int y, string text)
        {
            Print(x, y, text, Brushes.White);
        }

        public void Print(Point pos, string text)
        {
            Print(pos.X, pos.Y, text);
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
        
        /*
        * Bitmap support functions
        */
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

        public void DrawBitmap(ref Bitmap bmp, float x, float y)
        {
            device.DrawImage(bmp, (int)x, (int)y);
        }

        public void DrawBitmap(ref Bitmap bmp, float x, float y, int width, int height)
        {
            device.DrawImage(bmp, (int)x, (int)y, width, height);
        }

        public void DrawBitmap(ref Bitmap bmp, Point pos)
        {
            device.DrawImage(bmp, pos);
        }

        public void DrawBitmap(ref Bitmap bmp, Point pos, Size size)
        {
            device.DrawImage(bmp, pos.X, pos.Y, size.Width, size.Height);
        }

        public void Update()
        {
            //refresh the drawing surface
            pb.Image = surface;
        }
    }
}