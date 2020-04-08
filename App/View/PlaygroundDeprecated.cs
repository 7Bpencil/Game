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

        const int COLUMNS = 5; //Why only 5?

        //only for surface
        private Bitmap bmpTiles;
        private Bitmap bmpSurface;
        private PictureBox pbSurface;
        private Graphics gfxSurface;
        private Font fontArial;
        private tilemapStruct[] tilemap;
        
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
            this.Size = new Size(800 + 16, 600 + 38);

            //create tilemap
            tilemap = new tilemapStruct[128 * 128];
            
            

            //set up level drawing surface
            bmpSurface = new Bitmap(800, 600);
            pbSurface = new PictureBox();
            pbSurface.Parent = this;
            pbSurface.BackColor = Color.Black;
            pbSurface.Dock = DockStyle.Fill;
            
            //draw first layer with grass
            bmpGrass = (Bitmap) Image.FromFile("Images/grass.bmp");
            pbGrass = new PictureBox();
            pbGrass.Parent = this;
            pbGrass.BackColor = Color.Black;
            pbGrass.Dock = DockStyle.Fill;
            pbGrass.Image = bmpGrass;
            gfxSurface = Graphics.FromImage(bmpGrass);
            gfxSurface.DrawImage(bmpGrass, 0, 0, 800, 600);

            pbSurface.Image = bmpSurface;
            
            
            gfxSurface = Graphics.FromImage(bmpSurface);

            //create font
            fontArial = new Font("Arial Narrow", 8);

            //load tiles bitmap
            bmpTiles = new Bitmap("Images/sprite_map.png");
            
            //load the tilemap
            loadTilemapFile("Levels/level1.level");

            drawTilemap();
        }

        private void loadTilemapFile(string filename)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);
                XmlNodeList nodelist = doc.GetElementsByTagName("tiles");
                foreach (var node in nodelist)
                {
                    XmlElement element = (XmlElement) node;
                    int index = 0;
                    int value = 0;
                    string data1 = "";
                    bool collidable = false;
                    
                    //read tile index #
                    index = Convert.ToInt32(element.GetElementsByTagName("tile")[0].InnerText);
                    
                    //read tilenum
                    value = Convert.ToInt32(element.GetElementsByTagName(
                        "value")[0].InnerText);
                    
                    //read data1
                    data1 = Convert.ToString(element.GetElementsByTagName(
                        "data1")[0].InnerText);
                    
                    //read collidable
                    collidable = Convert.ToBoolean(element.GetElementsByTagName(
                        "collidable")[0].InnerText);

                    tilemap[index].tilenum = 129;
                    tilemap[index].data1 = data1;
                    tilemap[index].collidable = collidable;
                }
            }
            catch (Exception es)
            {
                MessageBox.Show(es.Message);
            }
        }

        private void drawTilemap()
        {
            for (int x = 0; x < 25; ++x)
            for (int y = 0; y < 19; ++y)
            {
                drawTileNumber(x, y, tilemap[y * 128 + x].tilenum);
            }
            
        }

        public void drawTileNumber(int x, int y, int tile)
        {
            //draw tile
            int sx = (tile % COLUMNS) * 33;//columns it mean columns with tiles in palette
            int sy = (tile / COLUMNS) * 33;//
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