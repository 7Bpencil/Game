using System.Drawing;
using App.Engine.PhysicsEngine;
using App.Model;

namespace App.View
{
    public class Camera
    {
        public Vector position;
        public Rectangle walkableArea;
        public Rectangle cursorArea;
        
        private void CorrectCameraDependsOnPlayerPosition(Player player)
        {
            var playerCenterInCamera = player.Center.ConvertFromWorldToCamera(position);
            
            var q = playerCenterInCamera.X - player.Radius - walkableArea.X;
            var b = walkableArea.X + walkableArea.Width - (playerCenterInCamera.X + player.Radius);
            var p = playerCenterInCamera.Y - player.Radius - walkableArea.Y;
            var a = walkableArea.Y + walkableArea.Height - (playerCenterInCamera.Y + player.Radius);
            
            if (q < 0) position.X += q;
            if (b < 0) position.X -= b;
            if (p < 0) position.Y += p;
            if (a < 0) position.Y -= a;
        }

        private void CorrectCameraDependsOnCursorPosition(Vector cursorPosition)
        {
            var q = cursorPosition.X - cursorArea.X;
            var b = cursorArea.X + cursorArea.Width - cursorPosition.X;
            var p = cursorPosition.Y - cursorArea.Y;
            var a = cursorArea.Y + cursorArea.Height - cursorPosition.Y;
            
            if (q < 0) position.X += q;
            if (b < 0) position.X -= b;
            if (p < 0) position.Y += p;
            if (a < 0) position.Y -= a;
        }
    }
}