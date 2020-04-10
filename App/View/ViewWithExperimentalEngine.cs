/*using System;
using System.Drawing;
using System.Windows.Forms;

namespace App.View
{
    public class ViewWithExperimentalEngine : Form
    {
        private System.ComponentModel.IContainer components = null;
        private bool gameOver;
        private int startTime;
        private int currentTime;
        public Game game;
        public Bitmap dragonImage;
        public Sprite dragonSprite;
        public Bitmap grass;
        public int frameCount;
        public int frameTimer;
        public float frameRate;
        public PointF velocity;
        public int direction = 2;


        public ViewWithExperimentalEngine()
        {
            ClientSize = new Size(1280, 720);
            Load += PlaygroundDeprecated_Load;
            FormClosed += Playground_FormClosed;
            KeyDown += PlaygroundDeprecated_KeyDown;
            ResumeLayout(false);
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
            Main();
        }

        private void PlaygroundDeprecated_KeyDown(object sender, KeyEventArgs e)
        {
            Game_KeyPressed(e.KeyCode);
        }

        public void Game_Init()
        {
            Text = "Sprite Drawing Demo";
            grass = (Bitmap) Image.FromFile(@"Images/grass.bmp");
            dragonImage = (Bitmap) Image.FromFile(@"Images/dragon.png");
            dragonSprite = new Sprite(ref game);
            dragonSprite.Image = dragonImage;
            dragonSprite.Width = 256;
            dragonSprite.Height = 256;
            dragonSprite.Columns = 8;
            dragonSprite.TotalFrames = 64;
            dragonSprite.AnimationRate = 20;
            dragonSprite.X = 250;
            dragonSprite.Y = 150;
        }

        public void Game_Update(int time)
        {
        }

        public void Game_Draw()
        {
            //draw background
            game.DrawBitmap(ref grass, 0, 0, 800, 600);

            switch (direction)
            {
                case 0:
                    velocity = new Point(0, -1);
                    break;
                case 2:
                    velocity = new Point(1, 0);
                    break;
                case 4:
                    velocity = new Point(0, 1);
                    break;
                case 6:
                    velocity = new Point(-1, 0);
                    break;
            }

            dragonSprite.X += velocity.X;
            dragonSprite.Y += velocity.Y;

            dragonSprite.Animate(direction * 8 + 1, direction * 8 + 7);
            dragonSprite.Draw();
        }

        public void Game_End()
        {
            dragonImage = null;
            dragonSprite = null;
            grass = null;
        }

        public void Game_KeyPressed(Keys key)
        {
            switch (key)
            {
                case Keys.Escape:
                    Shutdown();
                    break;
                case Keys.Up:
                    direction = 0;
                    break;
                case Keys.Right:
                    direction = 2;
                    break;
                case Keys.Down:
                    direction = 4;
                    break;
                case Keys.Left:
                    direction = 6;
                    break;
            }
        }

        private void Playground_FormClosed(object sender, EventArgs e)
        {
            Shutdown();
        }

        public void Shutdown()
        {
            gameOver = true;
        }

        public void Main()
        {
            Form form = this;
            game = new Game(ref form, 800, 600);
            Game_Init();
            while (!gameOver)
            {
                //update time
                currentTime = Environment.TickCount;

                //let gameplay code update
                Game_Update(time: currentTime - startTime);

                //refresh at 60 fps
                if (currentTime > startTime + 16)
                {
                    //update time
                    startTime = currentTime;

                    //let gameplay code draw
                    Game_Draw();

                    //give the form some cycles
                    Application.DoEvents();
                    game.Update();
                }

                frameCount += 1;
                if (currentTime > frameTimer + 1000)
                {
                    frameTimer = currentTime;
                    frameRate = frameCount;
                    frameCount = 0;
                }
            }

            Game_End();
            Application.Exit();
        }
    }
}*/