using System;
using System.Collections.Generic;
using System.Linq;
using GBG.Puppeteer.ThirdParties;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.BlendSpace
{
    public class BlendSpace2DPreviewWindow : EditorWindow
    {
        [MenuItem("Puppeteer/Test/BlendSpace2D Previewer Test")]
        private static void TestOpen()
        {
            var vertices = new Vector2[] // Test
            {
                new Vector2(0.00f, 0.00f),
                new Vector2(0.00f, 0.20f),
                new Vector2(0.00f, 0.40f),
                new Vector2(0.00f, -0.20f),
                new Vector2(0.00f, -0.40f),
                new Vector2(-0.20f, 0.20f),
                new Vector2(-0.20f, 0.40f),
                new Vector2(0.20f, 0.20f),
                new Vector2(0.20f, 0.40f),
                new Vector2(-0.20f, -0.20f),
                new Vector2(-0.20f, -0.40f),
                new Vector2(0.20f, -0.20f),
                new Vector2(0.20f, -0.40f),
            };

            var triangles = new Delaunay2D.Triangle[] // Test
            {
                new Delaunay2D.Triangle(1, 0, 5),
                new Delaunay2D.Triangle(2, 1, 6),
                new Delaunay2D.Triangle(1, 5, 6),
                new Delaunay2D.Triangle(0, 1, 7),
                new Delaunay2D.Triangle(1, 2, 8),
                new Delaunay2D.Triangle(7, 1, 8),
                new Delaunay2D.Triangle(0, 3, 9),
                new Delaunay2D.Triangle(5, 0, 9),
                new Delaunay2D.Triangle(3, 4, 10),
                new Delaunay2D.Triangle(9, 3, 10),
                new Delaunay2D.Triangle(3, 0, 11),
                new Delaunay2D.Triangle(0, 7, 11),
                new Delaunay2D.Triangle(4, 3, 12),
                new Delaunay2D.Triangle(3, 11, 12),
            };

            Open("BlendSpace Preview", vertices, triangles);
        }

        public static void Open(string name, IEnumerable<Vector2> vertices, IEnumerable<Delaunay2D.Triangle> triangles)
        {
            Assert.IsTrue(vertices != null);
            Assert.IsTrue(triangles != null);

            var window = GetWindow<BlendSpace2DPreviewWindow>(name);
            window._vertices = vertices.ToArray();
            window._triangles = triangles.ToArray();
            window.Initialize();
        }


        private Rect _graphBounds;


        private Rect _vertexBounds;

        private Vector2[] _vertices;

        private Delaunay2D.Triangle[] _triangles;


        private Vector2 _inputPosition;

        private Slider _xPositionSlider;

        private Slider _yPositionSlider;


        private GUIStyle _weightLabelStyle;


        private bool _lockGraphAspect;

        private GUIStyle _lockAspectButtonStyle;

        private GUIContent _lockAspectButtonContent;


        private void OnEnable()
        {
            _graphBounds = position;

            _xPositionSlider = new Slider("X Position");
            _xPositionSlider.RegisterValueChangedCallback(OnXPositionChanged);
            rootVisualElement.Add(_xPositionSlider);

            _yPositionSlider = new Slider("Y Position");
            _yPositionSlider.RegisterValueChangedCallback(OnYPositionChanged);
            rootVisualElement.Add(_yPositionSlider);

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
                    _lockGraphAspect);
                _xPositionSlider.value = inputPos.x;
                _yPositionSlider.value = inputPos.y;
            });
            rootVisualElement.Add(previewGraph);
        }

        /// <summary>
        /// Draw buttons on toolbar.
        /// Automatically called by unity.
        /// </summary>
        /// <param name="buttonPosition"></param>
        private void ShowButton(Rect buttonPosition)
        {
            // Button style
            if (_lockAspectButtonStyle == null)
            {
                _lockAspectButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    // Remove paddings
                    padding = new RectOffset()
                };
            }

            if (_lockAspectButtonContent == null)
            {
                UpdateLockAspectButtonContent();
            }

            // Draw lock aspect button
            if (GUI.Button(buttonPosition, _lockAspectButtonContent, _lockAspectButtonStyle))
            {
                _lockGraphAspect = !_lockGraphAspect;
                UpdateLockAspectButtonContent();
            }

            void UpdateLockAspectButtonContent()
            {
                _lockAspectButtonContent = _lockGraphAspect
                    ? EditorGUIUtility.IconContent("LockIcon-On")
                    : EditorGUIUtility.IconContent("LockIcon");
                _lockAspectButtonContent.tooltip = _lockGraphAspect
                    ? "Graph aspect locked"
                    : "Graph aspect unlocked";
            }
        }

        private void Initialize()
        {
            _graphBounds = position;
            _vertexBounds = GetVertexBounds(_vertices);
            _inputPosition = _vertexBounds.center;
            _xPositionSlider.lowValue = _vertexBounds.xMin;
            _xPositionSlider.highValue = _vertexBounds.xMax;
            _xPositionSlider.value = _inputPosition.x;
            _yPositionSlider.lowValue = _vertexBounds.yMin;
            _yPositionSlider.highValue = _vertexBounds.yMax;
            _yPositionSlider.value = _inputPosition.y;

            if (_triangles.Length == 0)
            {
                var warnContent = EditorGUIUtility.IconContent("console.warnicon");
                warnContent.text = "Vertices is empty or collinear!";
                ShowNotification(warnContent, Double.MaxValue);
            }
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
                var localVertex = TransformPoint(vertex, _vertexBounds, _graphBounds, _lockGraphAspect);
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
                    var localVert0 = TransformPoint(vert0, _vertexBounds, _graphBounds, _lockGraphAspect);
                    var localVert1 = TransformPoint(vert1, _vertexBounds, _graphBounds, _lockGraphAspect);
                    var localVert2 = TransformPoint(vert2, _vertexBounds, _graphBounds, _lockGraphAspect);
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

                var localVertex0 = TransformPoint(targetVertex0, _vertexBounds, _graphBounds, _lockGraphAspect);
                var localVertex1 = TransformPoint(targetVertex1, _vertexBounds, _graphBounds, _lockGraphAspect);
                var localVertex2 = TransformPoint(targetVertex2, _vertexBounds, _graphBounds, _lockGraphAspect);

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
            Handles.DrawSolidDisc(TransformPoint(inputPos, _vertexBounds, _graphBounds, _lockGraphAspect),
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
