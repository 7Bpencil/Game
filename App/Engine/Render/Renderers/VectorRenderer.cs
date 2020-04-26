using System.Drawing;
using App.Engine.Physics;

namespace App.Engine.Render.Renderers
{
    public static class VectorRenderer
    {
        public static void Fill(Vector vector, Brush brush, Graphics g)
        {
            g.FillEllipse(brush, vector.X - 2, vector.Y - 2, 4, 4);
        }
        
        public static void Fill(Vector vector, Vector cameraPosition, Brush brush, Graphics g)
        {
            var vectorPositionInCamera = vector.ConvertFromWorldToCamera(cameraPosition);
            g.FillEllipse(brush, vectorPositionInCamera.X - 2, vectorPositionInCamera.Y - 2, 4, 4);
        }
    }
}