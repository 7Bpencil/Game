using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidShapes;
using NUnit.Framework;

namespace App.Tests.CollisionDetection
{
    
    [TestFixture]
    public class TestDynamicCollisionDetector
    {
        private static readonly Vector StartPosition = new Vector(0, 0);
        private static readonly RigidCircle StaticBodyCircle = new RigidCircle(new Vector(0, 2), 1, true, true);
        private static readonly RigidAABB StaticBodyAABB = new RigidAABB(new Vector(-1, 1), new Vector(1, 3), true, true);
        
        [Test]
        public void TestAreCollideWithStaticCantHit()
        {
            var objectVelocity = new Vector(1, 0);
            Assert.IsNull(DynamicCollisionDetector.AreCollideWithStatic(StartPosition, objectVelocity, StaticBodyCircle));
            Assert.IsNull(DynamicCollisionDetector.AreCollideWithStatic(StartPosition, objectVelocity, StaticBodyAABB));
        }
        
        [Test]
        public void TestAreCollideWithStaticCanHit()
        {
            var objectVelocity = new Vector(0, 1);
            Assert.IsNotNull(DynamicCollisionDetector.AreCollideWithStatic(StartPosition, objectVelocity, StaticBodyCircle));
            Assert.IsNotNull(DynamicCollisionDetector.AreCollideWithStatic(StartPosition, objectVelocity, StaticBodyAABB));
        }
        
        [Test]
        public void TestAreCollideWithStaticCheckTime()
        {
            var objectVelocity = new Vector(0, 1);
            var resultTime = new float[] {1, 3};
            Assert.AreEqual(resultTime, DynamicCollisionDetector.AreCollideWithStatic(StartPosition, objectVelocity, StaticBodyCircle));
            Assert.AreEqual(resultTime, DynamicCollisionDetector.AreCollideWithStatic(StartPosition, objectVelocity, StaticBodyAABB));
        }
    }
}