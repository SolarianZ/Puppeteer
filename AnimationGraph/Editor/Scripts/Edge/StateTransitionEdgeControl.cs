using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.GraphEdge
{
    public sealed class StateTransitionEdgeControl : VisualElement
    {
        public byte EdgeHalfWidth { get; set; } = 2;

        public byte ContainsExtension { get; set; } = 2;

        public Color EdgeColor { get; set; } = Color.white;

        public Vector2 EdgePoint0 { get; set; }

        public Vector2 EdgePoint1 { get; set; }


        public StateTransitionEdgeControl(Vector2 point0, Vector2 point1)
        {
            EdgePoint0 = point0;
            EdgePoint1 = point1;

            name = "state-transition-edge-control";
            pickingMode = PickingMode.Ignore;
            style.position = Position.Absolute;
            style.flexGrow = 0;
            style.flexShrink = 0;
            // style.backgroundColor = new Color(80 / 255f, 80 / 255f, 80 / 255f, 0.6f); // For debugging

            generateVisualContent += GenLineVisualContent;
        }

        public void UpdateView()
        {
            // About '-1': position will always offsets by (1,1), why?
            var x = Math.Min(EdgePoint0.x, EdgePoint1.x) - EdgeHalfWidth - 1;
            var y = Math.Min(EdgePoint0.y, EdgePoint1.y) - EdgeHalfWidth - 1;
            style.width = Math.Abs(EdgePoint0.x - EdgePoint1.x) + EdgeHalfWidth * 2;
            style.height = Math.Abs(EdgePoint0.y - EdgePoint1.y) + EdgeHalfWidth * 2;
            transform.position = this.ChangeCoordinatesTo(parent, new Vector2(x, y));

            MarkDirtyRepaint();
        }

        public override bool ContainsPoint(Vector2 localPoint)
        {
            if (!base.ContainsPoint(localPoint))
            {
                return false;
            }

            // Check if the point is close to edge
            var vector0P = localPoint - EdgePoint0;
            var vector01 = EdgePoint1 - EdgePoint0;
            var dot = Vector2.Dot(vector0P, vector01);
            var sqrDist = vector0P.sqrMagnitude - dot * dot / vector01.sqrMagnitude;
            var contains = sqrDist < (EdgeHalfWidth + ContainsExtension) * (EdgeHalfWidth + ContainsExtension);

            return contains;
        }

        public override bool Overlaps(Rect rect)
        {
            if (base.Overlaps(rect))
            {
                return RectUtils.IntersectsSegment(rect, EdgePoint0, EdgePoint1) || ContainsPoint(rect.position);
            }

            return false;
        }


        private void GenLineVisualContent(MeshGenerationContext mgc)
        {
            var meshWriteData = mgc.Allocate(4, 6);
            var slope = (EdgePoint0.y - EdgePoint1.y) / (EdgePoint0.x - EdgePoint1.x);
            var closerToHorizontal = (slope >= -1 && slope <= 1);

            // Vertices
            var pointOffset = closerToHorizontal
                ? new Vector3(0, EdgeHalfWidth, 0) // Closer to horizontal
                : new Vector3(EdgeHalfWidth, 0, 0); // Closer to vertical
            var point0 = new Vector3(EdgePoint0.x, EdgePoint0.y, Vertex.nearZ);
            var point1 = new Vector3(EdgePoint1.x, EdgePoint1.y, Vertex.nearZ);
            meshWriteData.SetNextVertex(new Vertex
            {
                position = point0 - pointOffset,
                tint = EdgeColor,
            });
            meshWriteData.SetNextVertex(new Vertex
            {
                position = point1 - pointOffset,
                tint = EdgeColor,
            });
            meshWriteData.SetNextVertex(new Vertex
            {
                position = point1 + pointOffset,
                tint = EdgeColor,
            });
            meshWriteData.SetNextVertex(new Vertex
            {
                position = point0 + pointOffset,
                tint = EdgeColor,
            });

            // Indices
            if ((closerToHorizontal && point0.x <= point1.x) ||
                (!closerToHorizontal && point0.y >= point1.y))
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
