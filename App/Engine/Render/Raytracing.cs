using System;
using System.Collections.Generic;
using System.Linq;
using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Physics.RigidShapes;

namespace App.Engine.Render
{
    public static class Raytracing
    {
        private static readonly RaytracingPointsEqualityComparer Comparer = new RaytracingPointsEqualityComparer();
        
        public static List<RaytracingPoint> CalculateVisibilityPolygon(List<Edge> edges, Vector lightSourcePosition, float visibilityRadius)
        {
            var points = new List<RaytracingPoint> {Capacity = 200};
            foreach (var edge in edges)
            {
                for (var i = 0; i < 2; i++)
                {
                    var rdx = (i == 0 ? edge.Start.X : edge.End.X) - lightSourcePosition.X;
                    var rdy = (i == 0 ? edge.Start.Y : edge.End.Y) - lightSourcePosition.Y;

                    var baseAngle = (float) Math.Atan2(rdy, rdx);
                    var angle = 0f;

                    for (var j = 0; j < 3; j++)
                    {
                        if (j == 0) angle = baseAngle - 0.0001f;
                        if (j == 1) angle = baseAngle;
                        if (j == 2) angle = baseAngle + 0.0001f;

                        rdx = visibilityRadius * (float) Math.Cos(angle);
                        rdy = visibilityRadius * (float) Math.Sin(angle);

                        var minT = float.PositiveInfinity;
                        float minPX = 0, minPY = 0, minAngle = 0;
                        var bValid = false;

                        foreach (var otherEdge in edges)
                        {
                            var sdx = otherEdge.End.X - otherEdge.Start.X;
                            var sdy = otherEdge.End.Y - otherEdge.Start.Y;

                            if (Math.Abs(sdx - rdx) > 0.0f && Math.Abs(sdy - rdy) > 0.0f)
                            {
                                var t2 = (rdx * (otherEdge.Start.Y - lightSourcePosition.Y)
                                          + rdy * (lightSourcePosition.X - otherEdge.Start.X))
                                         / (sdx * rdy - sdy * rdx);
                                var t1 = (otherEdge.Start.X + sdx * t2 - lightSourcePosition.X) / rdx;
                                
                                if (t1 > 0 && t2 >= 0 && t2 <= 1.0f)
                                {
                                    if (t1 < minT)
                                    {
                                        minT = t1;
                                        minPX = lightSourcePosition.X + rdx * t1;
                                        minPY = lightSourcePosition.Y + rdy * t1;
                                        minAngle = (float) Math.Atan2(minPY - lightSourcePosition.Y, minPX - lightSourcePosition.X);
                                        bValid = true;
                                    }
                                }
                            }
                        }
                        
                        if(bValid) points.Add(new RaytracingPoint(minAngle, new Vector(minPX, minPY)));
                    }
                }
            }

            var uniquePoints = points.Distinct(Comparer);
            return uniquePoints.OrderBy(n => n.Angle).ToList();
        }

        public readonly struct RaytracingPoint
        {
            public readonly float Angle;
            public readonly Vector Position;

            public RaytracingPoint(float angle, Vector position)
            {
                Angle = angle;
                Position = position;
            }
        }
        
        private class RaytracingPointsEqualityComparer : IEqualityComparer<RaytracingPoint>
        {
            public bool Equals(RaytracingPoint first, RaytracingPoint second)
            {
                return Math.Abs(first.Position.X - second.Position.X) < 0.1f
                       && Math.Abs(first.Position.Y - second.Position.Y) < 0.1f;
            }

            public int GetHashCode(RaytracingPoint point)
            {
                return (int) point.Angle + point.Position.GetHashCode();
            }
        }

        public static Vector IsInView(RigidCircle circle, Vector lightSourcePosition, List<Edge> edges, float visibilityRadius)
        {
            var visibilityPolygonPoints = CalculateVisibilityPolygon(edges, lightSourcePosition, visibilityRadius);
            CollisionInfo info;
            for (var i = 0; i < visibilityPolygonPoints.Count - 1; i++)
            {
                info = CollisionSolver.GetCollisionInfo(
                    circle,
                    lightSourcePosition,
                    visibilityPolygonPoints[i].Position,
                    visibilityPolygonPoints[i + 1].Position);
                if (info != null) return info.Start;
            }
            info = CollisionSolver.GetCollisionInfo(
                circle,
                lightSourcePosition,
                visibilityPolygonPoints[visibilityPolygonPoints.Count - 1].Position,
                visibilityPolygonPoints[0].Position);
            
            return info?.Start;
        }
    }
}