using System.Drawing;
using App.Engine.Physics;

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
        private readonly int startFrame;
        private int currentFrame;
        private readonly int endFrame;

        private readonly Rectangle destRectInCamera;

        private readonly Size size;
        private readonly Bitmap bitmap;
        private readonly int columns;
        
        private readonly int framePeriod;
        private int ticksFromLastFrame;
        
        private double angle;
        public double Angle { get => angle; set => angle = value; }

        public Sprite(Vector center, Bitmap bitmap, int framePeriod, int startFrame, int endFrame, Size size, int columns)
        {
            this.center = center;
            topLeft = new Vector(center.X - size.Width / 2, center.Y - size.Height / 2);
            previousPosition = topLeft;
            angle = 0;
            
            this.size = size;
            this.bitmap = bitmap;
            this.columns = columns;
            destRectInCamera = new Rectangle(-size.Width / 2, -size.Height / 2, size.Width, size.Height);
            
            this.startFrame = startFrame;
            currentFrame = startFrame;
            this.endFrame = endFrame;
            
            this.framePeriod = framePeriod;
            ticksFromLastFrame = 0;
        }


        /// <summary>
        /// Case when coordinates are in camera axis
        /// </summary>
        /// <param name="graphics"></param>
        public void DrawNextFrame(Graphics graphics)
        {
            if (currentFrame > endFrame) currentFrame = startFrame;
            graphics.DrawImage(bitmap, GetBounds(), GetCurrentFrameTile(), GraphicsUnit.Pixel);
            UpdateFrame();
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
            if (currentFrame > endFrame) currentFrame = startFrame;
            graphics.DrawImage(bitmap, destRectInCamera, GetCurrentFrameTile(), GraphicsUnit.Pixel);
           
            graphics.Restore(stateBefore);
            
            UpdateFrame();
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
        
        public void IncrementTick() => ticksFromLastFrame++;

        private void UpdateFrame()
        {
            if (ticksFromLastFrame > framePeriod)
            {
                ticksFromLastFrame = 0;
                currentFrame++;
            }
        }
    }
}