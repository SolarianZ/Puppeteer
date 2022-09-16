using System;
using System.Collections.Generic;
using UnityEngine;

namespace GBG.AnimationGraph.ThirdParties
{
    // Upstream: https://github.com/godotengine/godot/blob/master/core/math/delaunay_2d.h
    // Copyright (c) 2007-2022 Juan Linietsky, Ariel Manzur.
    // Copyright (c) 2014-2022 Godot Engine contributors
    // License: MIT

    public static class Delaunay2D
    {
        public static List<Triangle> Triangulate(List<Vector2> points)
        {
            List<Triangle> triangles = new List<Triangle>();

            int sourcePointCount = points.Count;
            Rect rect = default;
            for (int i = 0; i < sourcePointCount; i++)
            {
                if (i == 0)
                {
                    rect.Position = points[i];
                }
                else
                {
                    rect.ExpandTo(points[i]);
                }
            }

            float deltaMax = Math.Max(rect.Size.x, rect.Size.y);
            Vector2 center = rect.Center;

            points.Add(new Vector2(center.x - 20 * deltaMax, center.y - deltaMax));
            points.Add(new Vector2(center.x, center.y + 20 * deltaMax));
            points.Add(new Vector2(center.x + 20 * deltaMax, center.y - deltaMax));

            triangles.Add(new Triangle(sourcePointCount + 0, sourcePointCount + 1, sourcePointCount + 2));

            List<Edge> polygon = new List<Edge>();
            for (int i = 0; i < sourcePointCount; i++)
            {
                polygon.Clear();
                for (int j = 0; j < triangles.Count; j++)
                {
                    if (CircumCircleContains(points, triangles[j], i))
                    {
                        var triangle = triangles[j];
                        triangle.Bad = true;
                        triangles[j] = triangle;

                        polygon.Add(new Edge(triangles[j].Point0, triangles[j].Point1));
                        polygon.Add(new Edge(triangles[j].Point1, triangles[j].Point2));
                        polygon.Add(new Edge(triangles[j].Point2, triangles[j].Point0));
                    }
                }

                for (int j = 0; j < triangles.Count; j++)
                {
                    if (triangles[j].Bad)
                    {
                        triangles.RemoveAt(j);
                        j--;
                    }
                }

                for (int j = 0; j < polygon.Count; j++)
                {
                    for (int k = j + 1; k < polygon.Count; k++)
                    {
                        if (EdgeCompare(points, polygon[j], polygon[k]))
                        {
                            var edgeJ = polygon[j];
                            edgeJ.Bad = true;
                            polygon[j] = edgeJ;

                            var edgeK = polygon[k];
                            edgeK.Bad = true;
                            polygon[k] = edgeK;
                        }
                    }
                }

                for (int j = 0; j < polygon.Count; j++)
                {
                    if (polygon[j].Bad)
                    {
                        continue;
                    }

                    triangles.Add(new Triangle(polygon[j].Point0, polygon[j].Point1, i));
                }
            }

            for (int i = 0; i < triangles.Count; i++)
            {
                bool invalid = false;
                for (int j = 0; j < 3; j++)
                {
                    if (triangles[i][j] >= sourcePointCount)
                    {
                        invalid = true;
                        break;
                    }
                }

                if (invalid)
                {
                    triangles.RemoveAt(i);
                    i--;
                }
            }

            points.RemoveRange(points.Count - 3, 3);

            return triangles;
        }

        private static bool CircumCircleContains(List<Vector2> vertices, Triangle triangle, int vertex)
        {
            Vector2 p1 = vertices[triangle.Point0];
            Vector2 p2 = vertices[triangle.Point1];
            Vector2 p3 = vertices[triangle.Point2];

            float ab = p1.x * p1.x + p1.y * p1.y;
            float cd = p2.x * p2.x + p2.y * p2.y;
            float ef = p3.x * p3.x + p3.y * p3.y;

            Vector2 circum = new Vector2
            {
                x = (ab * (p3.y - p2.y) + cd * (p1.y - p3.y) + ef * (p2.y - p1.y)) /
                    (p1.x * (p3.y - p2.y) + p2.x * (p1.y - p3.y) + p3.x * (p2.y - p1.y)),
                y = (ab * (p3.x - p2.x) + cd * (p1.x - p3.x) + ef * (p2.x - p1.x)) /
                    (p1.y * (p3.x - p2.x) + p2.y * (p1.x - p3.x) + p3.y * (p2.x - p1.x))
            };
            circum *= 0.5f;

            float r = Vector2.SqrMagnitude(circum - p1);
            float d = Vector2.SqrMagnitude(circum - vertices[vertex]);

            return d <= r;
        }

        private static bool EdgeCompare(List<Vector2> vertices, Edge a, Edge b)
        {
            if ((vertices[a.Point0] == vertices[b.Point0]) &&
                (vertices[a.Point1] == vertices[b.Point1]))
            {
                return true;
            }

            if ((vertices[a.Point0] == vertices[b.Point1]) &&
                (vertices[a.Point1] == vertices[b.Point0]))
            {
                return true;
            }

            return false;
        }

        public struct Triangle
        {
            public int this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0: return Point0;
                        case 1: return Point1;
                        case 2: return Point2;
                        default: throw new ArgumentOutOfRangeException(nameof(index));
                    }
                }
            }

            public int Point0 { get; }

            public int Point1 { get; }

            public int Point2 { get; }

            public bool Bad { get; set; }

            public Triangle(int point0, int point1, int point2)
            {
                Point0 = point0;
                Point1 = point1;
                Point2 = point2;
                Bad = false;
            }

            public override string ToString()
            {
                return $"{(Bad ? "Bad" : "Good")}({Point0.ToString()}, {Point1.ToString()}, {Point2.ToString()})";
            }
        }

        private struct Edge
        {
            public int Point0 { get; }

            public int Point1 { get; }

            public bool Bad { get; set; }

            public Edge(int point0, int point1)
            {
                Point0 = point0;
                Point1 = point1;
                Bad = false;
            }
        }

        private struct Rect
        {
            public Vector2 Position { get; set; }

            public Vector2 Size { get; set; }

            public Vector2 Center => Position + (Size * 0.5f);

            public void ExpandTo(Vector2 vector)
            {
                Vector2 begin = Position;
                Vector2 end = Position + Size;

                if (vector.x < begin.x)
                {
                    begin.x = vector.x;
                }

                if (vector.y < begin.y)
                {
                    begin.y = vector.y;
                }

                if (vector.x > end.x)
                {
                    end.x = vector.x;
                }

                if (vector.y > end.y)
                {
                    end.y = vector.y;
                }

                Position = begin;
                Size = end - begin;
            }
        }
    }
}
