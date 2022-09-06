// https://github.com/SolarianZ/delaunator-sharp

// ReSharper disable InconsistentNaming
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DelaunatorSharp
{
    public readonly struct Triangle
    {
        public readonly Vector2 Point0;

        public readonly Vector2 Point1;

        public readonly Vector2 Point2;


        public Triangle(Vector2 point0, Vector2 point1, Vector2 point2)
        {
            Point0 = point0;
            Point1 = point1;
            Point2 = point2;
        }
    }

    public class Delaunator
    {
        /// <summary>
        /// One value per half-edge, containing the point index of where a given half edge starts.
        /// </summary>
        public int[] Triangles { get; }

        /// <summary>
        /// The initial points Delaunator was constructed with.
        /// </summary>
        public Vector2[] Points { get; }

        private int trianglesLen;


        public Delaunator(IEnumerable<Vector2> points) : this(points.ToArray())
        {
        }

        public Delaunator(Vector2[] points)
        {
            if (points.Length < 3)
            {
                throw new ArgumentException("Need at least 3 points.", nameof(points));
            }

            var edgeStack = new int[512];

            Points = points;
            var coords = new float[Points.Length * 2];

            for (var i = 0; i < Points.Length; i++)
            {
                var p = Points[i];
                coords[2 * i] = p.x;
                coords[2 * i + 1] = p.y;
            }

            var n = points.Length;
            var maxTriangles = 2 * n - 5;

            Triangles = new int[maxTriangles * 3];

            // One value per half-edge, containing the opposite half-edge in the adjacent triangle,
            // or -1 if there is no adjacent triangle
            var halfEdges = new int[maxTriangles * 3];
            var hashSize = (int)Math.Ceiling(Math.Sqrt(n));

            var hullPrev = new int[n];
            var hullNext = new int[n];
            var hullTri = new int[n];
            var hullHash = new int[hashSize];

            var ids = new int[n];

            var minX = float.PositiveInfinity;
            var minY = float.PositiveInfinity;
            var maxX = float.NegativeInfinity;
            var maxY = float.NegativeInfinity;

            for (var i = 0; i < n; i++)
            {
                var x = coords[2 * i];
                var y = coords[2 * i + 1];
                if (x < minX) minX = x;
                if (y < minY) minY = y;
                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
                ids[i] = i;
            }

            var cx = (minX + maxX) / 2;
            var cy = (minY + maxY) / 2;

            var minDist = float.PositiveInfinity;
            var i0 = 0;
            var i1 = 0;
            var i2 = 0;

            // pick a seed point close to the center
            for (int i = 0; i < n; i++)
            {
                var d = SqrDist(cx, cy, coords[2 * i], coords[2 * i + 1]);
                if (d < minDist)
                {
                    i0 = i;
                    minDist = d;
                }
            }

            var i0x = coords[2 * i0];
            var i0y = coords[2 * i0 + 1];

            minDist = float.PositiveInfinity;

            // find the point closest to the seed
            for (int i = 0; i < n; i++)
            {
                if (i == i0) continue;
                var d = SqrDist(i0x, i0y, coords[2 * i], coords[2 * i + 1]);
                if (d < minDist && d > 0)
                {
                    i1 = i;
                    minDist = d;
                }
            }

            var i1x = coords[2 * i1];
            var i1y = coords[2 * i1 + 1];

            var minRadius = float.PositiveInfinity;

            // find the third point which forms the smallest circumcircle with the first two
            for (int i = 0; i < n; i++)
            {
                if (i == i0 || i == i1) continue;
                var r = SqrCircumradius(i0x, i0y, i1x, i1y, coords[2 * i], coords[2 * i + 1]);
                if (r < minRadius)
                {
                    i2 = i;
                    minRadius = r;
                }
            }

            var i2x = coords[2 * i2];
            var i2y = coords[2 * i2 + 1];

            if (float.IsPositiveInfinity(minRadius))
            {
                throw new Exception("No Delaunay triangulation exists for this input.");
            }

            if (Orient(i0x, i0y, i1x, i1y, i2x, i2y))
            {
                var i = i1;
                var x = i1x;
                var y = i1y;
                i1 = i2;
                i1x = i2x;
                i1y = i2y;
                i2 = i;
                i2x = x;
                i2y = y;
            }

            var circumcenter = Circumcenter(i0x, i0y, i1x, i1y, i2x, i2y);

            var dists = new float[n];
            for (var i = 0; i < n; i++)
            {
                dists[i] = SqrDist(coords[2 * i], coords[2 * i + 1], circumcenter.x, circumcenter.y);
            }

            // sort the points by distance from the seed triangle circumcenter
            Quicksort(ids, dists, 0, n - 1);

            // set up the seed triangle as the starting hull
            var hullStart = i0;

            hullNext[i0] = hullPrev[i2] = i1;
            hullNext[i1] = hullPrev[i0] = i2;
            hullNext[i2] = hullPrev[i1] = i0;

            hullTri[i0] = 0;
            hullTri[i1] = 1;
            hullTri[i2] = 2;

            hullHash[HashKey(i0x, i0y, hashSize, circumcenter)] = i0;
            hullHash[HashKey(i1x, i1y, hashSize, circumcenter)] = i1;
            hullHash[HashKey(i2x, i2y, hashSize, circumcenter)] = i2;

            trianglesLen = 0;
            AddTriangle(i0, i1, i2, -1, -1, -1, halfEdges);

            var xp = 0f;
            var yp = 0f;

            for (var k = 0; k < ids.Length; k++)
            {
                var i = ids[k];
                var x = coords[2 * i];
                var y = coords[2 * i + 1];

                // skip near-duplicate points
                if (k > 0 && Math.Abs(x - xp) <= float.Epsilon && Math.Abs(y - yp) <= float.Epsilon) continue;
                xp = x;
                yp = y;

                // skip seed triangle points
                if (i == i0 || i == i1 || i == i2) continue;

                // find a visible edge on the convex hull using edge hash
                var start = 0;
                for (var j = 0; j < hashSize; j++)
                {
                    var key = HashKey(x, y, hashSize, circumcenter);
                    start = hullHash[(key + j) % hashSize];
                    if (start != -1 && start != hullNext[start]) break;
                }


                start = hullPrev[start];
                var e = start;
                var q = hullNext[e];

                while (!Orient(x, y, coords[2 * e], coords[2 * e + 1], coords[2 * q], coords[2 * q + 1]))
                {
                    e = q;
                    if (e == start)
                    {
                        e = int.MaxValue;
                        break;
                    }

                    q = hullNext[e];
                }

                if (e == int.MaxValue) continue; // likely a near-duplicate point; skip it

                // add the first triangle from the point
                var t = AddTriangle(e, i, hullNext[e], -1, -1, hullTri[e], halfEdges);

                // recursively flip triangles from the point until they satisfy the Delaunay condition
                hullTri[i] = Legalize(t + 2, edgeStack, halfEdges, coords, hullPrev, hullTri, hullStart);
                hullTri[e] = t; // keep track of boundary triangles on the hull

                // walk forward through the hull, adding more triangles and flipping recursively
                var next = hullNext[e];
                q = hullNext[next];

                while (Orient(x, y, coords[2 * next], coords[2 * next + 1], coords[2 * q], coords[2 * q + 1]))
                {
                    t = AddTriangle(next, i, q, hullTri[i], -1, hullTri[next], halfEdges);
                    hullTri[i] = Legalize(t + 2, edgeStack, halfEdges, coords, hullPrev, hullTri, hullStart);
                    hullNext[next] = next; // mark as removed
                    next = q;

                    q = hullNext[next];
                }

                // walk backward from the other side, adding more triangles and flipping
                if (e == start)
                {
                    q = hullPrev[e];

                    while (Orient(x, y, coords[2 * q], coords[2 * q + 1], coords[2 * e], coords[2 * e + 1]))
                    {
                        t = AddTriangle(q, i, e, -1, hullTri[e], hullTri[q], halfEdges);
                        Legalize(t + 2, edgeStack, halfEdges, coords, hullPrev, hullTri, hullStart);
                        hullTri[q] = t;
                        hullNext[e] = e; // mark as removed
                        e = q;

                        q = hullPrev[e];
                    }
                }

                // update the hull indices
                hullStart = hullPrev[i] = e;
                hullNext[e] = hullPrev[next] = i;
                hullNext[i] = next;

                // save the two new edges in the hash table
                hullHash[HashKey(x, y, hashSize, circumcenter)] = i;
                hullHash[HashKey(coords[2 * e], coords[2 * e + 1], hashSize, circumcenter)] = e;
            }

            hullPrev = hullNext = hullTri = null; // get rid of temporary arrays

            //// trim typed triangle mesh arrays
            Triangles = Triangles.Take(trianglesLen).ToArray();
            halfEdges = halfEdges.Take(trianglesLen).ToArray();
        }


        #region CreationLogic

        private int Legalize(int a, int[] edgeStack, int[] halfEdges, float[] coords,
            int[] hullPrev, int[] hullTri, int hullStart)
        {
            var i = 0;
            int ar;

            // recursion eliminated with a fixed-size stack
            while (true)
            {
                var b = halfEdges[a];

                /* if the pair of triangles doesn't satisfy the Delaunay condition
                 * (p1 is inside the circumcircle of [p0, pl, pr]), flip them,
                 * then do the same check/flip recursively for the new pair of triangles
                 *
                 *           pl                    pl
                 *          /||\                  /  \
                 *       al/ || \bl            al/    \a
                 *        /  ||  \              /      \
                 *       /  a||b  \    flip    /___ar___\
                 *     p0\   ||   /p1   =>   p0\---bl---/p1
                 *        \  ||  /              \      /
                 *       ar\ || /br             b\    /br
                 *          \||/                  \  /
                 *           pr                    pr
                 */
                int a0 = a - a % 3;
                ar = a0 + (a + 2) % 3;

                if (b == -1)
                {
                    // convex hull edge
                    if (i == 0) break;
                    a = edgeStack[--i];
                    continue;
                }

                var b0 = b - b % 3;
                var al = a0 + (a + 1) % 3;
                var bl = b0 + (b + 2) % 3;

                var p0 = Triangles[ar];
                var pr = Triangles[a];
                var pl = Triangles[al];
                var p1 = Triangles[bl];

                var illegal = InCircle(
                    coords[2 * p0], coords[2 * p0 + 1],
                    coords[2 * pr], coords[2 * pr + 1],
                    coords[2 * pl], coords[2 * pl + 1],
                    coords[2 * p1], coords[2 * p1 + 1]);

                if (illegal)
                {
                    Triangles[a] = p1;
                    Triangles[b] = p0;

                    var hbl = halfEdges[bl];

                    // edge swapped on the other side of the hull (rare); fix the halfedge reference
                    if (hbl == -1)
                    {
                        var e = hullStart;
                        do
                        {
                            if (hullTri[e] == bl)
                            {
                                hullTri[e] = a;
                                break;
                            }

                            e = hullPrev[e];
                        } while (e != hullStart);
                    }

                    Link(a, hbl, halfEdges);
                    Link(b, halfEdges[ar], halfEdges);
                    Link(ar, bl, halfEdges);

                    var br = b0 + (b + 1) % 3;

                    // don't worry about hitting the cap: it can only happen on extremely degenerate input
                    if (i < edgeStack.Length)
                    {
                        edgeStack[i++] = br;
                    }
                }
                else
                {
                    if (i == 0) break;
                    a = edgeStack[--i];
                }
            }

            return ar;
        }

        private static bool InCircle(float ax, float ay, float bx, float by, float cx, float cy, float px, float py)
        {
            var dx = ax - px;
            var dy = ay - py;
            var ex = bx - px;
            var ey = by - py;
            var fx = cx - px;
            var fy = cy - py;

            var ap = dx * dx + dy * dy;
            var bp = ex * ex + ey * ey;
            var cp = fx * fx + fy * fy;

            return dx * (ey * cp - bp * fy) -
                dy * (ex * cp - bp * fx) +
                ap * (ex * fy - ey * fx) < 0;
        }

        private int AddTriangle(int i0, int i1, int i2, int a, int b, int c, int[] halfEdges)
        {
            var t = trianglesLen;

            Triangles[t] = i0;
            Triangles[t + 1] = i1;
            Triangles[t + 2] = i2;

            Link(t, a, halfEdges);
            Link(t + 1, b, halfEdges);
            Link(t + 2, c, halfEdges);

            trianglesLen += 3;
            return t;
        }

        private void Link(int a, int b, int[] halfEdges)
        {
            halfEdges[a] = b;
            if (b != -1) halfEdges[b] = a;
        }

        private int HashKey(float x, float y, int hashSize, Vector2 circumcenter)
        {
            return (int)(Math.Floor(PseudoAngle(x - circumcenter.x, y - circumcenter.y) * hashSize) % hashSize);
        }

        private static float PseudoAngle(float dx, float dy)
        {
            var p = dx / (Math.Abs(dx) + Math.Abs(dy));
            return (dy > 0 ? 3 - p : 1 + p) / 4; // [0..1]
        }

        private static void Quicksort(int[] ids, float[] dists, int left, int right)
        {
            if (right - left <= 20)
            {
                for (var i = left + 1; i <= right; i++)
                {
                    var temp = ids[i];
                    var tempDist = dists[temp];
                    var j = i - 1;
                    while (j >= left && dists[ids[j]] > tempDist) ids[j + 1] = ids[j--];
                    ids[j + 1] = temp;
                }
            }
            else
            {
                var median = (left + right) >> 1;
                var i = left + 1;
                var j = right;
                Swap(ids, median, i);
                if (dists[ids[left]] > dists[ids[right]]) Swap(ids, left, right);
                if (dists[ids[i]] > dists[ids[right]]) Swap(ids, i, right);
                if (dists[ids[left]] > dists[ids[i]]) Swap(ids, left, i);

                var temp = ids[i];
                var tempDist = dists[temp];
                while (true)
                {
                    do i++;
                    while (dists[ids[i]] < tempDist);
                    do j--;
                    while (dists[ids[j]] > tempDist);
                    if (j < i) break;
                    Swap(ids, i, j);
                }

                ids[left + 1] = ids[j];
                ids[j] = temp;

                if (right - i + 1 >= j - left)
                {
                    Quicksort(ids, dists, i, right);
                    Quicksort(ids, dists, left, j - 1);
                }
                else
                {
                    Quicksort(ids, dists, left, j - 1);
                    Quicksort(ids, dists, i, right);
                }
            }
        }

        private static void Swap(int[] arr, int i, int j)
        {
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }


        public static bool Orient(float px, float py, float qx, float qy, float rx, float ry) =>
            (qy - py) * (rx - qx) - (qx - px) * (ry - qy) < 0;

        public static float SqrCircumradius(float ax, float ay, float bx, float by, float cx, float cy)
        {
            var dx = bx - ax;
            var dy = by - ay;
            var ex = cx - ax;
            var ey = cy - ay;
            var bl = dx * dx + dy * dy;
            var cl = ex * ex + ey * ey;
            var d = 0.5f / (dx * ey - dy * ex);
            var x = (ey * bl - dy * cl) * d;
            var y = (dx * cl - ex * bl) * d;
            return x * x + y * y;
        }

        public static Vector2 Circumcenter(float ax, float ay, float bx, float by, float cx, float cy)
        {
            var dx = bx - ax;
            var dy = by - ay;
            var ex = cx - ax;
            var ey = cy - ay;
            var bl = dx * dx + dy * dy;
            var cl = ex * ex + ey * ey;
            var d = 0.5f / (dx * ey - dy * ex);
            var x = ax + (ey * bl - dy * cl) * d;
            var y = ay + (dx * cl - ex * bl) * d;

            return new Vector2(x, y);
        }

        public static float SqrDist(float ax, float ay, float bx, float by)
        {
            var dx = ax - bx;
            var dy = ay - by;
            return dx * dx + dy * dy;
        }

        #endregion CreationLogic


        public Triangle GetTriangle(int triangleIndex)
        {
            var point0 = Points[Triangles[3 * triangleIndex]];
            var point1 = Points[Triangles[3 * triangleIndex + 1]];
            var point2 = Points[Triangles[3 * triangleIndex + 2]];
            return new Triangle(point0, point1, point2);
        }

        public void GetTrianglePoints(int triangleIndex, out int pointIndex0, out int pointIndex1, out int pointIndex2)
        {
            pointIndex0 = Triangles[3 * triangleIndex];
            pointIndex1 = Triangles[3 * triangleIndex + 1];
            pointIndex2 = Triangles[3 * triangleIndex + 2];
        }

        public Vector2 GetTriangleCentroid(int triangleIndex)
        {
            var triangle = GetTriangle(triangleIndex);
            return GetCentroid(triangle.Point0, triangle.Point1, triangle.Point2);
        }

        private static Vector2 GetCentroid(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            return new Vector2(p0.x + p1.x + p2.x, p0.y + p1.y + p2.y) / 3;
        }
    }
}
