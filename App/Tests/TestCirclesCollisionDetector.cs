using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidShapes;
using NUnit.Framework;

namespace App.Tests
{
    [TestFixture]
    public class TestCirclesCollisionDetector
    {
        [Test]
        public void TestCirclesAreInTouch()
        {
            var first = new RigidCircle(Vector.ZeroVector, 10, true, true);
            var second = new RigidCircle(new Vector(0, 20), 15, true, true);
            var result = CollisionDetector.GetCollisionInfo(first, second);
            var expected = new CollisionInfo(5, new Vector(0, -1), new Vector(0, 10));
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void TestCirclesAreInTouchOnEdge()
        {
            var first = new RigidCircle(Vector.ZeroVector, 10, true, true);
            var second = new RigidCircle(new Vector(0, 20), 10, true, true);
            var expected = new CollisionInfo(0, new Vector(0, -1), new Vector(0, 10));
            Assert.AreEqual(expected, CollisionDetector.GetCollisionInfo(first, second));
        }
        
        [Test]
        public void TestOneCircleIsInAnother()
        {
            var first = new RigidCircle(Vector.ZeroVector, 30, true, true);
            var second = new RigidCircle(new Vector(0, 15), 10, true, true);
            var expected = new CollisionInfo(25, new Vector(0, -1), new Vector(0, 30));
            Assert.AreEqual(expected, CollisionDetector.GetCollisionInfo(first, second));
        }
        
        [Test]
        public void TestCirclesHaveTheSameCenters()
        {
            var first = new RigidCircle(Vector.ZeroVector, 10, true, true);
            var second = new RigidCircle(Vector.ZeroVector, 15, true, true);
            var expected = new CollisionInfo(15, new Vector(0, -1), new Vector(0, 15));
            Assert.AreEqual(expected, CollisionDetector.GetCollisionInfo(first, second));
        }

        [Test]
        public void TestCirclesAreNotInTouch()
        {
            var first = new RigidCircle(Vector.ZeroVector, 10, true, true);
            var second = new RigidCircle(new Vector(0, 20), 9, true, true);
            Assert.IsNull(CollisionDetector.GetCollisionInfo(first, second));
        }
    }
}