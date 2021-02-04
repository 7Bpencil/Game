using System.Collections.Generic;
using System.Drawing;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using NUnit.Framework;

namespace App.Tests.Navigation
{
    [TestFixture]
    public class TestNavMesh
    {
        private static readonly Size LevelSizeInTiles = new Size(10, 10);
        private static readonly List<RigidShape> NoWallsShapes = new List<RigidShape>();
        private static readonly List<RigidShape> OneWallShapes = new List<RigidShape>
        {
            new RigidAABB(new Vector(32, 32), new Vector(64, 64), true, true)
        };


        [Test]
        public void TestAddWallsNoWalls()
        {
            var navMesh = new NavMesh(LevelSizeInTiles, NoWallsShapes);
            Assert.IsEmpty(navMesh.walls);
        }

        [Test]
        public void TestAddWallsOneWall()
        {
            var navMesh = new NavMesh(LevelSizeInTiles, OneWallShapes);
            Assert.AreEqual(navMesh.walls, new HashSet<Point>
            {
                new Point {X = 1, Y = 1},
                new Point {X = 1, Y = 2},
                new Point {X = 1, Y = 3},
                new Point {X = 2, Y = 1},
                new Point {X = 2, Y = 2},
                new Point {X = 2, Y = 3},
                new Point {X = 3, Y = 1},
                new Point {X = 3, Y = 2},
            });
        }

        [Test]
        public void TestInBoundsTrue()
        {
            var point = new Point(1, 1);
            var navMesh = new NavMesh(LevelSizeInTiles, NoWallsShapes);
            Assert.IsTrue(navMesh.InBounds(point));
        }

        [Test]
        public void TestInBoundsFalse()
        {
            var point = new Point(-1, -1);
            var navMesh = new NavMesh(LevelSizeInTiles, NoWallsShapes);
            Assert.IsFalse(navMesh.InBounds(point));
        }

        [Test]
        public void TestPassableFalse()
        {
            var point = new Point(1, 2);
            var navMesh = new NavMesh(LevelSizeInTiles, OneWallShapes);
            Assert.IsFalse(navMesh.Passable(point));
        }

        [Test]
        public void TestPassableTrue()
        {
            var point = new Point(3, 3);
            var navMesh = new NavMesh(LevelSizeInTiles, OneWallShapes);
            Assert.IsTrue(navMesh.Passable(point));
        }
    }
}