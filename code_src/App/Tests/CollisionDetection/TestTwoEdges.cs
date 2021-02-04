using App.Engine.Physics;
using App.Engine.Physics.Collision;
using NUnit.Framework;

namespace App.Tests.CollisionDetection
{
    [TestFixture]
    public class TestTwoEdges
    {
        [Test]
        public void TestTwoEdgesCross()
        {
            var first = new Edge(Vector.ZeroVector, new Vector(20, 0));
            var second = new Edge(new Vector(10, 10), new Vector(10, -10));
            Assert.IsTrue(CollisionDetector.AreCollide(first, second));
        }
        
        [Test]
        public void TestTwoEdgesDontCross()
        {
            var first = new Edge(Vector.ZeroVector, new Vector(20, 0));
            var second = new Edge(new Vector(0, 10), new Vector(20, 10));
            Assert.IsFalse(CollisionDetector.AreCollide(first, second));
        }
        
        [Test]
        public void TestTwoEdgesCoincide()
        {
            var first = new Edge(Vector.ZeroVector, new Vector(20, 0));
            var second = new Edge(new Vector(-10, 0), new Vector(30, 0));
            Assert.IsTrue(CollisionDetector.AreCollide(first, second));
        }
        
        [Test]
        public void TestTwoEdgesCreateAngle()
        {
            var first = new Edge(Vector.ZeroVector, new Vector(20, 0));
            var second = new Edge(Vector.ZeroVector, new Vector(-10, 10));
            Assert.IsTrue(CollisionDetector.AreCollide(first, second));
        }
    }
}