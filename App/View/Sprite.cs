using System;
using System.Drawing;
using App.View.Renderings;

namespace App.View
{
    public class Sprite
    {
        public enum AnimateDir
        {
            NONE = 0,
            FORWARD = 1,
            BACKWARD = -1
        }
        
        public enum AnimateWrap
        {
            WRAP = 0,
            BOUNCE = 1
        }

        private Game game;
        private PointF position;
        private PointF velocity;
        private Size size;
        private Bitmap bitmap;
        private bool alive;
        private int columns;
        private int totalFrames;
        private int currentFrame;
        private AnimateDir animationDir;
        private AnimateWrap animationWrap;
        private int lastTime;
        private int animationRate;

        public Sprite(ref Game game)
        {
            this.game = game;
            position = new PointF(0, 0);
            velocity = new PointF(0, 0);
            size = new Size(0, 0);
            bitmap = null;
            alive = true;
            columns = 1;
            totalFrames = 1;//?
            currentFrame = 0;
            animationDir = AnimateDir.FORWARD;
            animationWrap = AnimateWrap.WRAP;
            lastTime = 0;
            animationRate = 30;
        }

        public bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }

        public Bitmap Image
        {
            get { return bitmap; }
            set { bitmap = value; }
        }

        public PointF Position
        {
            get { return position; }
            set { position = value; }
        }

        public PointF Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public float X
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public float Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        public Size Size
        {
            get { return size; }
            set { size = value; }
        }

        public int Width
        {
            get { return size.Width; }
            set { size.Width = value; }
        }

        public int Height
        {
            get { return size.Height; }
            set { size.Height = value; }
        }

        public int Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        public int TotalFrames
        {
            get { return totalFrames; }
            set { totalFrames = value; }
        }

        public int CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }

        public AnimateDir AnimateDirection
        {
            get { return animationDir; }
            set { animationDir = value; }
        }

        public AnimateWrap AnimateWrapMode
        {
            get { return animationWrap; }
            set { animationWrap = value; }
        }

        public int AnimationRate
        {
            get => 1000 / animationRate;
            set
            {
                if (value == 0) value = 1;
                animationRate = 1000 / value;
            }
        }

        public void Animate(int startFrame, int endFrame)
        {
            //do we even need to animate?
            if (totalFrames <= 0) return;
            
            //check animation timing
            int time = Environment.TickCount;
            if (time > lastTime + animationRate) //it need to resolve framerate and animation radnering
            {
                lastTime = time;
                
                //go to next frame
                currentFrame += (int) animationDir;//complexity
                switch (animationWrap)//too hard, too complexity
                {
                    case AnimateWrap.WRAP:
                        if (currentFrame < startFrame)
                            currentFrame = endFrame;
                        else if (currentFrame > endFrame)
                            currentFrame = startFrame;
                        break;
                    case AnimateWrap.BOUNCE:
                        if (currentFrame < startFrame)
                        {
                            currentFrame = startFrame;
                            animationDir = AnimateDir.FORWARD;
                        }
                        else if (currentFrame > endFrame)
                        {
                            currentFrame = startFrame;
                            animationDir = AnimateDir.BACKWARD;
                        }

                        break;
                }
            }
        }

        public void Draw()
        {
            Rectangle frame = new Rectangle();
            frame.X = (currentFrame % columns) * size.Width;
            frame.Y = (currentFrame / columns) * size.Height;
            frame.Width = size.Width;
            frame.Height = size.Height;
            game.Device.DrawImage(bitmap, Bounds, frame, GraphicsUnit.Pixel);
        }

        public Rectangle Bounds
        {
            get
            {
                Rectangle rect = new Rectangle((int)position.X, (int)position.Y, size.Width, size.Height);
                return rect;
            }
        }

        public bool IsColliding(ref Sprite other)
        {
            bool collision = Bounds.IntersectsWith(other.Bounds);
            return collision;
        }
    }
}