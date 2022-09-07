using System;
using DelaunatorSharp;
using GBG.Puppeteer;
using UnityEditor;
using UnityEngine;

public class DelaunatorTest : MonoBehaviour
{
    public Transform point;

    public Transform[] vertices = Array.Empty<Transform>();

    private void OnDrawGizmos()
    {
        if (vertices.Length < 3 || !point)
        {
            return;
        }

        var points = new Vector2[vertices.Length];
        for (int i = 0; i < points.Length; i++)
        {
            var p = vertices[i] ? vertices[i].position : Vector3.zero;
            points[i] = new Vector2(p.x, p.y);
        }

        var delaunator = new Delaunator(points);

        var containerTriangleIndex = -1;
        var closestTriangleIndex = -1;
        var closestTriangleSqrDist = float.MaxValue;
        for (int i = 0; i < delaunator.Triangles.Length / 3; i++)
        {
            var triangle = delaunator.GetTriangle(i);

            Gizmos.color = Color.gray;
            Gizmos.DrawLine(triangle.Point0, triangle.Point1);
            Gizmos.DrawLine(triangle.Point1, triangle.Point2);
            Gizmos.DrawLine(triangle.Point2, triangle.Point0);

            if (TriangleTool.IsPointInTriangle(point.position, triangle.Point0, triangle.Point1, triangle.Point2))
            {
                containerTriangleIndex = i;
                // break; // Still need draw edges, so don't break
            }

            var triangleCentroid = delaunator.GetTriangleCentroid(i);
            var sqrDist = Vector2.SqrMagnitude(triangleCentroid - (Vector2)point.position);
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
            var triangle = delaunator.GetTriangle(containerTriangleIndex);
            vertexWeights = TriangleTool.CalculateWeightsInsideTriangle(point.position, triangle.Point0,
                triangle.Point1, triangle.Point2);
            delaunator.GetTrianglePoints(containerTriangleIndex, out var x, out var y, out var z);
            // indexes = new Vector3Int(x, y, z);

            Handles.Label(points[x] + Vector2.up * 0.015f, vertexWeights.x.ToString("F3"));
            Handles.Label(points[y] + Vector2.up * 0.015f, vertexWeights.y.ToString("F3"));
            Handles.Label(points[z] + Vector2.up * 0.015f, vertexWeights.z.ToString("F3"));
            Handles.Label(points[x], containerTriangleIndex.ToString());
            Handles.Label(points[y], containerTriangleIndex.ToString());
            Handles.Label(points[z], containerTriangleIndex.ToString());

            var center = Delaunator.Circumcenter(points[x].x, points[x].y,
                points[y].x, points[y].y, points[z].x, points[z].y);
            var radius = Delaunator.SqrCircumradius(points[x].x, points[x].y,
                points[y].x, points[y].y, points[z].x, points[z].y);
            radius = Mathf.Sqrt(radius);
            Handles.color = Color.red;
            Handles.DrawWireDisc(center, Vector3.back, radius);
        }
        else
        {
            var triangle = delaunator.GetTriangle(closestTriangleIndex);
            vertexWeights = TriangleTool.CalculateWeights(point.position, triangle.Point0,
                triangle.Point1, triangle.Point2);
            delaunator.GetTrianglePoints(closestTriangleIndex, out var x, out var y, out var z);
            // indexes = new Vector3Int(x, y, z);

            var triangleCentroid = delaunator.GetTriangleCentroid(closestTriangleIndex);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(triangleCentroid, 0.0125f);

            Handles.Label(points[x] + Vector2.up * 0.015f, vertexWeights.x.ToString("F3"));
            Handles.Label(points[y] + Vector2.up * 0.015f, vertexWeights.y.ToString("F3"));
            Handles.Label(points[z] + Vector2.up * 0.015f, vertexWeights.z.ToString("F3"));
            Handles.Label(points[x], closestTriangleIndex.ToString());
            Handles.Label(points[y], closestTriangleIndex.ToString());
            Handles.Label(points[z], closestTriangleIndex.ToString());
        }
    }
}
