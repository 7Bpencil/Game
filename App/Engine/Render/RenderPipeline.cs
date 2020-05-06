using System.Collections.Generic;
using System.Drawing;
using App.Engine.Particles;
using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidShapes;
using App.Model.Entities;
using App.Model.LevelData;

namespace App.Engine.Render
{
    public class RenderPipeline
    {
        private readonly RenderMachine renderMachine;

        public RenderPipeline(RenderMachine renderMachine)
        {
            this.renderMachine = renderMachine;
        }

        public void Start(
            Vector playerPosition, Vector cameraPosition, Size cameraSize, Weapon currentWeapon,
            List<SpriteContainer> sprites, List<AbstractParticleUnit> particles, List<Bullet> bullets, List<Edge> raytracingEdges)
        {
            //var visibilityPolygons = 
            //    Raytracing.CalculateVisibilityPolygon(raytracingEdges, playerPosition, 1000);
            RerenderCamera(cameraPosition, cameraSize);
            //renderMachine.RenderVisibilityPolygon(playerPosition, visibilityPolygons, cameraPosition);
            RenderSprites(sprites, cameraPosition);
            RenderParticles(particles, cameraPosition);
            RenderBullets(bullets, cameraPosition);
            renderMachine.RenderHUD(currentWeapon.Name + " " + currentWeapon.AmmoAmount, cameraSize);

            renderMachine.Invalidate();
        }

        public static Bitmap RenderLevelMap(List<Layer> layers, TileSet levelTileSet, int tileSize, Size levelSize)
        {
            RenderMachine.PrepareLevelMap(levelSize);
            foreach (var layer in layers)
                RenderLayer(layer.Tiles, layer.WidthInTiles, layer.HeightInTiles, tileSize, levelTileSet.Image);
            return RenderMachine.GetLevelMap();
        }
        
        public void RenderDebugInfo(
            Vector cameraPosition, Size cameraSize, ShapesIterator shapes, List<Bot> targets,
            List<CollisionInfo> collisionInfo, List<Edge> raytracingEdges, List<Collectable> items, List<Bullet> bullets,
            Vector cursorPosition, Vector playerPosition, RigidShape cameraChaser, string[] debugInfo)
        {
            RenderShapes(shapes, cameraPosition);
            RenderCollectablesShapes(items, cameraPosition);
            RenderRaytracingEdges(raytracingEdges, cameraPosition);
            RenderCollisionInfo(collisionInfo, cameraPosition);
            renderMachine.RenderDebugCross(cameraSize);
            renderMachine.RenderShapeOnCamera(cameraChaser, cameraPosition);
            renderMachine.RenderEdgeOnCamera(
                new Edge(cursorPosition.ConvertFromWorldToCamera(cameraPosition),
                    playerPosition.ConvertFromWorldToCamera(cameraPosition)));
            renderMachine.PrintMessages(debugInfo);
            RenderEnemyInfo(targets, cameraPosition);
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
            RenderMachine.RenderNewTile(tileMap, targetX * tileSize, targetY * tileSize, src);
        }

        private void RerenderCamera(Vector cameraPosition, Size cameraSize)
        {
            var sourceRectangle = new Rectangle(
                (int) cameraPosition.X, (int) cameraPosition.Y,
                cameraSize.Width, cameraSize.Height);
            
            renderMachine.RenderCamera(sourceRectangle);
        }
        
        private void RenderSprites(List<SpriteContainer> spriteContainers, Vector cameraPosition)
        {
            foreach (var container in spriteContainers)
                if (!container.IsEmpty()) renderMachine.RenderSpriteOnCamera(container, cameraPosition);
        }

        private void RenderParticles(List<AbstractParticleUnit> particleUnits, Vector cameraPosition)
        {
            foreach (var container in particleUnits)
            {
                if (container.ShouldBeBurned)
                {
                    renderMachine.BurnParticleOnRenderedTiles(container);
                    container.ShouldBeBurned = false;
                    container.IsExpired = true;
                }
                if (!container.IsExpired)
                    renderMachine.RenderParticleOnCamera(container, cameraPosition);
            }
        }

        private void RenderCollectablesShapes(List<Collectable> items, Vector cameraPosition)
        {
            foreach (var item in items)
            {
                if (item == null) continue;
                renderMachine.RenderShapeOnCamera(item.CollisionShape, cameraPosition);
            }
        }

        private void RenderShapes(ShapesIterator shapes, Vector cameraPosition)
        {
            for (var i = 0; i < shapes.Length; i++)
                renderMachine.RenderShapeOnCamera(shapes[i], cameraPosition);
        }

        private void RenderCollisionInfo(List<CollisionInfo> collisionInfo, Vector cameraPosition)
        {
            foreach (var info in collisionInfo)
                renderMachine.RenderCollisionInfoOnCamera(info, cameraPosition);
        }
        
        private void RenderRaytracingEdges(List<Edge> raytracingEdges, Vector cameraPosition)
        {
            foreach (var edge in raytracingEdges)
                renderMachine.RenderEdgeOnCamera(edge, cameraPosition);
        }

        private void RenderBullets(List<Bullet> bullets, Vector cameraPosition)
        {
            foreach (var bullet in bullets)
                if (!bullet.IsStuck) renderMachine.RenderEdgeOnCamera(bullet.Shape, cameraPosition);
        }

        private void RenderEnemyInfo(List<Bot> targets, Vector cameraPosition)
        {
            foreach (var t in targets)
            {
                var position = new Vector(
                    t.CollisionShape.Center.X - t.CollisionShape.Radius / 2,
                    t.CollisionShape.Center.Y - t.CollisionShape.Radius / 2);
                var positionInCamera = position.ConvertFromWorldToCamera(cameraPosition);
                renderMachine.PrintString(t.Health + "\n" + t.Armour, positionInCamera);    
            }
        }

        private static Rectangle GetSourceRectangle(int tileID, int columnsInTileMap, int tileSize)
        {
            var sourceX = tileID % columnsInTileMap * tileSize;
            var sourceY = tileID / columnsInTileMap * tileSize;
            return new Rectangle(sourceX, sourceY, tileSize - 1, tileSize - 1);
        }
    }
}