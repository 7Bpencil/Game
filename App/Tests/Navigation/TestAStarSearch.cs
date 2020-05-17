using System.Collections.Generic;
using System.Drawing;
using App.Engine;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using NUnit.Framework;

namespace App.Tests.Navigation
{
    [TestFixture]
    public class TestAStarSearch
    {
        private static readonly Size LevelSizeInTiles = new Size(10, 10);
        private static readonly NavMesh NoWalls = new NavMesh(LevelSizeInTiles, new List<RigidShape>());
        private static readonly NavMesh OneWall = new NavMesh(LevelSizeInTiles, new List<RigidShape>
        {
            new RigidAABB(new Vector(32, 32), new Vector(64, 64), true, true)
        });
        
        
        [Test]
        public void TestGetCorrectPoint()
        {
            AStarSearch.SetMesh(OneWall);
            var correctPoint = AStarSearch.TryGetCorrectPoint(new Point(1, 1));
            Assert.AreEqual(correctPoint, new Point(0, 0));
        }

        [Test]
        public void TestSearchPathIncorrectSart()
        {
            AStarSearch.SetMesh(OneWall);
            Assert.AreEqual(AStarSearch.SearchPath(new Vector(1, 2), new Vector(5, 5)), new List<Vector>(){new Vector(0, 0)});
        }
        
        [Test]
        public void TestSearchPathIncorrectGoal()
        {
            AStarSearch.SetMesh(OneWall);
            Assert.AreEqual(AStarSearch.SearchPath(new Vector(5, 5), new Vector(1, 1)), new List<Vector>(){new Vector(0, 0)});
        }
        
        [Test]
        public void TestSearchPathCorrect()
        {
            AStarSearch.SetMesh(NoWalls);
            Assert.Positive(AStarSearch.SearchPath(new Vector(1, 1), new Vector(5, 5)).Count);
        }
    }
}