using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.ViewElement
{
    public class UIElementDrawLineSample : VisualElement
    {
        public byte LineWidth
        {
            get => _lineWidth;
            set
            {
                _lineWidth = value;
                MarkDirtyRepaint();
            }
        }

        private byte _lineWidth = 1;

        public Color LineColor
        {
            get => _lineColor;
            set
            {
                _lineColor = value;
                MarkDirtyRepaint();
            }
        }

        private Color _lineColor = Color.white;

        public Vector2 LineStart
        {
            get => _lineStart;
            set
            {
                _lineStart = value;
                MarkDirtyRepaint();
            }
        }

        private Vector2 _lineStart;

        public Vector2 LineEnd
        {
            get => _lineEnd;
            set
            {
                _lineEnd = value;
                MarkDirtyRepaint();
            }
        }

        private Vector2 _lineEnd;


        public UIElementDrawLineSample(Vector2 lineStart, Vector2 lineEnd)
        {
            _lineStart = lineStart;
            _lineEnd = lineEnd;

            style.flexGrow = 1;

            generateVisualContent += GenLineVisualContent;
        }


        private void GenLineVisualContent(MeshGenerationContext mgc)
        {
            var meshWriteData = mgc.Allocate(4, 6);
            var slope = (LineStart.y - LineEnd.y) / (LineStart.x - LineEnd.x);
            var closerToHorizontal = (slope >= -1 && slope <= 1);

            // Vertices
            var endpointOffset = closerToHorizontal
                ? new Vector3(0, LineWidth, 0) // Closer to horizontal
                : new Vector3(LineWidth, 0, 0); // Closer to vertical
            var lineStart3D = new Vector3(LineStart.x, LineStart.y, Vertex.nearZ);
            var lineEnd3D = new Vector3(LineEnd.x, LineEnd.y, Vertex.nearZ);
            meshWriteData.SetNextVertex(new Vertex
            {
                position = lineStart3D,
                tint = LineColor
            });
            meshWriteData.SetNextVertex(new Vertex
            {
                position = lineEnd3D,
                tint = LineColor
            });
            meshWriteData.SetNextVertex(new Vertex
            {
                position = lineEnd3D + endpointOffset,
                tint = LineColor
            });
            meshWriteData.SetNextVertex(new Vertex
            {
                position = lineStart3D + endpointOffset,
                tint = LineColor
            });

            // Indices
            if ((closerToHorizontal && lineStart3D.x <= lineEnd3D.x) ||
                (!closerToHorizontal && lineStart3D.y >= lineEnd3D.y))
            {
                // Ascending order
                meshWriteData.SetNextIndex(0);
                meshWriteData.SetNextIndex(1);
                meshWriteData.SetNextIndex(2);
                meshWriteData.SetNextIndex(2);
                meshWriteData.SetNextIndex(3);
                meshWriteData.SetNextIndex(0);
            }
            else
            {
                // Descending order
                meshWriteData.SetNextIndex(0);
                meshWriteData.SetNextIndex(3);
                meshWriteData.SetNextIndex(2);
                meshWriteData.SetNextIndex(2);
                meshWriteData.SetNextIndex(1);
                meshWriteData.SetNextIndex(0);
            }
        }
    }
}
