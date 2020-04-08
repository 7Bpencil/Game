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

        public struct tilemapStruct
        {
            public int tilenum;
            public string data1;
            public bool collidable;
        }

        public struct keyStates
        {
            public bool up, down, left, right;
        }
        

        const int COLUMNS = 5; //Why only 5?

        //only for surface
        private Bitmap bmpTiles;
        private Bitmap bmpSurface;
        private PictureBox pbSurface;
        private Graphics gfxSurface;
        private Font fontArial;
        private tilemapStruct[] tilemap;
        private PointF scrollPos = new PointF(0, 0);
        private PointF oldScrollPos = new PointF(-1, -1);
        private keyStates keyState;
        private Timer timer1;

        public PlaygroundDeprecated()
        {
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Load += new System.EventHandler(this.PlaygroundDeprecated_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Playground_FormClosed);
            this.KeyUp += new KeyEventHandler(this.PlaygroundDeprecated_KeyUp);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PlaygroundDeprecated_KeyDown);
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
            this.Text = "Smooth Scroller";
            this.Size = new Size(800 + 16, 600 + 38);

            //create tilemap
            tilemap = new tilemapStruct[128 * 128];

            //set up level drawing surface
            bmpSurface = new Bitmap(800, 600);
            pbSurface = new PictureBox();
            pbSurface.Parent = this;
            pbSurface.BackColor = Color.Black;
            pbSurface.Dock = DockStyle.Fill;
            pbSurface.Image = bmpSurface;
            gfxSurface = Graphics.FromImage(bmpSurface);

            //create font
            fontArial = new Font("Arial Narrow", 18);

            //load tiles bitmap
            bmpTiles = new Bitmap("Images/palette.bmp");
            
            //load the tilemap
            loadTilemapFile("Levels/level1.level");

            //create the timer
            timer1 = new Timer();
            timer1.Interval = 20;
            timer1.Enabled = true;
            timer1.Tick += new EventHandler(timer1_tick);
        }

        private void timer1_tick(object sender, EventArgs e)
        {
            if (keyState.up)
            {
                scrollPos.Y -= 1;
                if (scrollPos.Y < 0) scrollPos.Y = 0;
            }
            if (keyState.down)
            {
                scrollPos.Y += 1;
                if (scrollPos.Y > 127 - 19) scrollPos.Y = 127 - 19;
            }
            if (keyState.left)
            {
                scrollPos.X -= 1;
                if (scrollPos.X < 0) scrollPos.X = 0;
            }
            if (keyState.right)
            {
                scrollPos.X += 1;
                if (scrollPos.X > 127 - 25) scrollPos.X = 127 - 25;
            }
            
            drawTilemap();
            string text = "Scroll " + scrollPos.ToString();
            gfxSurface.DrawString(text, fontArial, Brushes.White, 10, 10);
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

                    tilemap[index].tilenum = value;
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
            int tilenum, sx, sy;
            for (int x = 0; x < 25; ++x)
            for (int y = 0; y < 19; ++y)
            {
                sx = (int) scrollPos.X + x;
                sy = (int) scrollPos.Y + y;
                tilenum = tilemap[sy * 128 + sx].tilenum;
                drawTileNumber(x, y, tilenum);
            }
            string text = "Scroll " + scrollPos.ToString();
            gfxSurface.DrawString(text, fontArial, Brushes.White, 10, 10);
        }

        public void drawTileNumber(int x, int y, int tile)
        {
            //draw tile
            int sx = (tile % COLUMNS) * 33;//columns it mean columns with tiles in palette
            int sy = (tile / COLUMNS) * 33;//
            Rectangle src = new Rectangle(sx, sy, 32, 32);
            int dx = x * 32;
            int dy = y * 32;
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
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Application.Exit();
                    break;
            
                case Keys.Up:
                case Keys.W:
                    keyState.up = false;
                    break;

                case Keys.Down:
                case Keys.S:
                    keyState.down = false;
                    break;

                case Keys.Left:
                case Keys.A:
                    keyState.left = false;
                    break;

                case Keys.Right:
                case Keys.D:
                    keyState.right = false;
                    break;
            }
        }

        private void PlaygroundDeprecated_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.W:
                    keyState.up = true;
                    break;

                case Keys.Down:
                case Keys.S:
                    keyState.down = true;
                    break;

                case Keys.Left:
                case Keys.A:
                    keyState.left = true;
                    break;

                case Keys.Right:
                case Keys.D:
                    keyState.right = true;
                    break;
            }
        }
    }
}