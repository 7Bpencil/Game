using System;
using System.Drawing;
using System.Runtime.InteropServices;
using App.Engine.PhysicsEngine;

namespace App.View
{
    public class Sprite
    {
        private Vector center;
        public Vector Center
        {
            get => center;
            set
            {
                center = value;
                centerVersion++;
            }
        }
        
        private Vector topLeft;
        public Vector TopLeft
        {
            get
            {
                if (centerVersion != expectedCenterVersion)
                    topLeft = new Vector(center.X - size.Width / 2, center.Y - size.Height / 2);
                expectedCenterVersion = centerVersion = 0;
                return topLeft;
            }
        }

        private int expectedCenterVersion;
        private int centerVersion = 0;
        
        private Vector previousPosition;
        private int startFrame;
        private int currentFrame;
        private int endFrame;
        private Vector velocity;
        private Size size;
        private Bitmap bitmap;
        private int columns;
        private int lastTime;
        private int animationRate;

        public Sprite(Vector center, Bitmap bitmap, int startFrame, int endFrame, Size size, int columns)
        {
            this.center = center;
            topLeft = new Vector(center.X - size.Width / 2, center.Y - size.Height / 2);
            velocity = Vector.ZeroVector;
            previousPosition = center;
            this.size = size;
            this.bitmap = bitmap;
            this.columns = columns;
            this.startFrame = startFrame;
            this.endFrame = endFrame;
            currentFrame = 0;
            lastTime = 0;
            animationRate = 1500 / 3;
        }
        
        public Vector Velocity
        { get => velocity; set => velocity = value; }

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
            if (currentFrame < startFrame || currentFrame > endFrame) currentFrame = startFrame;
            Draw(device);
            //check animation timing
            var time = Environment.TickCount;
            if (time > lastTime + animationRate && !previousPosition.Equals(center)) //it need to resolve framerate and animation radnering
            {
                lastTime = time;
                previousPosition = TopLeft;
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

        public RectangleF Bounds => new RectangleF(TopLeft.X, TopLeft.Y, size.Width, size.Height);
    }
}