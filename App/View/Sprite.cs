using System;
using System.Drawing;
using System.Runtime.InteropServices;
using App.Engine.PhysicsEngine;

namespace App.View
{
    public class Sprite
    {
        
        private Vector position;
        private Vector velocity;
        private Size size;
        private Bitmap bitmap;
        private bool alive;
        private int columns;
        private int totalFrames;
        private int currentFrame;
        private Vector center;
        
        private int lastTime;
        private int animationRate;

        public Sprite()
        {
            position = Vector.ZeroVector;
            velocity = Vector.ZeroVector;
            size = new Size(64, 64);
            bitmap = null;
            alive = true;
            columns = 1;
            totalFrames = 1;//?
            currentFrame = 106;
            lastTime = 0;
            animationRate = 30;
        }

        public Sprite(Vector position, bool alive, int currentFrame, int animationRate, int columns, Size size) // new constructor
        {
            this.size = size;
            this.position = position;
            this.center = new Vector(position.X + size.Width / 2, position.Y + size.Height / 2);
            this.alive = alive;
           // this.totalFrames = totalFrames;
            this.animationRate = animationRate;
            this.currentFrame = currentFrame;
            this.columns = columns;
        }

        public bool Alive
        { get => alive; set => alive = value; }

        public Bitmap Image
        { get => bitmap; set => bitmap = value; }

        public Vector Position
        { get => position; set => position = value; }

        public Vector Velocity
        { get => velocity; set => velocity = value; }

        public float X
        { get => position.X; set => position.X = value; }

        public float Y
        { get => position.Y; set => position.Y = value; }

        public Size Size
        { get => size; set => size = value; }

        public int Width
        { get => size.Width; set => size.Width = value; }

        public int Height
        { get => size.Height; set => size.Height = value; }

        public int Columns
        { get => columns; set => columns = value; }

        public int TotalFrames
        { get  => totalFrames; set  =>totalFrames = value; }

        public int CurrentFrame
        { get  => currentFrame; set => currentFrame = value; }

        public Vector Center
        {
            get  => center;
            set
            {
                center = value;
                position.X = center.X - size.Width / 2;
                position.Y = center.Y - size.Height / 2;
            }
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

        /*public void Animate(int startFrame, int endFrame)
        {
            //do we even need to animate?
            if (totalFrames <= 0) return;
            
            //check animation timing
            var time = Environment.TickCount;
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
        }*/

        public void Draw(Graphics device)
        {
            var frame = new Rectangle
            {
                X = currentFrame % columns * size.Width,
                Y = currentFrame / columns * size.Height,
                Width = size.Width,
                Height = size.Height
            };
            device.DrawImage(bitmap, Bounds, frame, GraphicsUnit.Pixel);
        }

        public RectangleF Bounds => new RectangleF(X, Y, Width, Height);
        
        public bool IsColliding(ref Sprite other)
        {
            return Bounds.IntersectsWith(other.Bounds);
        }
    }
}