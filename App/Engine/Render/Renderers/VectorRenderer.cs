using System.Drawing;
using App.Engine.Physics;

namespace App.Engine.Render.Renderers
{
    public static class VectorRenderer
    {
        public static void Fill(Vector vector, Brush brush, Graphics g)
        {
            g.FillEllipse(brush, vector.X - 3, vector.Y - 3, 6, 6);
        }
        
        public static void Fill(Vector vector, Vector cameraPosition, Brush brush, Graphics g)
        {
            var vectorPositionInCamera = Vector.ConvertFromWorldToCamera(vector, cameraPosition);
            g.FillEllipse(brush, vectorPositionInCamera.X - 3, vectorPositionInCamera.Y - 3, 6, 6);
        }
    }
}