using System.Drawing;
using System.Drawing.Drawing2D;
using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidBody;
using App.Engine.Render.Renderers;
using App.View;

namespace App.Engine.Render
{
    public class RenderMachine
    {
        private ViewForm view;
        private Graphics gfxRenderedTiles;
        private Bitmap bmpRenderedTiles;
        private BufferedGraphics cameraBuffer;
        private Graphics gfxCamera;

        private readonly Font debugFont;
        private readonly Brush debugBrush;
        private readonly Pen debugPen;
        private readonly Pen collisionPen;

        public RenderMachine(ViewForm view, Size cameraSize)
        {
            this.view = view;
            
            var renderSize = new Size(45 * 32, 40 * 32); // TODO remove this const
            SetUpRenderer(renderSize, cameraSize);
            
            debugFont = new Font("Arial", 18, FontStyle.Regular, GraphicsUnit.Pixel);
            debugBrush = new SolidBrush(Color.White);
            debugPen = new Pen(Color.White, 4);
            collisionPen = new Pen(Color.Crimson, 4);
        }
        
        private void SetUpRenderer(Size renderSize, Size cameraSize)
        {
            bmpRenderedTiles = new Bitmap(renderSize.Width, renderSize.Height);
            gfxRenderedTiles = Graphics.FromImage(bmpRenderedTiles);
         
            SetCameraBuffer(cameraSize);
            gfxCamera = cameraBuffer.Graphics;
            gfxCamera.InterpolationMode = InterpolationMode.Bilinear;
        }

        private void SetCameraBuffer(Size cameraSize)
        {
            var context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(cameraSize.Width + 1, cameraSize.Height + 1);
            using (var g = view.CreateGraphics())
                cameraBuffer = context.Allocate(g, new Rectangle(0, 0, cameraSize.Width, cameraSize.Height));
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

        public BufferedGraphics GetCameraBuffer()
        {
            return cameraBuffer;
        }
    }
}