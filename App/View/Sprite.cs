using System;
using System.Drawing;
using App.Engine.PhysicsEngine;

namespace App.View
{
    public class Sprite
    {
        private Vector center;
        private Vector _topLeft;
        private Vector velocity;
        private Vector prevPos;
        private Size size;
        private Bitmap bitmap;
        private bool alive;
        private int columns;
        private int currentFrame;
        private int lastTime;
        private int animationRate;

        public Sprite()
        {
            velocity = Vector.ZeroVector;
            size = new Size(64, 64);
            bitmap = null;
            alive = true;
            columns = 1;
            currentFrame = 0;
            lastTime = 0;
            animationRate = 1500 / 3;
        }

        public Vector Center
        {
            get => center;
            set
            {
                center = value;
                _topLeft = center - new Vector(Width / 2, Height / 2);
            }
        }

        public bool Alive
        { get => alive; set => alive = value; }

        public Bitmap Image
        { get => bitmap; set => bitmap = value; }
        
        public int StartFrame { get; set; }
        public int EndFrame { get; set; }


        public Vector TopLeft => _topLeft;

        public Vector PreviousPosition
        { get => prevPos; set => prevPos = value; }
        
        public Vector Velocity
        { get => velocity; set => velocity = value; }

        public Size Size
        { get => size; set => size = value; }

        public int Width
        { get => size.Width; set => size.Width = value; }

        public int Height
        { get => size.Height; set => size.Height = value; }

        public int Columns
        { get => columns; set => columns = value; }

        /*public int CurrentFrame
        { get  => currentFrame; set => currentFrame = value; }*/
        
        public int AnimationRate
        {
            get => 1000 / animationRate;
            set
            {
                if (value == 0) value = 1;
                animationRate = 1000 / value;
            }
        }

        public void Animate(Graphics device)
        {
            if (currentFrame < StartFrame || currentFrame > EndFrame)
                currentFrame = StartFrame;
            Draw(device);
            //check animation timing
            var time = Environment.TickCount;
            if (time > lastTime + animationRate && !Equals(prevPos, _topLeft)) //it need to resolve framerate and animation rendering
            {
                lastTime = time;
                prevPos = _topLeft;
                currentFrame++;
            }
        }

        private void Draw(Graphics device)
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

        public RectangleF Bounds => new RectangleF(_topLeft.X, _topLeft.Y, Width, Height);
    }
}