using System;
using System.Collections.Generic;
using System.Drawing;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using App.Model.Entities.Collectables;
using App.Model.Factories;

namespace App.Model.LevelData
{
    public class LevelInfo
    {
        public readonly Size LevelSizeInTiles;
        public readonly string Name;
        public readonly RigidAABB Exit;
        public readonly List<RigidShape> StaticShapes;
        public readonly Bitmap LevelMap;
        public readonly List<Edge> RaytracingEdges;
        public readonly NavMesh NavMesh;
        public readonly List<Vector> BotSpawnPoints;
        public readonly List<Vector> BotPatrolPoints;
        public readonly int WavesAmount;
        public readonly EntityFactory.PlayerInfo PlayerInfo;
        public readonly List<EntityFactory.BotInfo> BotsInfo;
        public readonly List<CollectableWeaponInfo> CollectableWeaponsInfo;

        public LevelInfo(
            Size levelSizeInTiles, string name, RigidAABB exit, List<RigidShape> staticShapes,
            Bitmap levelMap, List<Edge> raytracingEdges, NavMesh navMesh, List<Vector> botSpawnPoints,
            int wavesAmount, EntityFactory.PlayerInfo playerInfo, List<EntityFactory.BotInfo> botsInfo,
            List<CollectableWeaponInfo> collectableWeaponsInfo, List<Vector> botPatrolPoints)
        {
            LevelSizeInTiles = levelSizeInTiles;
            Name = name;
            Exit = exit;
            StaticShapes = staticShapes;
            LevelMap = levelMap;
            RaytracingEdges = raytracingEdges;
            NavMesh = navMesh;
            BotSpawnPoints = botSpawnPoints;
            BotPatrolPoints = botPatrolPoints;
            WavesAmount = wavesAmount;
            PlayerInfo = playerInfo;
            BotsInfo = botsInfo;
            CollectableWeaponsInfo = collectableWeaponsInfo;
        }
    }

    public class ShapesIterator
    {
        private readonly List<RigidShape> staticShapes;
        private readonly List<RigidShape> dynamicShapes;
        public int Length => staticShapes.Count + dynamicShapes.Count;

        public RigidShape this[int index]
        {
            get
            {
                if (index < 0 || index > Length - 1) throw new IndexOutOfRangeException();
                if (index < staticShapes.Count) return staticShapes[index];
                return dynamicShapes[index - staticShapes.Count];
            }
        }

        public ShapesIterator(List<RigidShape> staticShapes, List<RigidShape> dynamicShapes)
        {
            this.staticShapes = staticShapes;
            this.dynamicShapes = dynamicShapes;
        }
    }
}
