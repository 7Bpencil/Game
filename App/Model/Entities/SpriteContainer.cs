using App.Engine;

namespace App.Model.Entities
{
    public class SpriteContainer
    {
        public Sprite Content;
        public SpriteContainer(Sprite startContent) => Content = startContent;
        public void ClearContent() => Content = null;
    }
}