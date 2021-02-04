using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidShapes;
using App.Engine.Render.Renderers;
using App.Model.Entities;
using App.View;

namespace App.Engine.Render
{
    public static class RenderMachine
    {
        private static ViewForm view;
        
        private static BufferedGraphics cameraBuffer;
        private static Graphics gfxCamera;
        
        private static Bitmap bmpLevelMap;
        private static Graphics gfxLevelMap;
        
        private static Bitmap bmpShadowMask;
        private static Graphics gfxShadowMask;

        private static readonly Font DebugFont = new Font("Arial", 18, FontStyle.Regular, GraphicsUnit.Pixel);
        private static readonly Brush DebugBrush = new SolidBrush(Color.White);
        private static readonly Brush AnotherDebugBrush = new SolidBrush(Color.Gold);
        private static readonly Pen ShapePen = new Pen(Color.White, 4);
        private static readonly Pen CollisionPen = new Pen(Color.Crimson, 4);
        private static readonly Pen RaytracingEdgePen = new Pen(Color.Salmon, 4);
        private static readonly Brush PenetrationBrush = new SolidBrush(Color.Maroon);
        private static readonly Brush TransparentBrush = new SolidBrush(Color.FromArgb(0, Color.Empty));
        private static readonly Color ShadowColor = Color.FromArgb(128, Color.Black);

        public static void Initialize(ViewForm viewForm, Size cameraSize)
        {
            view = viewForm;
            SetCameraBuffer(cameraSize);
            
            gfxCamera = cameraBuffer.Graphics;
            gfxCamera.InterpolationMode = InterpolationMode.Bilinear;
            
            bmpShadowMask = new Bitmap(cameraSize.Width, cameraSize.Height);
            gfxShadowMask = Graphics.FromImage(bmpShadowMask);
            gfxShadowMask.CompositingMode = CompositingMode.SourceCopy;
        }

        private static void SetCameraBuffer(Size cameraSize)
        {
            var context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(cameraSize.Width + 1, cameraSize.Height + 1);
            using (var g = view.CreateGraphics())
                cameraBuffer = context.Allocate(g, new Rectangle(0, 0, cameraSize.Width, cameraSize.Height));
        }

        public static void Invalidate()
        {
            view.Invalidate();
        }

        public static void RenderShadowMask()
        {
            gfxCamera.DrawImage(bmpShadowMask, 0, 0, bmpShadowMask.Width, bmpShadowMask.Height);
        }
        
        public static void PrepareLevelMap(Size levelSize)
        {
            bmpLevelMap = new Bitmap(levelSize.Width, levelSize.Height);
            gfxLevelMap = Graphics.FromImage(bmpLevelMap);
        }
        
        public static void RenderTile(Bitmap tileMap, int x, int y, Rectangle src)
        {
            gfxLevelMap.DrawImage(tileMap, x, y, src, GraphicsUnit.Pixel);
        }

        public static Bitmap GetLevelMapCopy()
        {
            return (Bitmap) bmpLevelMap.Clone();
        }

        public static void RenderCamera(Rectangle sourceRectangle)
        {
            gfxCamera.DrawImage(bmpLevelMap, 0, 0, sourceRectangle, GraphicsUnit.Pixel);
        }

        public static void RenderSpriteOnCamera(SpriteContainer container, Vector cameraPosition)
        {
            SpriteRenderer.DrawNextFrame(container.Content, container.CenterPosition, container.Angle, cameraPosition, gfxCamera);
        }

        public static void RenderParticleOnCamera(AbstractParticleUnit unit, Vector cameraPosition)
        {
            SpriteRenderer.DrawNextFrame(unit.Content, unit.CurrentFrame, unit.CenterPosition, unit.Angle, cameraPosition, gfxCamera);
        }

        public static void BurnParticleOnRenderedTiles(AbstractParticleUnit unit)
        {
            SpriteRenderer.DrawNextFrame(unit.Content, unit.CurrentFrame, unit.CenterPosition, unit.Angle, gfxLevelMap);
        }

        public static void RenderEdgeOnCamera(Edge edge)
        {
            EdgeRenderer.Draw(edge, RaytracingEdgePen, gfxCamera);
        }

        public static void RenderEdgeOnCamera(Edge edge, Vector cameraPosition)
        {
            EdgeRenderer.Draw(edge, cameraPosition, RaytracingEdgePen, gfxCamera);
        }
        
        public static void RenderEdgeOnTiles(Edge edge)
        {
            EdgeRenderer.Draw(edge, RaytracingEdgePen, gfxLevelMap);
        }

        public static void RenderShapeOnCamera(RigidShape shape, Vector cameraPosition)
        {
            RigidBodyRenderer.Draw(shape, cameraPosition, ShapePen, gfxCamera);
        }

        public static void RenderVisibilityRegion(Raytracing.VisibilityRegion region, Vector cameraPosition)
        {
            gfxShadowMask.Clear(ShadowColor);

            VisibilityPolygonRenderer.Draw(
                region.LightSourcePosition,
                region.VisibilityRegionPoints,
                cameraPosition,
                TransparentBrush,
                gfxShadowMask);
        }
        
        public static void PrepareShadowMask()
        {
            gfxShadowMask.Clear(ShadowColor);
        }

        public static void RenderCollisionInfoOnCamera(CollisionInfo info, Vector cameraPosition)
        {
            CollisionInfoRenderer.Draw(info, cameraPosition, CollisionPen, gfxCamera);
        }

        public static void RenderPoint(Vector point, Vector cameraPosition)
        {
            VectorRenderer.Fill(point, cameraPosition, PenetrationBrush, gfxCamera);
        }

        public static void RenderPath(List<Vector> path, Vector cameraPosition)
        {
            PathRenderer.Draw(path, cameraPosition, gfxCamera);
        }
        
        public static void PrintMessages(string[] messages)
        {
            for (var i = 0; i < messages.Length; i++)
                gfxCamera.DrawString(messages[i], DebugFont, DebugBrush, 0, i * DebugFont.Height);
        }

        public static void PrintString(string message, Vector position)
        {
            gfxCamera.DrawString(message, DebugFont, AnotherDebugBrush, position.X, position.Y);
        }

        public static void RenderHUD(string weaponInfo, Size cameraSize)
        {
            gfxCamera.DrawString(weaponInfo, DebugFont, DebugBrush, 0, cameraSize.Height - DebugFont.Height);
        }

        public static void RenderDebugCross(Size cameraSize)
        {
            var a = cameraSize.Width / 2;
            var b = cameraSize.Height / 2;
            var verticalEdge = new Edge(a, 0, a, cameraSize.Height);
            var horizontalEdge = new Edge(0, b, cameraSize.Width, b);
            EdgeRenderer.Draw(verticalEdge, ShapePen, gfxCamera);
            EdgeRenderer.Draw(horizontalEdge, ShapePen, gfxCamera);
        }

        public static BufferedGraphics GetCameraBuffer()
        {
            return cameraBuffer;
        }

        public static void ResetLevelMap(Bitmap levelMap)
        {
            bmpLevelMap = (Bitmap) levelMap.Clone();
            gfxLevelMap = Graphics.FromImage(bmpLevelMap);
        }
    }
}