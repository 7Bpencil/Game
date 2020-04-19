using System.Collections.Generic;
using System.Drawing;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.Collision;
using App.Engine.PhysicsEngine.RigidBody;
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

        public void Start(Vector cameraPosition, Size cameraSize, List<Sprite> sprites)
        {
            RerenderCamera(cameraPosition, cameraSize);
            RenderSprites(sprites, cameraPosition);

            renderMachine.Invalidate();
        }

        public void Load(Level currentLevel)
        {
            var tileSize = currentLevel.TileSet.tileWidth;
            foreach (var layer in currentLevel.Layers)
                RenderLayer(layer, currentLevel.TileSet.image, tileSize);
        }
        
        public void RenderDebugInfo(
            Vector cameraPosition, Size cameraSize, List<RigidShape> shapes, 
            List<CollisionInfo> collisionInfo, List<Polygon> raytracingPolygons, 
            Vector cursorPosition, Vector playerPosition, RigidShape cameraChaser,
            Size levelSizeInTiles)
        {
            RenderShapes(shapes, cameraPosition);
            RenderRaytracingPolygons(raytracingPolygons, cameraPosition);
            RenderCollisionInfo(collisionInfo, cameraPosition);
            renderMachine.RenderDebugCross(cameraSize);
            renderMachine.RenderShapeOnCamera(cameraChaser, cameraPosition);
            renderMachine.RenderEdgeOnCamera(
                new Edge(cursorPosition.ConvertFromWorldToCamera(cameraPosition),
                    playerPosition.ConvertFromWorldToCamera(cameraPosition)));
            renderMachine.PrintMessages(
                GetDebugInfo(cameraPosition, cameraSize, playerPosition, cursorPosition, levelSizeInTiles));
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
        
        private void RenderSprites(List<Sprite> sprites, Vector cameraPosition)
        {
            foreach (var sprite in sprites)
                renderMachine.RenderSpriteOnCamera(sprite, cameraPosition);
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
        
        private void RenderRaytracingPolygons(List<Polygon> raytracingPolygons, Vector cameraPosition)
        {
            foreach (var polygon in raytracingPolygons)
                renderMachine.RenderPolygonOnCamera(polygon, cameraPosition);
        }
        
        private Rectangle GetSourceRectangle(int tileID, int columnsInTileMap, int tileSize)
        {
            var sourceX = tileID % columnsInTileMap * tileSize;
            var sourceY = tileID / columnsInTileMap * tileSize;
            return new Rectangle(sourceX, sourceY, tileSize - 1, tileSize - 1);
        }
        
        private string[] GetDebugInfo(
            Vector cameraPosition, Size cameraSize, Vector playerPosition, Vector cursorPosition, Size levelSizeInTiles)
        {
            return new []
            {
                "Camera Size: " + cameraSize.Width + " x " + cameraSize.Height,
                "Scene Size (in Tiles): " + levelSizeInTiles.Width + " x " + levelSizeInTiles.Height,
                "(WAxis) Scroll Position: " + cameraPosition,
                "(WAxis) Player Position: " + playerPosition,
                "(CAxis) Player Position: " + playerPosition.ConvertFromWorldToCamera(cameraPosition),
                "(CAxis) Cursor Position: " + cursorPosition
            };
        }
    }
}