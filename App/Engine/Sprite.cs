using System.Drawing;
using App.Engine.Physics;

namespace App.Engine
{
    public class Sprite
    {
        protected Vector center;
        public Vector Center => center;

        protected Vector topLeft;
        public Vector TopLeft => topLeft;

        protected int startFrame;
        protected int currentFrame;
        protected int endFrame;

        private Rectangle destRectInCamera;

        private Size size;
        private Bitmap bitmap;
        private int columns;
        
        protected int framePeriod;
        protected int ticksFromLastFrame;
        
        private double angle;
        public double Angle { get => angle; set => angle = value; }

        public bool ShouldBeDeleted;

        private RectangleF GetBounds()
        {
            return new RectangleF(TopLeft.X, TopLeft.Y, size.Width, size.Height);
        }
        
        protected virtual Rectangle GetCurrentFrameTile()
        {
            return new Rectangle
            {
                X = currentFrame % columns * size.Width,
                Y = currentFrame / columns * size.Height,
                Width = size.Width,
                Height = size.Height
            };
        }

        public Sprite(Vector center, Bitmap bitmap, float angle, int framePeriod, int startFrame, int endFrame, Size size, int columns)
        {
            this.center = center;
            topLeft = new Vector(center.X - size.Width / 2, center.Y - size.Height / 2);
            this.angle = angle;
            
            this.size = size;
            this.bitmap = bitmap;
            this.columns = columns;
            destRectInCamera = new Rectangle(-size.Width / 2, -size.Height / 2, size.Width, size.Height);
            
            this.startFrame = startFrame;
            currentFrame = startFrame;
            this.endFrame = endFrame;
            
            this.framePeriod = framePeriod;
            ticksFromLastFrame = 0;
            ShouldBeDeleted = false;
        }
        
        /// <summary>
        /// Case when coordinates are in camera axis
        /// </summary>
        /// <param name="graphics"></param>
        public void DrawNextFrame(Graphics graphics)
        {
            graphics.DrawImage(bitmap, GetBounds(), GetCurrentFrameTile(), GraphicsUnit.Pixel);
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
            graphics.TranslateTransform(centerInCamera.X, centerInCamera.Y);
            graphics.RotateTransform((float)-angle);
            graphics.DrawImage(bitmap, destRectInCamera, GetCurrentFrameTile(), GraphicsUnit.Pixel);
           
            graphics.Restore(stateBefore);
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

        /// <summary>
        /// Default method that loops through frames
        /// </summary>
        public virtual void UpdateFrame()
        {
            ticksFromLastFrame++;
            if (ticksFromLastFrame > framePeriod)
            {
                ticksFromLastFrame = 0;
                currentFrame++;
                if (currentFrame > endFrame) currentFrame = startFrame;
            }
        }
    }
}