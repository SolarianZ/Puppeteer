using System.Collections.Generic;
using GBG.AnimationGraph.ThirdParties;
using GBG.AnimationGraph.Utility;
using UnityEditor;
using UnityEngine;

namespace GBG.Puppeteer.Tests.Editor
{
    public class BlendWeight2DTestGizmos
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        public static void DrawGizmos(BlendWeight2DTest target, GizmoType gizmoType)
        {
            if (target.points.Length < 3 || !target.position)
            {
                return;
            }

            var vertices = new List<Vector2>(target.points.Length);
            foreach (var point in target.points)
            {
                var pos = point ? point.position : Vector3.zero;
                vertices.Add(new Vector2(pos.x, pos.y));
            }

            var triangles = Delaunay2D.Triangulate(vertices);
            if (triangles.Count == 0)
            {
                // Collinear
                Debug.LogError("Points Collinear, No Delaunay Triangles!");
                return;
            }

            var containerTriangleIndex = -1;
            var closestTriangleIndex = -1;
            var closestTriangleSqrDist = float.MaxValue;
            for (int i = 0; i < triangles.Count; i++)
            {
                var triangle = triangles[i];

                Gizmos.color = Color.gray;
                Gizmos.DrawLine(vertices[triangle.Point0], vertices[triangle.Point1]);
                Gizmos.DrawLine(vertices[triangle.Point1], vertices[triangle.Point2]);
                Gizmos.DrawLine(vertices[triangle.Point2], vertices[triangle.Point0]);

                if (TriangleTool.IsPointInTriangle(target.position.position,
                        vertices[triangle.Point0],
                        vertices[triangle.Point1],
                        vertices[triangle.Point2]))
                {
                    containerTriangleIndex = i;
                    // break; // Still need to draw edges, so don't break
                }

                var triangleCentroid = TriangleTool.GetTriangleCentroid(
                    vertices[triangle.Point0],
                    vertices[triangle.Point1],
                    vertices[triangle.Point2]);
                var sqrDist = Vector2.SqrMagnitude(triangleCentroid - (Vector2)target.position.position);
                if (sqrDist < closestTriangleSqrDist)
                {
                    closestTriangleIndex = i;
                    closestTriangleSqrDist = sqrDist;
                }
            }

            Vector3 vertexWeights;
            // Vector3Int indexes;
            if (containerTriangleIndex > -1)
            {
                var triangle = triangles[containerTriangleIndex];
                vertexWeights = TriangleTool.CalculateWeights(target.position.position,
                    vertices[triangle.Point0],
                    vertices[triangle.Point1],
                    vertices[triangle.Point2],
                    true);
                // indexes = new Vector3Int(x, y, z);

                Handles.Label(vertices[triangle.Point0] + Vector2.up * 0.015f, vertexWeights.x.ToString("F3"));
                Handles.Label(vertices[triangle.Point1] + Vector2.up * 0.015f, vertexWeights.y.ToString("F3"));
                Handles.Label(vertices[triangle.Point2] + Vector2.up * 0.015f, vertexWeights.z.ToString("F3"));
                Handles.Label(vertices[triangle.Point0], containerTriangleIndex.ToString());
                Handles.Label(vertices[triangle.Point1], containerTriangleIndex.ToString());
                Handles.Label(vertices[triangle.Point2], containerTriangleIndex.ToString());
            }
            else
            {
                var triangle = triangles[closestTriangleIndex];
                vertexWeights = TriangleTool.CalculateWeights(target.position.position,
                    vertices[triangle.Point0],
                    vertices[triangle.Point1],
                    vertices[triangle.Point2],
                    false);
                // indexes = new Vector3Int(x, y, z);

                var triangleCentroid = TriangleTool.GetTriangleCentroid(
                    vertices[triangle.Point0],
                    vertices[triangle.Point1],
                    vertices[triangle.Point2]);
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(triangleCentroid, 0.0125f);

                Handles.Label(vertices[triangle.Point0] + Vector2.up * 0.015f, vertexWeights.x.ToString("F3"));
                Handles.Label(vertices[triangle.Point1] + Vector2.up * 0.015f, vertexWeights.y.ToString("F3"));
                Handles.Label(vertices[triangle.Point2] + Vector2.up * 0.015f, vertexWeights.z.ToString("F3"));
                Handles.Label(vertices[triangle.Point0], closestTriangleIndex.ToString());
                Handles.Label(vertices[triangle.Point1], closestTriangleIndex.ToString());
                Handles.Label(vertices[triangle.Point2], closestTriangleIndex.ToString());
            }
        }
    }
}