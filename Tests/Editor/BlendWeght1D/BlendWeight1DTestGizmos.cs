using UnityEditor;
using UnityEngine.Assertions;

namespace GBG.Puppeteer.Tests.Editor
{
    public class BlendWeight1DTestGizmos
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        public static void DrawGizmos(BlendWeight1DTest target, GizmoType gizmoType)
        {
            if (target.points.Length < 2 || !target.position)
            {
                return;
            }

            target.vertices = new float[target.points.Length];
            for (int i = 0; i < target.vertices.Length; i++)
            {
                target.vertices[i] = target.points[i] ? target.points[i].position.x : 0;
            }

            // MAKE SURE to use the same algorithm with BlendSpace1DInstance.cs

            BlendWeight1DTest.SortVertices(target.vertices);

            var position = target.position.position.x;
            var leftIndex = new int?();
            for (int i = 0; i < target.vertices.Length; i++)
            {
                if (leftIndex == null)
                {
                    // The left most motion
                    if (position < target.vertices[i])
                    {
                        Assert.AreEqual(i, 0);
                        Handles.Label(target.points[i].position, "1");

                        leftIndex = -1;
                        continue;
                    }

                    if (i < target.vertices.Length - 1)
                    {
                        // Not in the interval
                        if (position > target.vertices[i + 1])
                        {
                            Handles.Label(target.points[i].position, "0");
                            continue;
                        }

                        // In the interval
                        var rightWeight = (position - target.vertices[i]) / (target.vertices[i + 1] - target.vertices[i]);
                        var leftWeight = 1 - rightWeight;
                        Handles.Label(target.points[i].position, leftWeight.ToString("F3"));
                        Handles.Label(target.points[i + 1].position, rightWeight.ToString("F3"));
                        leftIndex = i;
                        continue;
                    }

                    // The most right motion
                    Assert.AreEqual(i, target.vertices.Length - 1);
                    Handles.Label(target.points[i].position, "1");
                    leftIndex = i;
                }
                else if (leftIndex.Value + 1 != i)
                {
                    // Not in the interval
                    Handles.Label(target.points[i].position, "0");
                }
            }
        }
    }
}
