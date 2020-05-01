using App.Engine;
using App.Engine.Physics;

namespace App.Model.Entities
{
    public class SpriteContainer
    {
        private Sprite content;
        public Vector CenterPosition;
        public float Angle;

        public SpriteContainer(Sprite startContent, Vector startCenterPosition, float startAngle)
        {
            content = startContent;
            CenterPosition = startCenterPosition;
            Angle = startAngle;
        }
        
        public void ClearContent() => content = null;
        public bool IsEmpty() => content == null;
        public void ChangeContent(Sprite newContent) => content = newContent;
        public Sprite GetContent() => content;
    }
}