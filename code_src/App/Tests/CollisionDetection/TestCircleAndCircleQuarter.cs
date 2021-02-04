using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidShapes;
using NUnit.Framework;

namespace App.Tests.CollisionDetection
{
    [TestFixture]
    public class TestCircleAndCircleQuarter
    {
        private static readonly RigidCircle MainWholeCircle = new RigidCircle(Vector.ZeroVector, 30, true, true);
        private static readonly Vector MainDirection = new Vector(0, -1);
        private static readonly Vector SecondDirection = new Vector(1, 0);

        private void TestInTouch(RigidCircle circle, RigidCircleQuarter quarterMainDirection, RigidCircleQuarter quarterSecondDirection, CollisionInfo expected)
        {
            Assert.AreEqual(expected, CollisionDetector.GetCollisionInfo(circle, quarterMainDirection));
            Assert.AreEqual(expected, CollisionDetector.GetCollisionInfo(circle, quarterSecondDirection));
        }

        private void TestNotInTouch(RigidCircle circle, RigidCircleQuarter quarterMainDirection, RigidCircleQuarter quarterSecondDirection)
        {
            Assert.IsNull(CollisionDetector.GetCollisionInfo(circle, quarterMainDirection));
            Assert.IsNull(CollisionDetector.GetCollisionInfo(circle, quarterSecondDirection));
        }

        
        
        private static readonly RigidCircleQuarter CircleFirstQuarterMain = new RigidCircleQuarter(MainDirection, 1, MainWholeCircle);
        private static readonly RigidCircleQuarter CircleFirstQuarterSecond = new RigidCircleQuarter(SecondDirection, 1, MainWholeCircle);
        [Test]
        public void TestCircleAndFirstQuarterAreInTouch()
        {
            TestInTouch(
                new RigidCircle(new Vector(40, 0), 15, true, true), 
                CircleFirstQuarterMain, 
                CircleFirstQuarterSecond, 
                new CollisionInfo(5, new Vector(1, 0), new Vector(25, 0)));
        }
        
        [Test]
        public void TestCircleAndFirstQuarterAreNotInTouch()
        {
            TestNotInTouch(
                new RigidCircle(new Vector(-20, 0), 15, true, true), 
                CircleFirstQuarterMain, 
                CircleFirstQuarterSecond);
        }
        
        
        
        private static readonly RigidCircleQuarter CircleSecondQuarterMain = new RigidCircleQuarter(MainDirection, 2, MainWholeCircle);
        private static readonly RigidCircleQuarter CircleSecondQuarterSecond = new RigidCircleQuarter(SecondDirection, 2, MainWholeCircle);
        [Test]
        public void TestCircleAndSecondQuarterAreInTouch()
        {
            TestInTouch(
                new RigidCircle(new Vector(0, -35), 15, true, true), 
                CircleSecondQuarterMain, 
                CircleSecondQuarterSecond, 
                new CollisionInfo(10, new Vector(0, -1), new Vector(0, -20)));
        }
        
        [Test]
        public void TestCircleAndSecondQuarterAreNotInTouch()
        {
            TestNotInTouch(
                new RigidCircle(new Vector(0, 20), 15, true, true), 
                CircleSecondQuarterMain, 
                CircleSecondQuarterSecond);
        }
        
        
        
        private static readonly RigidCircleQuarter CircleThirdQuarterMain = new RigidCircleQuarter(MainDirection, 3, MainWholeCircle);
        private static readonly RigidCircleQuarter CircleThirdQuarterSecond = new RigidCircleQuarter(SecondDirection, 3, MainWholeCircle);
        [Test]
        public void TestCircleAndThirdQuarterAreInTouch()
        {
            TestInTouch(
                new RigidCircle(new Vector(-30, 0), 20, true, true), 
                CircleThirdQuarterMain, 
                CircleThirdQuarterSecond, 
                new CollisionInfo(20, new Vector(-1, 0), new Vector(-10, 0)));
        }
        
        [Test]
        public void TestCircleAndThirdQuarterAreNotInTouch()
        {
            TestNotInTouch(
                new RigidCircle(new Vector(0, 50), 19, true, true), 
                CircleThirdQuarterMain, 
                CircleThirdQuarterSecond);
        }

        

        private static readonly RigidCircleQuarter CircleFourthQuarterMain = new RigidCircleQuarter(MainDirection, 4, MainWholeCircle);
        private static readonly RigidCircleQuarter CircleFourthQuarterSecond = new RigidCircleQuarter(SecondDirection, 4, MainWholeCircle);
        [Test]
        public void TestCircleAndFourthQuarterAreInTouch()
        {
            TestInTouch(
                new RigidCircle(new Vector(0, 200), 180, true, true), 
                CircleFourthQuarterMain, 
                CircleFourthQuarterSecond, 
                new CollisionInfo(10, new Vector(0, 1), new Vector(0, 20)));
        }
        
        [Test]
        public void TestCircleAndFourthQuarterAreNotInTouch()
        {
            TestNotInTouch(
                new RigidCircle(new Vector(0, -20), 19, true, true), 
                CircleFourthQuarterMain, 
                CircleFourthQuarterSecond);
        }
    }
}