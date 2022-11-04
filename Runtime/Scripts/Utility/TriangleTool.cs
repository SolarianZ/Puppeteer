using UnityEngine;
using UnityEngine.Assertions;

namespace GBG.AnimationGraph.Utility
{
    public static class TriangleTool
    {
        public static Vector3 CalculateWeights(Vector2 point, Vector2 vertex0, Vector2 vertex1,
            Vector2 vertex2, bool pointInsideTriangle)
        {
            Vector3 weights;

            if (pointInsideTriangle)
            {
                Assert.IsTrue(IsPointInTriangle(point, vertex0, vertex1, vertex2),
                    $"Point{point.ToString()} is not inside triangle({vertex0.ToString()},{vertex1.ToString()},{vertex2.ToString()}).");

                // Use area proportion of sub triangles
                weights.x = ((vertex1.y - vertex2.y) * (point.x - vertex2.x) +
                             (vertex2.x - vertex1.x) * (point.y - vertex2.y)) /
                            ((vertex1.y - vertex2.y) * (vertex0.x - vertex2.x) +
                             (vertex2.x - vertex1.x) * (vertex0.y - vertex2.y));
                weights.y = ((vertex2.y - vertex0.y) * (point.x - vertex2.x) +
                             (vertex0.x - vertex2.x) * (point.y - vertex2.y)) /
                            ((vertex1.y - vertex2.y) * (vertex0.x - vertex2.x) +
                             (vertex2.x - vertex1.x) * (vertex0.y - vertex2.y));
                weights.z = 1 - weights.x - weights.y;
            }
            else
            {
                var dist0 = Vector2.Distance(point, vertex0);
                var dist1 = Vector2.Distance(point, vertex1);
                var dist2 = Vector2.Distance(point, vertex2);
                var totalDist = dist0 + dist1 + dist2;
                if (totalDist < float.Epsilon)
                {
                    return new Vector3(1f / 3, 1f / 3, 1f / 3);
                }

                weights = new Vector3
                {
                    x = totalDist / (dist0 + float.Epsilon),
                    y = totalDist / (dist1 + float.Epsilon),
                    z = totalDist / (dist2 + float.Epsilon),
                };

                // Normalization
                var totalWeight = weights.x + weights.y + weights.z;
                weights.x /= totalWeight;
                weights.y /= totalWeight;
                weights.z /= totalWeight;
            }

            return weights;
        }

        public static bool IsPointInTriangle(Vector2 point, Vector2 vertex0, Vector2 vertex1, Vector2 vertex2)
        {
            // ReSharper disable InconsistentNaming

            if (point == vertex0 || point == vertex1 || point == vertex2)
            {
                // Point overlaps with one vertex 
                return true;
            }

            var pa = vertex0 - point;
            var pb = vertex1 - point;
            var pc = vertex2 - point;

            // Z value of Vector2 cross product
            var zPAxPB = pa.x * pb.y - pa.y * pb.x;
            var zPBxPC = pb.x * pc.y - pb.y * pc.x;
            var zPCxPA = pc.x * pa.y - pc.y * pa.x;

            // Sign of z value
            var signZ_PAxPB = Sign(zPAxPB);
            var signZ_PBxPC = Sign(zPBxPC);
            var signZ_PCxPA = Sign(zPCxPA);
            var signSum = signZ_PAxPB + signZ_PBxPC + signZ_PCxPA;

            // Point inside triangle:
            //     -1 + -1 + -1 = -3 (or 1 + 1 + 1 = 3 for right-handed coordinate system)
            // Point on edge:
            //     -1 + -1 + 0 = -2 (or 1 + 1 + 0 = 2 for right-handed coordinate system)
            return (signSum < -1) || (signSum > 1);

            static sbyte Sign(float value)
            {
                if (value > 0) return 1;
                if (value < 0) return -1;
                return 0;
            }
        }

        public static Vector2 GetTriangleCentroid(Vector2 vertex0, Vector2 vertex1, Vector2 vertex2)
        {
            return new Vector2(vertex0.x + vertex1.x + vertex2.x, vertex0.y + vertex1.y + vertex2.y) / 3;
        }
    }
}
