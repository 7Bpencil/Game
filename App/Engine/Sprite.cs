using System;
using System.Drawing;
using App.Engine.PhysicsEngine;

namespace App.Engine
{
    public class Sprite
    {
        private Vector center;
        public Vector Center => center;

        private Vector topLeft;
        public Vector TopLeft => topLeft;

        private RectangleF GetBounds()
        {
            return new RectangleF(TopLeft.X, TopLeft.Y, size.Width, size.Height);
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

        private Vector previousPosition;
        private int startFrame;
        private int currentFrame;
        private int endFrame;
        
        private Rectangle destRectInCamera;
        private Vector velocity;
        public Vector Velocity { get => velocity; set => velocity = value; }
        
        private Size size;
        private Bitmap bitmap;
        private int columns;
        private int lastTime;
        private int animationRate;
        
        private double angle;
        public double Angle { get => angle; set => angle = value; }

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
            angle = 0;
            destRectInCamera = new Rectangle(-size.Width / 2, -size.Height / 2, size.Width, size.Height);
        }

        //public int CurrentFrame { get  => currentFrame; set => currentFrame = value; }
        
        public int AnimationRate
        {
            get => 1000 / animationRate;
            set
            {
                if (value == 0) value = 1;
                animationRate = 1000 / value;
            }
        }
        
        /// <summary>
        /// Case when coordinates are in camera axis
        /// </summary>
        /// <param name="graphics"></param>
        public void DrawNextFrame(Graphics graphics)
        {
            if (currentFrame < startFrame || currentFrame > endFrame) currentFrame = startFrame;
            graphics.DrawImage(bitmap, GetBounds(), GetCurrentFrameTile(), GraphicsUnit.Pixel);
            var time = Environment.TickCount;
            if (time > lastTime + animationRate && !previousPosition.Equals(center)) // That check is need to resolve framerate and animation rendering
            {
                lastTime = time;
                previousPosition = TopLeft;
                currentFrame++;
            }
        }

        /// <summary>
        /// Case when coordinates are in world axis
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="cameraPosition"></param>
        public void DrawNextFrame(Graphics graphics, Vector cameraPosition)
        {
            var stateBefore = graphics.Save();
            
            var centerInCamera = Center.ConvertFromWorldToCamera(cameraPosition);
            if (!centerInCamera.Equals(Vector.ZeroVector))
                graphics.TranslateTransform(centerInCamera.X, centerInCamera.Y);
            graphics.RotateTransform((float)-angle);
            if (currentFrame < startFrame || currentFrame > endFrame) currentFrame = startFrame;
            graphics.DrawImage(bitmap, destRectInCamera, GetCurrentFrameTile(), GraphicsUnit.Pixel);
           
            graphics.Restore(stateBefore);
            
            var time = Environment.TickCount;
            if (time > lastTime + animationRate && !previousPosition.Equals(center)) // That check is need to resolve framerate and animation rendering
            {
                lastTime = time;
                previousPosition = TopLeft;
                currentFrame++;
            }
        }

        public void MoveBy(Vector delta)
        {
            center += delta;
            topLeft = new Vector(center.X - size.Width / 2, center.Y - size.Height / 2);            
        }
        
        public void MoveTo(Vector newCenterPosition)
        {
            center = newCenterPosition;
            topLeft = new Vector(center.X - size.Width / 2, center.Y - size.Height / 2);            
        }
    }
}