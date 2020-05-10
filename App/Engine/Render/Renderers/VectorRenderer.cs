using System.Drawing;
using App.Engine.Physics;

namespace App.Engine.Render.Renderers
{
    public static class VectorRenderer
    {
        public static void Fill(Vector vector, Brush brush, Graphics g)
        {
            g.FillEllipse(brush, vector.X - 8, vector.Y - 8, 16, 16);
        }
        
        public static void Fill(Vector vector, Vector cameraPosition, Brush brush, Graphics g)
        {
            var vectorPositionInCamera = vector.ConvertFromWorldToCamera(cameraPosition);
            g.FillEllipse(brush, vectorPositionInCamera.X - 8, vectorPositionInCamera.Y - 8, 16, 16);
        }
    }
}