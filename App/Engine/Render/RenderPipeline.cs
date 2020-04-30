using System.Collections.Generic;
using System.Drawing;
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
            List<SpriteContainer> sprites, List<Bullet> bullets, List<Edge> raytracingEdges)
        {
            //var visibilityPolygons = 
            //    Raytracing.CalculateVisibilityPolygon(raytracingEdges, playerPosition, 1000);
            RerenderCamera(cameraPosition, cameraSize);
            //renderMachine.RenderVisibilityPolygon(playerPosition, visibilityPolygons, cameraPosition);
            RenderSprites(sprites, cameraPosition);
            RenderBullets(bullets, cameraPosition);
            RenderDynamicPenetrations(bullets, cameraPosition);
            renderMachine.RenderHUD(currentWeapon.Name + " " + currentWeapon.AmmoAmount, cameraSize);

            renderMachine.Invalidate();
        }

        public void Load(Level currentLevel)
        {
            var tileSize = currentLevel.TileSet.tileWidth;
            foreach (var layer in currentLevel.Layers)
                RenderLayer(layer, currentLevel.TileSet.image, tileSize);
        }
        
        public void RenderDebugInfo(
            Vector cameraPosition, Size cameraSize, List<RigidShape> shapes, List<ShootingRangeTarget> targets,
            List<CollisionInfo> collisionInfo, List<Edge> raytracingEdges, List<Collectable> items, List<Bullet> bullets,
            Vector cursorPosition, Vector playerPosition, RigidShape cameraChaser, string[] debugInfo)
        {
            RenderShapes(shapes, cameraPosition);
            RenderCollectablesShapes(items, cameraPosition);
            RenderRaytracingEdges(raytracingEdges, cameraPosition);
            RenderCollisionInfo(collisionInfo, cameraPosition);
            RenderStaticPenetrations(bullets, cameraPosition);
            renderMachine.RenderDebugCross(cameraSize);
            renderMachine.RenderShapeOnCamera(cameraChaser, cameraPosition);
            renderMachine.RenderEdgeOnCamera(
                new Edge(cursorPosition.ConvertFromWorldToCamera(cameraPosition),
                    playerPosition.ConvertFromWorldToCamera(cameraPosition)));
            renderMachine.PrintMessages(debugInfo);
            RenderEnemyInfo(targets, cameraPosition);
        }
        
        private void RenderLayer(Layer layer, Bitmap levelTileMap, int tileSize)
        {
            for (var x = 0; x <= layer.WidthInTiles; ++x)
            for (var y = 0; y <= layer.HeightInTiles; ++y)
            {
                var tileIndex = y * layer.WidthInTiles + x;
                if (tileIndex > layer.Tiles.Length - 1) break;
                
                var tileID = layer.Tiles[tileIndex];
                if (tileID == 0) continue;
                
                RenderTile(x, y, tileID - 1, levelTileMap, tileSize);
            }
        }

        private void RenderTile(int targetX, int targetY, int tileID, Bitmap tileMap, int tileSize)
        {
            var src = GetSourceRectangle(tileID, tileMap.Width / tileSize, tileSize);
            renderMachine.RenderTile(tileMap, targetX * tileSize, targetY * tileSize, src);
        }

        private void RerenderCamera(Vector cameraPosition, Size cameraSize)
        {
            var sourceRectangle = new Rectangle(
                (int) cameraPosition.X, (int) cameraPosition.Y,
                cameraSize.Width, cameraSize.Height);
            
            renderMachine.RenderCamera(sourceRectangle);
        }
        
        private void RenderSprites(List<SpriteContainer> sprites, Vector cameraPosition)
        {
            foreach (var container in sprites)
                if (container.Content != null) renderMachine.RenderSpriteOnCamera(container.Content, cameraPosition);
        }

        private void RenderCollectablesShapes(List<Collectable> items, Vector cameraPosition)
        {
            foreach (var item in items)
            {
                if (item == null) continue;
                renderMachine.RenderShapeOnCamera(item.CollisionShape, cameraPosition);
            }
        }

        private void RenderShapes(List<RigidShape> shapes, Vector cameraPosition)
        {
            foreach (var shape in shapes)
                renderMachine.RenderShapeOnCamera(shape, cameraPosition);
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
            {
                if (bullet == null) continue;
                renderMachine.RenderEdgeOnCamera(bullet.shape, cameraPosition);
            }
        }

        private void RenderDynamicPenetrations(List<Bullet> bullets, Vector cameraPositions)
        {
            foreach (var bullet in bullets)
            {
                if (bullet == null) continue;
                foreach (var point in bullet.collisionWithDynamicInfo)
                    renderMachine.RenderPoint(point, cameraPositions);
            }
        }
        
        private void RenderStaticPenetrations(List<Bullet> bullets, Vector cameraPositions)
        {
            foreach (var bullet in bullets)
            {
                if (bullet == null) continue;
                foreach (var points in bullet.collisionWithStaticInfo)
                {
                    renderMachine.RenderPoint(points[0], cameraPositions);
                    renderMachine.RenderPoint(points[1], cameraPositions);
                }
            }
        }

        private void RenderEnemyInfo(List<ShootingRangeTarget> targets, Vector cameraPosition)
        {
            foreach (var t in targets)
            {
                var position = new Vector(
                    t.collisionShape.Center.X - t.collisionShape.Radius / 2,
                    t.collisionShape.Center.Y - t.collisionShape.Radius / 2);
                var positionInCamera = position.ConvertFromWorldToCamera(cameraPosition);
                renderMachine.PrintString(t.Health + "\n" + t.Armour, positionInCamera);    
            }
        }

        private Rectangle GetSourceRectangle(int tileID, int columnsInTileMap, int tileSize)
        {
            var sourceX = tileID % columnsInTileMap * tileSize;
            var sourceY = tileID / columnsInTileMap * tileSize;
            return new Rectangle(sourceX, sourceY, tileSize - 1, tileSize - 1);
        }
    }
}