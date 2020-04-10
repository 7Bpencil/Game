using System;
using System.Drawing;
using System.Runtime.InteropServices;
using App.Engine.PhysicsEngine;

namespace App.View
{
    public class Sprite
    {
        private Vector _topLeft;
        private Vector velocity;
        private Size size;
        private Bitmap bitmap;
        private bool alive;
        private int columns;
        private int currentFrame;
        private int lastTime;

        public Sprite()
        {
            _topLeft = Vector.ZeroVector;
            velocity = Vector.ZeroVector;
            size = new Size(64, 64);
            bitmap = null;
            alive = true;
            columns = 1;
            currentFrame = 0;
            lastTime = 0;
        }
        
        public bool Alive
        { get => alive; set => alive = value; }

        public Bitmap Image
        { get => bitmap; set => bitmap = value; }
        

        public Vector TopLeft
        { get => _topLeft; set => _topLeft = value; }

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

        public int CurrentFrame
        { get  => currentFrame; set => currentFrame = value; }

        /*public void Animate(int startFrame, int endFrame)
        {
            //check animation timing
            var time = Environment.TickCount;
            if (time > lastTime + animationRate) //it need to resolve framerate and animation radnering
            {
                lastTime = time;
                currentFrame = 
            }
        }
        */

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

        public RectangleF Bounds => new RectangleF(_topLeft.X, _topLeft.Y, Width, Height);
        
        public bool IsColliding(ref Sprite other)
        {
            return Bounds.IntersectsWith(other.Bounds);
        }
    }
}