using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using App.View.Renderings;

namespace App.View
{

    public partial class PlaygroundDeprecated : Form
    {
        private System.ComponentModel.IContainer components = null;
        private Size palette_size = new Size(64, 64);

        public struct tilemapStruct
        {
            public int tilenum;
            public string data1;
            public bool collidable;
        }

        const int COLUMNS = 9; //Why only 5?
        private const int ROWS = 16;

        //only for surface
        private Bitmap bmpTiles;
        private Bitmap bmpSurface;
        private PictureBox pbSurface;
        private Graphics gfxSurface;
        private Font fontArial;

        //only for grass
        private Bitmap bmpGrass;
        private PictureBox pbGrass;
        private Graphics gfxGrass;
        

        public PlaygroundDeprecated()
        {
            this.ClientSize = new System.Drawing.Size(824, 582);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Load += new System.EventHandler(this.PlaygroundDeprecated_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Playground_FormClosed);
            this.KeyUp += new KeyEventHandler(this.PlaygroundDeprecated_KeyUp);
            this.ResumeLayout(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void PlaygroundDeprecated_Load(object sender, EventArgs e)
        {
            this.Text = "Level Viewer";
            this.Size = new Size(800, 600);

            //set up level drawing surface
            bmpSurface = new Bitmap(800, 600);
            pbSurface = new PictureBox();
            pbSurface.Parent = this;
            pbSurface.BackColor = Color.Black;
            pbSurface.Dock = DockStyle.Fill;
            
            
            pbSurface.Image = bmpSurface;
            
            
            gfxSurface = Graphics.FromImage(bmpSurface);
            
            //draw first layer with grass
            bmpGrass = (Bitmap) Image.FromFile("Images/grass.bmp");
            gfxSurface.DrawImage(bmpGrass, 0, 0);
            

            //create font
            fontArial = new Font("Arial Narrow", 8);

            //load tiles bitmap
            bmpTiles = new Bitmap("Images/sprite_map.png");
            
        }
        

        public void drawTileNumber(int x, int y, int tile)
        {
            //draw tile
            int sx = (tile % COLUMNS) * ROWS;//columns it mean columns with tiles in palette
            int sy = (tile / COLUMNS) * ROWS;//
            Rectangle src = new Rectangle(sx, sy, palette_size.Width, palette_size.Height);
            int dx = x * palette_size.Width;
            int dy = y * palette_size.Height;
            gfxSurface.DrawImage(bmpTiles, dx, dy, src, GraphicsUnit.Pixel);
            
            //save changes
            pbSurface.Image = bmpSurface;
        }
        
        public Bitmap LoadBitmap(string filename)
        {
            Bitmap bmp = null;
            try
            {
                bmp = new Bitmap(filename);
            }
            catch (Exception ex)
            {
            }

            return bmp;
        }


        private void Playground_FormClosed(object sender, EventArgs e)
        {
            bmpSurface.Dispose();
            pbSurface.Dispose();
            gfxSurface.Dispose();
        }

        private void PlaygroundDeprecated_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Application.Exit();
        }
    }
}