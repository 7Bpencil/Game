using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidShapes;
using NUnit.Framework;

namespace App.Tests.CollisionDetection
{
    [TestFixture]
    public class TestTwoCircles
    {
        private static readonly RigidCircle MainCircle = new RigidCircle(Vector.ZeroVector, 30, true, true);

        [Test]
        public void TestTwoCirclesAreInTouch()
        {
            var circle = new RigidCircle(new Vector(40, 0), 15, true, true);
            var expected = new CollisionInfo(5, new Vector(1, 0), new Vector(25, 0));
            Assert.AreEqual(expected, CollisionDetector.GetCollisionInfo(circle, MainCircle));
        }

        [Test]
        public void TestTwoCirclesAreInTouchCircleCenterIsInMainCircle()
        {
            var circle = new RigidCircle(new Vector(20, 0), 15, true, true);
            var expected = new CollisionInfo(25, new Vector(1, 0), new Vector(5, 0));
            Assert.AreEqual(expected, CollisionDetector.GetCollisionInfo(circle, MainCircle));
        }

        [Test]
        public void TestTwoCirclesAreInTouchOnEdge()
        {
            var first = new RigidCircle(new Vector(40, 0), 10, true, true);
            var expected = new CollisionInfo(0, new Vector(1, 0), new Vector(30, 0));
            Assert.AreEqual(expected, CollisionDetector.GetCollisionInfo(first, MainCircle));
        }

        [Test]
        public void TestOneCircleIsInAnother()
        {
            var first = new RigidCircle(new Vector(15, 0), 10, true, true);
            var expected = new CollisionInfo(25, new Vector(1, 0), new Vector(5, 0));
            Assert.AreEqual(expected, CollisionDetector.GetCollisionInfo(first, MainCircle));
        }

        [Test]
        public void TestTwoCirclesHaveTheSameCenters()
        {
            var first = new RigidCircle(Vector.ZeroVector, 10, true, true);
            var expected = new CollisionInfo(MainCircle.Radius, new Vector(0, -1), new Vector(0, 30));
            Assert.AreEqual(expected, CollisionDetector.GetCollisionInfo(first, MainCircle));
        }

        [Test]
        public void TestTwoCirclesAreNotInTouch()
        {
            var first = new RigidCircle(new Vector(50, 0), 10, true, true);
            Assert.IsNull(CollisionDetector.GetCollisionInfo(first, MainCircle));
        }
    }
}
