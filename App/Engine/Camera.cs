using System;
using System.Drawing;
using App.Engine.PhysicsEngine;

namespace App.Engine
{
    public class Camera
    {
        private Vector cameraPosition;
        public Vector Position => cameraPosition;
        private Vector chaserPosition;

        public Size Size;

        public Camera(Vector playerPosition, Size size)
        {
            cameraPosition = playerPosition - new Vector(size.Width, size.Height) / 2;
            chaserPosition = playerPosition.Copy();
            this.Size = size;
        }

        public void UpdateCamera(Vector playerPosition, float playerRadius, Vector delta, int step, Size levelSizeInTiles, int tileSize)
        {
            CorrectCameraDependsOnPlayerPosition(playerPosition, playerRadius, delta, step);
            //RemoveEscapingFromScene(levelSizeInTiles, tileSize);
        }

        private void RemoveEscapingFromScene(Size levelSizeInTiles, int tileSize)
        {
            var rightBorder = levelSizeInTiles.Width * tileSize - Size.Width;
            const int leftBorder = 0;
            var bottomBorder = levelSizeInTiles.Height * tileSize - Size.Height;
            const int topBorder = 0;
            
            if (cameraPosition.Y < topBorder) cameraPosition.Y = topBorder;
            if (cameraPosition.Y > bottomBorder) cameraPosition.Y = bottomBorder;
            if (cameraPosition.X < leftBorder) cameraPosition.X = leftBorder;
            if (cameraPosition.X > rightBorder) cameraPosition.X = rightBorder;
        }

        private void CorrectCameraDependsOnPlayerPosition(Vector playerPosition, float playerRadius, Vector delta, float step)
        {
            var dist = (chaserPosition - playerPosition).Length; 
            
            if (delta.Equals(Vector.ZeroVector) && Math.Abs(dist) > 6)
                delta = 8 * (playerPosition - chaserPosition).Normalize();
            else if (dist > 3 * playerRadius)
                delta = (playerPosition - chaserPosition).Normalize() * (dist - 3 * playerRadius);
            else 
                delta = delta.Normalize() * (step - 4);

            chaserPosition += delta;
            cameraPosition += delta;
        }
    }
}