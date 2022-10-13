using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.ThirdParties;
using GBG.AnimationGraph.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.ViewElement
{
    public class BlendSpace2DPreviewer : VisualElement
    {
        private Rect _graphBounds;


        private readonly Rect _vertexBounds;

        private readonly Vector2[] _vertices;

        private readonly Delaunay2D.Triangle[] _triangles;


        private readonly Slider _xPositionSlider;

        private readonly Slider _yPositionSlider;


        private GUIStyle _weightLabelStyle;


        public bool LockGraphAspect { get; set; }

        private GUIStyle _lockAspectButtonStyle;

        private GUIContent _lockAspectButtonContent;


        public BlendSpace2DPreviewer(IEnumerable<Vector2> vertices, IEnumerable<Delaunay2D.Triangle> triangles)
        {
            #region View Elements

            style.flexGrow = 1;

            _xPositionSlider = new Slider("X Position");
            _xPositionSlider.RegisterValueChangedCallback(OnXPositionChanged);
            Add(_xPositionSlider);

            _yPositionSlider = new Slider("Y Position");
            _yPositionSlider.RegisterValueChangedCallback(OnYPositionChanged);
            Add(_yPositionSlider);

            var previewGraph = new IMGUIContainer(DrawPreviewGraph)
            {
                style =
                {
                    flexGrow = 1,
                    marginLeft = 20,
                    marginRight = 20,
                    marginTop = 20,
                    marginBottom = 20,
                }
            };
            previewGraph.RegisterCallback<GeometryChangedEvent>(evt => { _graphBounds = evt.newRect; });
            previewGraph.RegisterCallback<MouseDownEvent>(evt =>
            {
                var inputPos = InverseTransformPoint(evt.localMousePosition, _vertexBounds, _graphBounds,
                    LockGraphAspect);
                _xPositionSlider.value = inputPos.x;
                _yPositionSlider.value = inputPos.y;
            });
            Add(previewGraph);

            #endregion

            #region Graph Data

            _vertices = vertices.ToArray();
            _triangles = triangles.ToArray();

            _vertexBounds = GetVertexBounds(_vertices);

            var inputPosition = _vertexBounds.center;
            _xPositionSlider.lowValue = _vertexBounds.xMin;
            _xPositionSlider.highValue = _vertexBounds.xMax;
            _xPositionSlider.value = inputPosition.x;
            _yPositionSlider.lowValue = _vertexBounds.yMin;
            _yPositionSlider.highValue = _vertexBounds.yMax;
            _yPositionSlider.value = inputPosition.y;

            if (_triangles.Length == 0)
            {
                var warnLabel = new Label
                {
                    text = "Warning: Vertices is empty or collinear!",
                    style =
                    {
                        color = Color.yellow,
                    }
                };
                Add(warnLabel);
            }

            #endregion
        }


        private void OnXPositionChanged(ChangeEvent<float> evt)
        {
            _xPositionSlider.label = $"X Position ({evt.newValue:F3})";
        }

        private void OnYPositionChanged(ChangeEvent<float> evt)
        {
            _yPositionSlider.label = $"Y Position ({evt.newValue:F3})";
        }

        private void DrawPreviewGraph()
        {
            const float POINT_RADIUS = 5f;

            // Draw vertices
            foreach (var vertex in _vertices)
            {
                var localVertex = TransformPoint(vertex, _vertexBounds, _graphBounds, LockGraphAspect);
                Handles.color = Color.white;
                Handles.DrawSolidDisc(localVertex, Vector3.back, POINT_RADIUS);
            }

            // Draw triangles
            var inputPos = new Vector2(_xPositionSlider.value, _yPositionSlider.value);
            if (_triangles.Length > 0)
            {
                var containerTriangleIndex = -1;
                var closestTriangleIndex = -1;
                var closestSqrDist = float.MaxValue;

                for (var i = 0; i < _triangles.Length; i++)
                {
                    var triangle = _triangles[i];
                    var vert0 = _vertices[triangle.Point0];
                    var vert1 = _vertices[triangle.Point1];
                    var vert2 = _vertices[triangle.Point2];
                    var localVert0 = TransformPoint(vert0, _vertexBounds, _graphBounds, LockGraphAspect);
                    var localVert1 = TransformPoint(vert1, _vertexBounds, _graphBounds, LockGraphAspect);
                    var localVert2 = TransformPoint(vert2, _vertexBounds, _graphBounds, LockGraphAspect);
                    Handles.color = Color.white;
                    Handles.DrawLine(localVert0, localVert1);
                    Handles.DrawLine(localVert1, localVert2);
                    Handles.DrawLine(localVert2, localVert0);

                    if (TriangleTool.IsPointInTriangle(inputPos, vert0, vert1, vert2))
                    {
                        containerTriangleIndex = i;
                    }

                    if (containerTriangleIndex > -1) continue;

                    var triangleCentroid = TriangleTool.GetTriangleCentroid(vert0, vert1, vert2);
                    var sqrDist = Vector2.SqrMagnitude(inputPos - triangleCentroid);
                    if (closestTriangleIndex < 0 || closestSqrDist > sqrDist)
                    {
                        closestTriangleIndex = i;
                        closestSqrDist = sqrDist;
                    }
                }

                // Draw weights
                var pointInsideTriangle = containerTriangleIndex > -1;
                var targetTriangleIndex = pointInsideTriangle ? containerTriangleIndex : closestTriangleIndex;
                var targetTriangle = _triangles[targetTriangleIndex];
                var targetVertex0 = _vertices[targetTriangle.Point0];
                var targetVertex1 = _vertices[targetTriangle.Point1];
                var targetVertex2 = _vertices[targetTriangle.Point2];
                var vertexWeights = TriangleTool.CalculateWeights(inputPos,
                    targetVertex0, targetVertex1, targetVertex2, pointInsideTriangle);

                var localVertex0 = TransformPoint(targetVertex0, _vertexBounds, _graphBounds, LockGraphAspect);
                var localVertex1 = TransformPoint(targetVertex1, _vertexBounds, _graphBounds, LockGraphAspect);
                var localVertex2 = TransformPoint(targetVertex2, _vertexBounds, _graphBounds, LockGraphAspect);

                _weightLabelStyle ??= new GUIStyle
                {
                    normal =
                    {
                        textColor = Color.green,
                    }
                };

                var labelOffset = new Vector2(-20, 5);
                Handles.Label(localVertex0 + labelOffset, vertexWeights.x.ToString("F3"), _weightLabelStyle);
                Handles.Label(localVertex1 + labelOffset, vertexWeights.y.ToString("F3"), _weightLabelStyle);
                Handles.Label(localVertex2 + labelOffset, vertexWeights.z.ToString("F3"), _weightLabelStyle);
            }

            // Draw input position
            Handles.color = Color.green;
            Handles.DrawSolidDisc(TransformPoint(inputPos, _vertexBounds, _graphBounds, LockGraphAspect),
                Vector3.back, POINT_RADIUS);
        }


        private static Rect GetVertexBounds(Vector2[] vertices)
        {
            if (vertices.Length == 0)
            {
                return Rect.zero;
            }

            float? xMin = null;
            float? xMax = null;
            float? yMin = null;
            float? yMax = null;

            foreach (var vertex in vertices)
            {
                if (xMin == null || xMin > vertex.x) xMin = vertex.x;
                if (xMax == null || xMax < vertex.x) xMax = vertex.x;
                if (yMin == null || yMin > vertex.y) yMin = vertex.y;
                if (yMax == null || yMax < vertex.y) yMax = vertex.y;
            }

            // ReSharper disable PossibleInvalidOperationException
            return Rect.MinMaxRect(xMin.Value, yMin.Value, xMax.Value, yMax.Value);
            // ReSharper restore PossibleInvalidOperationException
        }

        private static Vector2 TransformPoint(Vector2 point, Rect vertexBounds, Rect windowBounds, bool lockAspect)
        {
            var xScale = windowBounds.width / vertexBounds.width;
            var yScale = windowBounds.height / vertexBounds.height;
            var scale = lockAspect ? Vector2.one * Math.Min(xScale, yScale) : new Vector2(xScale, yScale);
            scale.y *= -1;
            var offset = point - vertexBounds.center;
            offset.Scale(scale);

            // Don't use windowBounds.center, transform point to window center
            var windowCenter = windowBounds.size / 2;
            var windowSpacePoint = windowCenter + offset;

            return windowSpacePoint;
        }

        private static Vector2 InverseTransformPoint(Vector2 point, Rect vertexBounds, Rect windowBounds,
            bool lockAspect)
        {
            var xScale = vertexBounds.width / windowBounds.width;
            var yScale = vertexBounds.height / windowBounds.height;
            var scale = lockAspect ? Vector2.one * Math.Max(xScale, yScale) : new Vector2(xScale, yScale);
            scale.y *= -1;
            // Don't use windowBounds.center, transform point to window center
            var windowCenter = windowBounds.size / 2;
            var offset = point - windowCenter;
            offset.Scale(scale);
            var originalPoint = vertexBounds.center / 2 + offset;

            return originalPoint;
        }
    }
}
