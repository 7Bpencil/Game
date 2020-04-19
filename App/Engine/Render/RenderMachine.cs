using System.Drawing;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.Collision;
using App.Engine.PhysicsEngine.RigidBody;
using App.Engine.Render.Renderers;
using App.View;

namespace App.Engine.Render
{
    public class RenderMachine
    {
        private ViewForm view;
        private Graphics gfxRenderedTiles;
        private Bitmap bmpRenderedTiles;
        private Graphics gfxCamera;

        private readonly Font debugFont;
        private readonly Brush debugBrush;
        private readonly Pen debugPen;
        private readonly Pen collisionPen;

        public RenderMachine(ViewForm view, Graphics gfxRenderedTiles, Bitmap bmpRenderedTiles, Graphics gfxCamera)
        {
            this.view = view;
            this.gfxRenderedTiles = gfxRenderedTiles;
            this.bmpRenderedTiles = bmpRenderedTiles;
            this.gfxCamera = gfxCamera;
            
            debugFont = new Font("Arial", 18, FontStyle.Regular, GraphicsUnit.Pixel);
            debugBrush = new SolidBrush(Color.White);
            debugPen = new Pen(Color.White, 4);
            collisionPen = new Pen(Color.Crimson, 4);
        }

        public void Invalidate()
        {
            view.Invalidate();
        }

        public void RenderTile(Bitmap tileMap, int x, int y, Rectangle src)
        {
            gfxRenderedTiles.DrawImage(tileMap, x, y, src, GraphicsUnit.Pixel);
        }

        public void RenderCamera(Rectangle sourceRectangle)
        {
            gfxCamera.DrawImage(bmpRenderedTiles, 0, 0, sourceRectangle, GraphicsUnit.Pixel);
        }

        public void RenderSpriteOnCamera(Sprite sprite, Vector cameraPosition)
        {
            sprite.DrawNextFrame(gfxCamera, cameraPosition);
        }
        
        public void RenderSpriteOnCamera(Sprite sprite)
        {
            sprite.DrawNextFrame(gfxCamera);
        }

        public void RenderPolygonOnCamera(Polygon polygon, Vector cameraPosition)
        {
            PolygonRenderer.Draw(polygon, cameraPosition, debugPen, gfxCamera);
        }

        public void RenderEdgeOnCamera(Edge edge)
        {
            EdgeRenderer.Draw(edge, debugPen, gfxCamera);
        }

        public void RenderShapeOnCamera(RigidShape shape, Vector cameraPosition)
        {
            RigidBodyRenderer.Draw(shape, cameraPosition, debugPen, gfxCamera);
        }

        public void RenderCollisionInfoOnCamera(CollisionInfo info, Vector cameraPosition)
        {
            CollisionInfoRenderer.Draw(info, cameraPosition, collisionPen, gfxCamera);
        }
        
        public void PrintMessages(string[] messages)
        {
            for (var i = 0; i < messages.Length; i++)
                gfxCamera.DrawString(messages[i], debugFont, debugBrush, 0, i * debugFont.Height);
        }

        public void RenderDebugCross(Size cameraSize)
        {
            var a = cameraSize.Width / 2;
            var b = cameraSize.Height / 2;
            var verticalEdge = new Edge(a, 0, a, cameraSize.Height);
            var horizontalEdge = new Edge(0, b, cameraSize.Width, b);
            RenderEdgeOnCamera(verticalEdge);
            RenderEdgeOnCamera(horizontalEdge);
        }
    }
}