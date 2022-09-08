using System;
using UnityEngine;

namespace GBG.Puppeteer.Tests
{
    [DisallowMultipleComponent]
    public class BlendWeight1DTest : MonoBehaviour
    {
        public Transform position;

        public Transform[] points = Array.Empty<Transform>();

        public float[] vertices;

        public static void SortVertices(float[] vertices)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                var swapped = false;
                for (int j = 0; j < vertices.Length - 1 - i; j++)
                {
                    if (vertices[j] > vertices[j + 1])
                    {
                        (vertices[j], vertices[j + 1]) = (vertices[j + 1], vertices[j]);
                        swapped = true;
                    }
                }

                // Ordered data
                if (!swapped)
                {
                    return;
                }
            }
        }

        // See: GBG.Puppeteer.Tests.BlendWeight1DTestGizmos
    }
}
