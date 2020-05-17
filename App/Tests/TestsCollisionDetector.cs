using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidShapes;
using NUnit.Framework;

namespace App.Tests
{
    [TestFixture]
    public class TestsCollisionDetector
    {
        [Test]
        public void TestSimpleCirclesCollision()
        {
            var first = new RigidCircle(Vector.ZeroVector, 10, true, true);
            var second = new RigidCircle(new Vector(0, 15), 10, true, true);
            Assert.IsNotNull(CollisionDetector.GetCollisionInfo(first, second));
        }
    }
}