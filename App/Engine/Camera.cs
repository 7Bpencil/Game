using System;
using System.Drawing;
using App.Engine.Physics;
using App.Engine.Physics.RigidShape;

namespace App.Engine
{
    public class Camera
    {
        private Vector position;
        public Vector Position => position;
        private Vector chaserPosition;
        private readonly float playerRadius;
        public readonly Size Size;
        public readonly Vector cameraCenter;

        public Camera(Vector playerPosition, float playerRadius, Size size)
        {
            position = playerPosition - new Vector(size.Width, size.Height) / 2;
            chaserPosition = playerPosition.Copy();
            Size = size;
            cameraCenter = new Vector(size.Width, size.Height) / 2;
            this.playerRadius = playerRadius;
        }

        public void UpdateCamera(Vector playerPosition, Vector playerVelocity, Vector cursorPosition, int step)
        {
            CorrectCameraDependsOnPlayerPosition(playerPosition, playerVelocity, step);
            CorrectCameraDependsOnCursorPosition(cursorPosition);
            //RemoveEscapingFromScene(levelSizeInTiles, tileSize);
        }

        private void RemoveEscapingFromScene(Size levelSizeInTiles, int tileSize)
        {
            var rightBorder = levelSizeInTiles.Width * tileSize - Size.Width;
            const int leftBorder = 0;
            var bottomBorder = levelSizeInTiles.Height * tileSize - Size.Height;
            const int topBorder = 0;
            
            if (position.Y < topBorder) position.Y = topBorder;
            if (position.Y > bottomBorder) position.Y = bottomBorder;
            if (position.X < leftBorder) position.X = leftBorder;
            if (position.X > rightBorder) position.X = rightBorder;
        }

        private void CorrectCameraDependsOnPlayerPosition(Vector playerPosition, Vector playerVelocity, float step)
        {
            var dist = (chaserPosition - playerPosition).Length; 
            
            if (playerVelocity.Equals(Vector.ZeroVector) && Math.Abs(dist) > 6)
                playerVelocity = 8 * (playerPosition - chaserPosition).Normalize();
            else if (dist > playerRadius)
                playerVelocity = (playerPosition - chaserPosition).Normalize() * (dist - playerRadius);
            else 
                playerVelocity = playerVelocity.Normalize() * (step - 4);

            chaserPosition += playerVelocity;
            position += playerVelocity;
        }

        private void CorrectCameraDependsOnCursorPosition(Vector cursorPosition)
        {
            position = chaserPosition + (cursorPosition - chaserPosition).Normalize() * playerRadius - cameraCenter;
        }

        public RigidCircle GetChaser()
        {
            return new RigidCircle(chaserPosition, 32, false, false);
        }
    }
}