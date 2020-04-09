using System;
using System.Drawing;
using App.Engine.PhysicsEngine;

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
        private Vector position;
        private Vector velocity;
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
            position = Vector.ZeroVector;
            velocity = Vector.ZeroVector;
            size = new Size(0, 0);
            bitmap = null;
            alive = true;
            columns = 1;
            totalFrames = 1; //?
            currentFrame = 0;
            animationDir = AnimateDir.FORWARD;
            animationWrap = AnimateWrap.WRAP;
            lastTime = 0;
            animationRate = 30;
        }

        public Sprite(Game game, Vector position, bool alive, int totalFrames, int animationRate) // new constructor
        {
            this.game = game;
            this.position = position;
            this.alive = alive;
            this.totalFrames = totalFrames;
            this.animationRate = animationRate;
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

        public AnimateDir AnimateDirection
        { get => animationDir; set => animationDir = value; }

        public AnimateWrap AnimateWrapMode
        { get => animationWrap; set => animationWrap = value; }

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
        }

        public void Draw()
        {
            var frame = new Rectangle
            {
                X = currentFrame % columns * size.Width,
                Y = currentFrame / columns * size.Height,
                Width = size.Width,
                Height = size.Height
            };
            game.Device.DrawImage(bitmap, Bounds, frame, GraphicsUnit.Pixel);
        }

        public RectangleF Bounds => new RectangleF(X, Y, Width, Height);
        
        public bool IsColliding(ref Sprite other)
        {
            return Bounds.IntersectsWith(other.Bounds);
        }
    }
}