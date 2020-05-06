using System.Collections.Generic;
using App.Engine.Particles;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidShapes;
using App.Model.Entities;

namespace App.Model.LevelData
{
    public class LevelRuntimeState
    {
        
        public readonly ShapesIterator SceneShapes;
        public readonly List<RigidShape> DynamicShapes;
        public readonly List<Collectable> Collectables;
        
        public readonly List<SpriteContainer> Sprites;
        public readonly List<AbstractParticleUnit> Particles;
        public readonly List<CollisionInfo> CollisionInfo;
        public readonly List<Bullet> Bullets;
        public readonly List<Bot> Targets;

        private bool isLevelLoaded;

        public LevelRuntimeState(Level levelInfo)
        {
            
            Bullets = new List<Bullet> {Capacity = 1000};
            Particles = new List<AbstractParticleUnit> {Capacity = 1000};
        }
    }
}