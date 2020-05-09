using System.Drawing;
using App.Engine.Physics.RigidShapes;
using App.Engine.Sprites;
using App.Model.Entities;

namespace App.Model.Factories
{
    public static class SpriteFactory
    {
        private static readonly Bitmap ExitSprite = new Bitmap(@"Assets\Sprites\exit.png");
        
        public static SpriteContainer CreateExitSprite(RigidAABB exit)
        {
            var sprite = new StaticSprite(ExitSprite,0, new Size(128, 128), exit.Width, exit.Height);
            return new SpriteContainer(sprite, exit.Center, 0);
        }
    }
}