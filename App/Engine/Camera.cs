using System;
using System.Drawing;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;

namespace App.Engine
{
    public class Camera
    {
        private Vector position;
        public Vector Position => position;
        private Vector chaserPosition;
        private readonly float playerRadius;
        public readonly Size Size;
        public readonly Vector CameraCenter;

        public Camera(Vector playerPosition, float playerRadius, Size size)
        {
            position = playerPosition - new Vector(size.Width, size.Height) / 2;
            chaserPosition = playerPosition.Copy();
            Size = size;
            CameraCenter = new Vector(size.Width, size.Height) / 2;
            this.playerRadius = playerRadius;
        }

        public void UpdateCamera(Vector playerPosition, Vector playerVelocity, Vector cursorPosition)
        {
            CorrectCameraDependsOnPlayerPosition(playerPosition, playerVelocity);
            CorrectCameraDependsOnCursorPosition(cursorPosition);
        }

        private void CorrectCameraDependsOnPlayerPosition(Vector playerPosition, Vector playerVelocity)
        {
            var dist = (chaserPosition - playerPosition).Length; 
            
            if (playerVelocity.Equals(Vector.ZeroVector) && Math.Abs(dist) > 6)
                playerVelocity = 8 * (playerPosition - chaserPosition).Normalize();
            else if (dist > playerRadius)
                playerVelocity = (playerPosition - chaserPosition).Normalize() * (dist - playerRadius);
            else
                playerVelocity /= 2;

            chaserPosition += playerVelocity;
            position += playerVelocity;
        }

        private void CorrectCameraDependsOnCursorPosition(Vector cursorPosition)
        {
            position = chaserPosition + (cursorPosition - chaserPosition).Normalize() * playerRadius - CameraCenter;
        }

        public RigidCircle GetChaser()
        {
            return new RigidCircle(chaserPosition, 32, false, false);
        }

        public void Reset(Vector playerPosition)
        {
            position = playerPosition - new Vector(Size.Width, Size.Height) / 2;
            chaserPosition = playerPosition.Copy();
        }
    }
}