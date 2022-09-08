using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeInstance
{
    [Serializable]
    public class MotionField2D
    {
        [SerializeField]
        public AnimationClip Clip;

        [SerializeField]
        public Vector2 Position;

        [SerializeField]
        public float PlaybackSpeed = 1.0f;

        // [SerializeField]
        // public bool Mirror; // Not yet supported
    }

    public class BlendSpace2DInstance
    {
        public AnimationMixerPlayable Mixer { get; }

        public Vector2 Position { get; private set; }


        private readonly MotionField2D[] _motionFields;

        private readonly int[] _triangles;


        public BlendSpace2DInstance(PlayableGraph graph, MotionField2D[] motionFields, int[] triangles,
            Vector2 position)
        {
            _motionFields = motionFields;
            _triangles = triangles;

            Mixer = AnimationMixerPlayable.Create(graph, _motionFields.Length);
            for (int i = 0; i < _motionFields.Length; i++)
            {
                var clipPlayable = AnimationClipPlayable.Create(graph, _motionFields[i].Clip);
                Mixer.ConnectInput(i, clipPlayable, 0);
            }

            SetPosition(position);
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;

            if (_motionFields.Length == 0)
            {
                return;
            }

            if (_motionFields.Length == 1)
            {
                Mixer.SetInputWeight(0, 1);
                return;
            }

            // Approximate collinear
            if (_triangles == null)
            {
                var indexA = 0;
                var indexB = 0;
                var distA = float.MaxValue;
                var distB = float.MaxValue;

                for (int i = 0; i < _motionFields.Length; i++)
                {
                    var motion = _motionFields[i];
                    var dist = Vector2.SqrMagnitude(position - motion.Position);
                    if (dist < distA)
                    {
                        distA = dist;
                        indexA = i;
                    }
                    else if (dist < distB)
                    {
                        distB = dist;
                        indexB = i;
                    }
                }

                var weights = CalculateWeights(position, _motionFields[indexA].Position,
                    _motionFields[indexB].Position);
                Mixer.SetInputWeight(indexA, weights.x);
                Mixer.SetInputWeight(indexB, weights.y);
                return;
            }

            // Find container triangle or closest triangle
            var containerTriangleIndex = -1;
            var closestTriangleIndex = -1;
            var closestTriangleSqrDist = float.MaxValue;
            for (int i = 0; i < _triangles.Length / 3; i++)
            {
                GetTriangleVertices(i, out var vert0, out var vert1, out var vert2);
                if (TriangleTool.IsPointInTriangle(position, vert0, vert1, vert2))
                {
                    containerTriangleIndex = i;
                    break;
                }

                var triangleCentroid = GetCentroid(vert0, vert1, vert2);
                var sqrDist = Vector2.SqrMagnitude(triangleCentroid - position);
                if (sqrDist < closestTriangleSqrDist)
                {
                    closestTriangleIndex = i;
                    closestTriangleSqrDist = sqrDist;
                }
            }

            Vector3 vertexWeights;
            Vector3Int indices;
            if (containerTriangleIndex > -1)
            {
                GetTriangleVertices(containerTriangleIndex, out var vert0, out var vert1, out var vert2);
                vertexWeights = TriangleTool.CalculateWeights(position, vert0, vert1, vert2, true);
                GetTriangleVertexIndices(containerTriangleIndex, out var index0, out var index1, out var index2);
                indices = new Vector3Int(index0, index1, index2);
            }
            else
            {
                GetTriangleVertices(closestTriangleIndex, out var vert0, out var vert1, out var vert2);
                vertexWeights = TriangleTool.CalculateWeights(position, vert0, vert1, vert2, false);
                GetTriangleVertexIndices(closestTriangleIndex, out var index0, out var index1, out var index2);
                indices = new Vector3Int(index0, index1, index2);
            }

            Mixer.SetInputWeight(indices.x, vertexWeights.x);
            Mixer.SetInputWeight(indices.y, vertexWeights.y);
            Mixer.SetInputWeight(indices.z, vertexWeights.z);
        }

        public void SetXPosition(float x)
        {
            SetPosition(new Vector2(x, Position.y));
        }

        public void SetYPosition(float y)
        {
            SetPosition(new Vector2(Position.x, y));
        }


        private void GetTriangleVertices(int triangleIndex, out Vector2 point0, out Vector2 point1, out Vector2 point2)
        {
            point0 = _motionFields[_triangles[3 * triangleIndex]].Position;
            point1 = _motionFields[_triangles[3 * triangleIndex + 1]].Position;
            point2 = _motionFields[_triangles[3 * triangleIndex + 2]].Position;
        }

        private void GetTriangleVertexIndices(int triangleIndex, out int pointIndex0, out int pointIndex1,
            out int pointIndex2)
        {
            pointIndex0 = _triangles[3 * triangleIndex];
            pointIndex1 = _triangles[3 * triangleIndex + 1];
            pointIndex2 = _triangles[3 * triangleIndex + 2];
        }

        private static Vector2 GetCentroid(Vector2 point0, Vector2 point1, Vector2 point2)
        {
            return new Vector2(point0.x + point1.x + point2.x, point0.y + point1.y + point2.y) / 3;
        }

        private static Vector2 CalculateWeights(Vector2 position, Vector2 point0, Vector2 point1)
        {
            var dist0 = Vector2.Distance(position, point0);
            var dist1 = Vector2.Distance(position, point1);
            var totalDist = dist0 + dist1;
            if (totalDist < float.Epsilon)
            {
                return new Vector2(1f / 2, 1f / 2);
            }

            var weights = new Vector2
            {
                x = totalDist / (dist0 + float.Epsilon),
                y = totalDist / (dist1 + float.Epsilon),
            };

            // Normalization
            var totalWeight = weights.x + weights.y;
            weights.x /= totalWeight;
            weights.y /= totalWeight;

            return weights;
        }
    }
}
