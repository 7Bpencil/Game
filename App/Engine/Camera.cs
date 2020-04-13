using System.Drawing;
using App.Engine.PhysicsEngine;
using App.Model;

namespace App.Engine
{
    public class Camera
    {
        public Vector position;
        private Rectangle walkableArea;
        private Rectangle cursorArea;

        public Size size;

        public Camera(Vector position, Size size,  Rectangle walkableArea, Rectangle cursorArea)
        {
            this.position = position;
            this.size = size;
            this.walkableArea = walkableArea;
            this.cursorArea = cursorArea;
        }

        public void UpdateCamera(Vector cursorPosition, Player player, Size levelSizeInTiles, int tileSize)
        { 
            CorrectCameraDependsOnCursorPosition(cursorPosition);
            CorrectCameraDependsOnPlayerPosition(player);
            RemoveEscapingFromScene(levelSizeInTiles, tileSize);
        }

        private void RemoveEscapingFromScene(Size levelSizeInTiles, int tileSize)
        {
            var rightBorder = levelSizeInTiles.Width * tileSize - size.Width;
            const int leftBorder = 0;
            var bottomBorder = levelSizeInTiles.Height * tileSize - size.Height;
            const int topBorder = 0;
            
            if (position.Y < topBorder) position.Y = topBorder;
            if (position.Y > bottomBorder) position.Y = bottomBorder;
            if (position.X < leftBorder) position.X = leftBorder;
            if (position.X > rightBorder) position.X = rightBorder;
        }

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