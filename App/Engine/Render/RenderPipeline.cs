using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Model.Entities;
using App.Model.LevelData;

namespace App.Engine.Render
{
    public static class RenderPipeline
    {
        private static readonly Stopwatch Clock = new Stopwatch();
        
        public static void Render(Level level, Camera camera, Vector cursorPosition, bool renderRaytracing, bool renderDebug)
        {
            var cameraPosition = camera.Position;
            var cameraSize = camera.Size;
            
            Clock.Start();
            
            RerenderCamera(cameraPosition, cameraSize);
            var rC = Measure();

            RenderSprites(level.Sprites, cameraPosition);
            var rS = Measure();

            RenderParticles(level.Particles, cameraPosition);
            var rP = Measure();

            RenderBullets(level.Bullets, cameraPosition);
            var rB = Measure();
            

            var rT = 0L;
            if (renderRaytracing)
            {
                RenderVisibilityRegions(level.VisibilityRegions, cameraPosition);
                RenderMachine.RenderShadowMask();
                rT += Measure();
            }
            Clock.Stop();
            Clock.Reset();

            var perfomanceInfo = 
                "summary: " + (rC + rS + rP + rB + rT).ToString() +
                "ms, camera: " + rC.ToString() + 
                "ms, sprites: " + rS.ToString() + 
                "ms, particles: " + rP.ToString() + 
                "ms, projectiles: " + rB.ToString() + 
                "ms, raytracing: " + rT.ToString(); 
            if (renderDebug) RenderDebugInfo(level, camera, cursorPosition, perfomanceInfo);
            
            var playerWeapon = level.Player.CurrentWeapon;
            RenderMachine.RenderHUD(playerWeapon.Name + " " + playerWeapon.AmmoAmount, cameraSize);

            RenderMachine.Invalidate();
        }

        private static void RenderDebugInfo(Level level, Camera camera, Vector cursorPosition, string perfomanceInfo)
        {
            var playerPosition = level.Player.Position;
            var cameraPosition = camera.Position;
            var cameraSize = camera.Size;

            RenderNavMesh(level.NavMesh, cameraPosition);
            RenderPaths(level.Paths, cameraPosition);
            RenderShapes(level.SceneShapes, cameraPosition);
            RenderCollectablesShapes(level.Collectables, cameraPosition);
            RenderRaytracingEdges(level.RaytracingEdges, cameraPosition);
            RenderCollisionInfo(level.CollisionsInfo, cameraPosition);
            RenderMachine.RenderDebugCross(cameraSize);
            RenderMachine.RenderShapeOnCamera(camera.GetChaser(), cameraPosition);
            RenderMachine.RenderEdgeOnCamera(
                new Edge(cursorPosition.ConvertFromWorldToCamera(cameraPosition),
                    playerPosition.ConvertFromWorldToCamera(cameraPosition)));

            var debugInfo = new []
            {
                perfomanceInfo,
                "Camera Size: " + camera.Size.Width.ToString() + " x " + camera.Size.Height.ToString(),
                "Scene Size (in Tiles): " + level.LevelSizeInTiles.Width.ToString() + " x " + level.LevelSizeInTiles.Height.ToString(),
                "(WAxis) Scroll Position: " + camera.Position,
                "(WAxis) Player Position: " + playerPosition,
                "(CAxis) Player Position: " + playerPosition.ConvertFromWorldToCamera(camera.Position),
                "(CAxis) Cursor Position: " + cursorPosition
            };
            RenderMachine.PrintMessages(debugInfo);
            RenderEnemyInfo(level.Bots, cameraPosition);
        }

        public static Bitmap RenderLevelMap(List<Layer> layers, TileSet levelTileSet, int tileSize, Size levelSizeInTiles)
        {
            RenderMachine.PrepareLevelMap(
                new Size(levelSizeInTiles.Width * tileSize, levelSizeInTiles.Height * tileSize));
            foreach (var layer in layers)
                RenderLayer(layer.Tiles, levelSizeInTiles.Width, levelSizeInTiles.Height, tileSize, levelTileSet.Image);
            return RenderMachine.GetLevelMapCopy();
        }

        private static void RenderLayer(
            int[] tiles, int layerWidthInTiles, int layerHeightInTiles, int tileSize, Bitmap levelTileMap)
        {
            for (var x = 0; x <= layerWidthInTiles; ++x)
            for (var y = 0; y <= layerHeightInTiles; ++y)
            {
                var tileIndex = y * layerWidthInTiles + x;
                if (tileIndex > tiles.Length - 1) break;
                
                var tileID = tiles[tileIndex];
                if (tileID == 0) continue;
                
                RenderTile(x, y, tileID - 1, levelTileMap, tileSize);
            }
        }

        private static void RenderTile(int targetX, int targetY, int tileID, Bitmap tileMap, int tileSize)
        {
            var src = GetSourceRectangle(tileID, tileMap.Width / tileSize, tileSize);
            RenderMachine.RenderTile(tileMap, targetX * tileSize, targetY * tileSize, src);
        }

        private static void RerenderCamera(Vector cameraPosition, Size cameraSize)
        {
            var sourceRectangle = new Rectangle(
                (int) cameraPosition.X, (int) cameraPosition.Y,
                cameraSize.Width, cameraSize.Height);
            
            RenderMachine.RenderCamera(sourceRectangle);
        }
        
        private static void RenderSprites(List<SpriteContainer> spriteContainers, Vector cameraPosition)
        {
            foreach (var container in spriteContainers)
                if (!container.IsEmpty) RenderMachine.RenderSpriteOnCamera(container, cameraPosition);
        }

        private static void RenderParticles(List<AbstractParticleUnit> particleUnits, Vector cameraPosition)
        {
            foreach (var container in particleUnits)
            {
                if (container.ShouldBeBurned)
                {
                    RenderMachine.BurnParticleOnRenderedTiles(container);
                    container.ShouldBeBurned = false;
                    container.IsExpired = true;
                    container.ClearContent();
                }
                if (!container.IsExpired)
                    RenderMachine.RenderParticleOnCamera(container, cameraPosition);
            }
        }

        private static void RenderCollectablesShapes(List<Collectable> items, Vector cameraPosition)
        {
            foreach (var item in items)
            {
                if (item == null) continue;
                RenderMachine.RenderShapeOnCamera(item.CollisionShape, cameraPosition);
            }
        }
        
        private static void RenderVisibilityRegions(List<Raytracing.VisibilityRegion> visibilityRegions, Vector cameraPosition)
        {
            RenderMachine.PrepareShadowMask();
            foreach (var region in visibilityRegions)
            {
                RenderMachine.RenderVisibilityRegion(region, cameraPosition);
            }
        }

        private static void RenderShapes(ShapesIterator shapes, Vector cameraPosition)
        {
            for (var i = 0; i < shapes.Length; i++)
                RenderMachine.RenderShapeOnCamera(shapes[i], cameraPosition);
        }

        private static void RenderCollisionInfo(List<CollisionInfo> collisionInfo, Vector cameraPosition)
        {
            foreach (var info in collisionInfo)
                RenderMachine.RenderCollisionInfoOnCamera(info, cameraPosition);
        }
        
        private static void RenderRaytracingEdges(List<Edge> raytracingEdges, Vector cameraPosition)
        {
            foreach (var edge in raytracingEdges)
                RenderMachine.RenderEdgeOnCamera(edge, cameraPosition);
        }

        private static void RenderBullets(List<AbstractProjectile> projectiles, Vector cameraPosition)
        {
            foreach (var projectile in projectiles)
            {
                if (!(projectile is Bullet)) continue;
                var bullet = (Bullet) projectile;
                if (bullet.IsStuck) continue;
                if (bullet.IsDeformed)
                {
                    RenderMachine.RenderEdgeOnTiles(bullet.Shape);
                    bullet.IsStuck = true;
                }
                else RenderMachine.RenderEdgeOnCamera(bullet.Shape, cameraPosition);
            }
        }

        private static void RenderEnemyInfo(List<Bot> targets, Vector cameraPosition)
        {
            foreach (var t in targets)
            {
                var position = new Vector(
                    t.CollisionShape.Center.X - t.CollisionShape.Radius / 2,
                    t.CollisionShape.Center.Y - t.CollisionShape.Radius / 2);
                var positionInCamera = position.ConvertFromWorldToCamera(cameraPosition);
                RenderMachine.PrintString(t.Health.ToString() + "\n" + t.Armor.ToString(), positionInCamera);    
            }
        }

        private static void RenderNavMesh(NavMesh navMesh, Vector cameraPosition)
        {
            var points = navMesh.RenderForm.Points;
            var edges = navMesh.RenderForm.Edges;
            foreach (var p in points)
                RenderMachine.RenderPoint(p, cameraPosition);
            foreach (var edge in edges)
                RenderMachine.RenderEdgeOnCamera(edge, cameraPosition);
        }

        private static void RenderPaths(List<List<Vector>> paths, Vector cameraPosition)
        {
            foreach (var path in paths)
                RenderMachine.RenderPath(path, cameraPosition);
        }

        private static Rectangle GetSourceRectangle(int tileID, int columnsInTileMap, int tileSize)
        {
            var sourceX = tileID % columnsInTileMap * tileSize;
            var sourceY = tileID / columnsInTileMap * tileSize;
            return new Rectangle(sourceX, sourceY, tileSize - 1, tileSize - 1);
        }

        private static long Measure()
        {
            Clock.Stop();
            var result = Clock.ElapsedMilliseconds;
            Clock.Reset();
            Clock.Start();
            return result;
        }
    }
}