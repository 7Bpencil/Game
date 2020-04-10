using System;
using System.Drawing;
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
        
        private RectangleF GetBoundsInCamera(Vector cameraPosition)
        {
            var topLeftInCamera = TopLeft.ConvertFromWorldToCamera(cameraPosition);
            return new RectangleF(topLeftInCamera.X, topLeftInCamera.Y, size.Width, size.Height);
        }
        
        private Rectangle GetCurrentFrameTile()
        {
            return new Rectangle
            {
                X = currentFrame % columns * size.Width,
                Y = currentFrame / columns * size.Height,
                Width = size.Width,
                Height = size.Height
            };
        }

        private int expectedCenterVersion;
        private int centerVersion;
        
        private Vector previousPosition;
        private int startFrame;
        private int currentFrame;
        private int endFrame;
        private Vector velocity;
        public Vector Velocity
        { get => velocity; set => velocity = value; }
        
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

        public void DrawNextFrame(Graphics graphics, Vector cameraPosition)
        {
            if (currentFrame < startFrame || currentFrame > endFrame) currentFrame = startFrame;
            graphics.DrawImage(bitmap, GetBoundsInCamera(cameraPosition), GetCurrentFrameTile(), GraphicsUnit.Pixel);
            var time = Environment.TickCount;
            if (time > lastTime + animationRate && !previousPosition.Equals(center)) // That check is need to resolve framerate and animation rendering
            {
                lastTime = time;
                previousPosition = TopLeft;
                currentFrame++;
            }
        }
    }
}