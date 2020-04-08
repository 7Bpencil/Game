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

        public struct keyStates
        {
            public bool up, down, left, right;
        }
        
        public struct tilemapStruct
        {
            public int tilenum;
            public string data1;
            public bool collidable;
        }

        const int COLUMNS = 5; //Why only 5?

        //only for surface
        private Bitmap bmpSurface;
        private PictureBox pbSurface;
        private Graphics gfxSurface;
        private Font fontArial;
        private PointF scrollPos = new PointF(0, 0);
        private PointF oldScrollPos = new PointF(-1, -1);
        private keyStates keyState;
        private Timer timer1;

        private tilemapStruct[] tilemap;
        private Bitmap bmpTiles;
        PointF subtile = new PointF(0, 0);

        private Bitmap bmpScrollBuffer;
        private Graphics gfxScrollBuffer;
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
            this.Text = "Sub-Tile Smooth Scroller";
            this.Size = new Size(900, 700);
            //create font
            fontArial = new Font("Arial Narrow", 18);

            //set up level drawing surface
            bmpSurface = new Bitmap(1024, 786);
            pbSurface = new PictureBox();
            pbSurface.Parent = this;
            pbSurface.BackColor = Color.Black;
            pbSurface.Dock = DockStyle.Fill;
            pbSurface.Image = bmpSurface;
            gfxSurface = Graphics.FromImage(bmpSurface);
            
            //create tilemap
            tilemap = new tilemapStruct[128 * 128];
            bmpTiles = new Bitmap("Images/palette.bmp");
            loadTilemapFile("Levels/level1.level");
            
            //create scroll buffer
            bmpScrollBuffer = new Bitmap(25 * 32 + 64, 19 * 32 + 64);
            gfxScrollBuffer = Graphics.FromImage(bmpScrollBuffer);
            
            //create the timer
            timer1 = new Timer();
            timer1.Interval = 20;
            timer1.Enabled = true;
            timer1.Tick += new EventHandler(timer1_tick);
        }

        private void timer1_tick(object sender, EventArgs e)
        {
            int steps = 4;
            if (keyState.up)
            {
                scrollPos.Y -= steps;
                if (scrollPos.Y < 0) scrollPos.Y = 0;
            }
            if (keyState.down)
            {
                scrollPos.Y += steps;
                if (scrollPos.Y > (127 - 19) * 32) scrollPos.Y = 
                    (127 - 19) * 32;
            }
            if (keyState.left)
            {
                scrollPos.X -= steps;
                if (scrollPos.X < 0) scrollPos.X = 0;
            }
            if (keyState.right)
            {
                scrollPos.X += steps;
                if (scrollPos.X > (127 - 25) * 32) scrollPos.X = 
                    (127 - 25) * 32;
            }
            
            //clear the ground
            //note that this is usually not needed when drawing
            //the game level but this example draws the whole buffer
            //however it's very need
            gfxSurface.Clear(Color.Black);
            
            //update and draw the tiles
            drawScrollBuffer();

            //print scroll position
            string text = "Scroll " + scrollPos.ToString();
            gfxSurface.DrawString(text, fontArial, Brushes.White, 600, 0);
            gfxSurface.DrawString("Sub-tile " + subtile.ToString(), 
                fontArial, Brushes.White, 0, 0);

            //draw a rect representing the actual scroll area
            gfxSurface.DrawRectangle(Pens.Blue, 0, 0, 801, 601);
            gfxSurface.DrawRectangle(Pens.Blue, 1, 1, 801, 601);
            
            pbSurface.Image = bmpSurface;
        }

        public void updateScrollBuffer()
        {
            //fill scroll buffer with tiles
            int tilenum, sx, sy;
            for (int x = 0; x < 26; ++x)
            for (int y = 0; y < 20; ++y)
            {
                sx = (int) (scrollPos.X / 32) + x;
                sy = (int) (scrollPos.Y / 32) + y;
                tilenum = tilemap[sy * 128 + sx].tilenum;
                drawTileNumber(x, y, tilenum, COLUMNS);
            }
        }

        private void loadTilemapFile(string filename)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);
                XmlNodeList nodelist = doc.GetElementsByTagName("tiles");
                foreach (XmlNode node in nodelist)
                {
                    XmlElement element = (XmlElement)node;
                    int index = 0;
                    int value = 0;
                    string data1 = "";
                    bool collidable = false;

                    //read tile index #
                    index = Convert.ToInt32(element.GetElementsByTagName(
                        "tile")[0].InnerText);

                    //read tilenum
                    value = Convert.ToInt32(element.GetElementsByTagName(
                        "value")[0].InnerText);

                    //read data1
                    data1 = Convert.ToString(element.GetElementsByTagName(
                        "data1")[0].InnerText);

                    //read collidable
                    collidable = Convert.ToBoolean(element.
                        GetElementsByTagName("collidable")[0].InnerText);

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
        
        public void drawTileNumber(int x, int y, int tile, int columns)
        {
            int sx = (tile % columns) * 33;
            int sy = (tile / columns) * 33;
            Rectangle src = new Rectangle(sx, sy, 32, 32);
            int dx = x * 32;
            int dy = y * 32;
            gfxScrollBuffer.DrawImage(bmpTiles, dx, dx, src, GraphicsUnit.Pixel);
        }

        public void drawScrollBuffer()
        {
            //fill scroll buffer only when moving
            if (scrollPos != oldScrollPos)
            {
                updateScrollBuffer();
                oldScrollPos = scrollPos;
            }
            
            //calculate sub-tile size
            subtile.X = scrollPos.X % 32;
            subtile.Y = scrollPos.Y % 32;
            
            //create the source rect
            Rectangle source = new Rectangle((int)subtile.X, (int)subtile.Y,
                bmpScrollBuffer.Width, bmpScrollBuffer.Height);
            
            //draw the scroll viewport
            gfxSurface.DrawImage(bmpScrollBuffer, 1, 1, source, GraphicsUnit.Pixel);
        }

        private void Playground_FormClosed(object sender, EventArgs e)
        {
            bmpSurface.Dispose();
            pbSurface.Dispose();
            gfxSurface.Dispose();
            bmpScrollBuffer.Dispose();
            gfxScrollBuffer.Dispose();
            fontArial.Dispose();
            timer1.Dispose();
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