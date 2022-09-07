using UnityEngine;
using UnityEngine.Assertions;

namespace GBG.Puppeteer
{
    public static class TriangleTool
    {
        public static Vector3 CalculateWeightsInsideTriangle(Vector2 point, Vector2 vert0, Vector2 vert1, Vector2 vert2)
        {
            Assert.IsTrue(IsPointInTriangle(point, vert0, vert1, vert2));

            // Use area proportion of sub triangles
            var weights = new Vector3();
            weights.x = ((vert1.y - vert2.y) * (point.x - vert2.x) + (vert2.x - vert1.x) * (point.y - vert2.y)) /
                        ((vert1.y - vert2.y) * (vert0.x - vert2.x) + (vert2.x - vert1.x) * (vert0.y - vert2.y));
            weights.y = ((vert2.y - vert0.y) * (point.x - vert2.x) + (vert0.x - vert2.x) * (point.y - vert2.y)) /
                        ((vert1.y - vert2.y) * (vert0.x - vert2.x) + (vert2.x - vert1.x) * (vert0.y - vert2.y));
            weights.z = 1 - weights.x - weights.y;

            return weights;
        }

        public static Vector3 CalculateWeights(Vector2 point, Vector2 vertex0, Vector2 vertex1, Vector2 vertex2)
        {
            var dist0 = Vector2.Distance(point, vertex0);
            var dist1 = Vector2.Distance(point, vertex1);
            var dist2 = Vector2.Distance(point, vertex2);
            var totalDist = dist0 + dist1 + dist2;
            if (totalDist < float.Epsilon)
            {
                return new Vector3(1f / 3, 1f / 3, 1f / 3);
            }

            var weights = new Vector3
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

            return weights;
        }

        public static bool IsPointInTriangle(Vector2 point, Vector2 vertexA, Vector2 vertexB, Vector2 vertexC)
        {
            // ReSharper disable InconsistentNaming

            var pa = vertexA - point;
            var pb = vertexB - point;
            var pc = vertexC - point;

            // Z value of Vector2 cross product
            var zPAxPB = pa.x * pb.y - pa.y * pb.x;
            var zPBxPC = pb.x * pc.y - pb.y * pc.x;
            var zPCxPA = pc.x * pa.y - pc.y * pa.x;

            // Sign of z value
            var signZ_PAxPB = Sign(zPAxPB);
            var signZ_PBxPC = Sign(zPBxPC);
            var signZ_PCxPA = Sign(zPCxPA);
            var signSum = signZ_PAxPB + signZ_PBxPC + signZ_PCxPA;

            // When point inside a triangle, all signs of z value must be the same,
            // the sum is (-1 + -1 + -1) or (1 + 1 + 1) or (1 + 0 + 1) or (-1 + 0 + -1).
            return (signSum < -1) || (signSum > 1);

            static sbyte Sign(float value)
            {
                if (value > 0) return 1;
                if (value < 0) return -1;
                return 0;
            }
        }

        public static bool IsPointInTriangle_CentroidMethod(Vector2 point, Vector2 vertexA, Vector2 vertexB,
            Vector2 vertexC)
        {
            var ap = point - vertexA;
            var ab = vertexB - vertexA;
            var ac = vertexC - vertexA;

            var u = (Dot(ab, ab) * Dot(ap, ac) - Dot(ac, ab) * Dot(ap, ab)) /
                    (Dot(ac, ac) * Dot(ab, ab) - Dot(ac, ab) * Dot(ab, ac));

            if (u < 0 || u > 1)
            {
                return false;
            }

            var v = (Dot(ac, ac) * Dot(ap, ab) - Dot(ac, ab) * Dot(ap, ac)) /
                    (Dot(ac, ac) * Dot(ab, ab) - Dot(ac, ab) * Dot(ab, ac));

            return (v >= 0 && v <= 1) && (u + v <= 1);

            static float Dot(Vector2 lhs, Vector2 rhs) => lhs.x * rhs.x + lhs.y * rhs.y;
        }
    }
}
