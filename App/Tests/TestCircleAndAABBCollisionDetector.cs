using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidShapes;
using NUnit.Framework;

namespace App.Tests
{
    [TestFixture]
    public class TestCircleAndAABBCollisionDetector
    {
        [Test]
        public void TestCircleAndAABBAreInTouch()
        {
            var circle = new RigidCircle(new Vector(0, -5), 20, true, true);
            var rectangle = new RigidAABB(Vector.ZeroVector, new Vector(40, 40), true, true);
            var expected = new CollisionInfo(15, new Vector(0, -1), Vector.ZeroVector);
            Assert.AreEqual(expected, CollisionDetector.GetCollisionInfo(circle, rectangle));
        }
        
        [Test]
        public void TestCircleAndAABBAreInTouchOnEdge()
        {
            var circle = new RigidCircle(new Vector(5, -10), 10, true, true);
            var rectangle = new RigidAABB(Vector.ZeroVector, new Vector(40, 40), true, true);
            var expected = new CollisionInfo(0, new Vector(0, -1), new Vector(5, 0));
            Assert.AreEqual(expected, CollisionDetector.GetCollisionInfo(circle, rectangle));
        }
        
        [Test]
        public void TestCircleAndAABBAreNotInTouch()
        {
            var circle = new RigidCircle(new Vector(10, -10), 5, true, true);
            var rectangle = new RigidAABB(Vector.ZeroVector, new Vector(40, 40), true, true);
            Assert.IsNull(CollisionDetector.GetCollisionInfo(circle, rectangle));
        }
    }
}