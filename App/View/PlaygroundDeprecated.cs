using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using App.Model.LevelData;
using App.Model.Parser;
using App.View.Renderings;
using Timer = System.Windows.Forms.Timer;

namespace App.View
{

    public partial class PlaygroundDeprecated : Form
    {
        private System.ComponentModel.IContainer components = null;
        //game variables
        private static Size paletteSize = new Size(64, 64);
        private static Size cameraSize = new Size(16, 9);
        private static Size sceneSize = new Size(30, 30);
        const int COLUMNS = 9; //Why only 5?

        public struct keyStates
        {
            public bool up, down, left, right;
        }
        
        private Level level;
        
        

        //only for surface
        private Bitmap bmpTiles;
        private Bitmap bmpSurface;
        private PictureBox pbSurface;
        private Graphics gfxSurface;
        private Font fontArial;
        private PointF scrollPos = new PointF(0, 0);
        private PointF oldScrollPos = new PointF(-1, -1);
        PointF subtile = new PointF(0, 0);
        private keyStates keyState;
        private Timer timer1;

        //only for scene
        private Bitmap bmpScene;
        private Bitmap bmpScrollBuffer;
        private Graphics gfxScrollBuffer;
        
        public PlaygroundDeprecated()
        {
            this.ClientSize = new Size(cameraSize.Width * paletteSize.Width, cameraSize.Height * paletteSize.Height);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Load += new EventHandler(this.PlaygroundDeprecated_Load);
            this.FormClosed += new FormClosedEventHandler(this.Playground_FormClosed);
            this.KeyUp += new KeyEventHandler(this.PlaygroundDeprecated_KeyUp);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PlaygroundDeprecated_KeyDown);
            this.ResumeLayout(false);
        }
        private void PlaygroundDeprecated_Load(object sender, EventArgs e)
        {
            //font and form
            this.Text = "Level Viewer";
            this.Size = new Size(cameraSize.Width * paletteSize.Width, cameraSize.Height * paletteSize.Height);
            fontArial = new Font("Arial Narrow", 8);

            //set up level drawing surface
            bmpSurface = new Bitmap(sceneSize.Width * paletteSize.Width, sceneSize.Height * paletteSize.Height);
            
            pbSurface = new PictureBox();
            pbSurface.Parent = this;
            pbSurface.BackColor = Color.Black;
            pbSurface.Dock = DockStyle.Fill;
            pbSurface.Image = bmpSurface;
            
            gfxSurface = Graphics.FromImage(bmpSurface);

            //load tiles bitmap
            bmpTiles = new Bitmap("Images/sprite_map.png");
            
            try
            { 
                level = LevelParser.ParseLevel("Levels/secondTry.tmx");
            }
            catch (Exception es)
            {
                MessageBox.Show(es.Message);
            }
            
            //create scroll buffer
            bmpScrollBuffer = new Bitmap(cameraSize.Width * paletteSize.Width, cameraSize.Height * paletteSize.Height);
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
                if (scrollPos.Y > (sceneSize.Height - 1) * paletteSize.Height) scrollPos.Y = 
                    (sceneSize.Height - 1) * paletteSize.Height;
            }
            if (keyState.left)
            {
                scrollPos.X -= steps;
                if (scrollPos.X < 0) scrollPos.X = 0;
            }
            if (keyState.right)
            {
                scrollPos.X += steps;
                if (scrollPos.X > (sceneSize.Width - 1) * paletteSize.Width) scrollPos.X = 
                    (sceneSize.Width - 1) * paletteSize.Width;
            }
            
            //clear the ground
            //note that this is usually not needed when drawing
            //the game level but this example draws the whole buffer
            //however it's very need
            gfxSurface.Clear(Color.Black);
            
            //update and draw the tiles
            drawScrollBuffer();
            
            pbSurface.Image = bmpSurface;
        }   
        
        public void updateScrollBuffer()
        {
            //fill scroll buffer with tiles
            int tilenum, sx, sy;
            for (int i = 0; i < level.Layers.Count; ++i)
            {
                for (int x = 0; x <= cameraSize.Width; ++x) //here may be mistake!!!
                for (int y = 0; y <= cameraSize.Height; ++y)
                {
                    sx = (int) (scrollPos.X / paletteSize.Width) + x;
                    sy = (int) (scrollPos.Y / paletteSize.Height) + y;
                    tilenum = level.Layers[i].Tiles[sy * sceneSize.Height + sx];
                    if (tilenum != 0) 
                        drawTileNumber(x, y, tilenum - 1);
                }
            }
        }
        
        public void drawTileNumber(int x, int y, int tile)
        {
            //draw tile
            int sx = (tile % COLUMNS) * paletteSize.Width;//columns it mean columns with tiles in palette
            int sy = (tile / COLUMNS) * paletteSize.Height;//
            Rectangle src = new Rectangle(sx, sy, paletteSize.Width, paletteSize.Height);
            int dx = x * paletteSize.Width;
            int dy = y * paletteSize.Height;
            gfxSurface.DrawImage(bmpTiles, dx, dy, src, GraphicsUnit.Pixel);
            //save changes
            pbSurface.Image = bmpSurface;
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
            subtile.X = scrollPos.X % paletteSize.Width;
            subtile.Y = scrollPos.Y % paletteSize.Height;
            
            //create the source rect
            Rectangle source = new Rectangle((int)subtile.X, (int)subtile.Y,
                bmpScrollBuffer.Width, bmpScrollBuffer.Height);
            
            //draw the scroll viewport
            gfxSurface.DrawImage(bmpScrollBuffer, 0, 0, source, GraphicsUnit.Pixel);
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
        
        //It need only stuff
        
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
            bmpScrollBuffer.Dispose();
            gfxScrollBuffer.Dispose();
            fontArial.Dispose();
            timer1.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}