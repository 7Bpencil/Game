using System.Drawing;
using App.Engine.Physics;

namespace App.Engine.Render.Renderers
{
    public static class SpriteRenderer
    {
        public static void DrawNextFrame(
            Sprite sprite, Vector centerPosition, float angle, Vector cameraPosition, Graphics graphics)
        {
            var stateBefore = graphics.Save();

            var centerInCamera = centerPosition.ConvertFromWorldToCamera(cameraPosition);
            graphics.TranslateTransform(centerInCamera.X, centerInCamera.Y);
            graphics.RotateTransform(-angle);
            graphics.DrawImage(sprite.Bitmap, sprite.DestRectInCamera, sprite.GetCurrentFrame(), GraphicsUnit.Pixel);

            graphics.Restore(stateBefore);
        }

        public static void DrawNextFrame(
            Sprite sprite, Vector centerPosition, float angle, Graphics graphics)
        {
            var stateBefore = graphics.Save();

            graphics.TranslateTransform(centerPosition.X, centerPosition.Y);
            graphics.RotateTransform(-angle);
            graphics.DrawImage(sprite.Bitmap, sprite.DestRectInCamera, sprite.GetCurrentFrame(), GraphicsUnit.Pixel);

            graphics.Restore(stateBefore);
        }

        public static void DrawNextFrame(
            AbstractParticle particle, Rectangle currentFrame, Vector centerPosition, float angle, Vector cameraPosition, Graphics graphics)
        {
            var stateBefore = graphics.Save();

            var centerInCamera = centerPosition.ConvertFromWorldToCamera(cameraPosition);
            graphics.TranslateTransform(centerInCamera.X, centerInCamera.Y);
            graphics.RotateTransform(-angle);
            graphics.DrawImage(particle.Bitmap, particle.DestRectInCamera, currentFrame, GraphicsUnit.Pixel);

            graphics.Restore(stateBefore);
        }

        public static void DrawNextFrame(
            AbstractParticle particle, Rectangle currentFrame, Vector centerPosition, float angle, Graphics graphics)
        {
            var stateBefore = graphics.Save();

            graphics.TranslateTransform(centerPosition.X, centerPosition.Y);
            graphics.RotateTransform(-angle);
            graphics.DrawImage(particle.Bitmap, particle.DestRectInCamera, currentFrame, GraphicsUnit.Pixel);

            graphics.Restore(stateBefore);
        }
    }
}
