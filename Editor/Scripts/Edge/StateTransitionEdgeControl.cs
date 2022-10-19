using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace GBG.AnimationGraph.Editor.GraphEdge
{
    [Flags]
    public enum StateTransitionEdgeDirections
    {
        // 0 -> 1
        Dir_0_1 = 1,

        // 1 -> 0
        Dir_1_0 = 2,

        // 0 <-> 1
        Bidirectional = Dir_0_1 | Dir_1_0,
    }

    public interface IEdgePointProvider
    {
        Vector2 GetEdgePoint0();

        Vector2 GetEdgePoint1();
    }

    public class StateTransitionEdgeControl : IMGUIContainer
    {
        public Vector2 EdgePoint0 => _pointProvider.GetEdgePoint0();

        public Vector2 EdgePoint1 => _pointProvider.GetEdgePoint1();

        public StateTransitionEdgeDirections EdgeDirections { get; private set; } =
            StateTransitionEdgeDirections.Dir_0_1;

        public Color NormalColor { get; set; } = Color.white;

        public Color HighlightColor { get; set; } = new Color(68 / 255f, 192 / 255f, 255 / 255f);

        public Color IndicationColor { get; set; } = new Color(200 / 255f, 200 / 255f, 40 / 255f);

        public ushort IndicationDurationMs { get; set; } = 600;

        public bool Highlight { get; set; }

        public byte EdgeWidth { get; set; } = 4;

        public byte ContainsExtension { get; set; } = 2;


        private Vector2 ArrowPosition => (EdgePoint0 + EdgePoint1) / 2 - Vector2.one * EdgeWidth * 3;

        private readonly IEdgePointProvider _pointProvider;

        private static Texture2D _lineTex;

        private static Texture2D _arrowTex;

        private StateTransitionEdgeDirections _indicationDirections;

        private float? _indicationAlpha01;

        private float? _indicationAlpha10;


        public StateTransitionEdgeControl(IEdgePointProvider pointProvider)
        {
            _pointProvider = pointProvider;

            name = "state-transition-edge-control";
            pickingMode = PickingMode.Ignore;
            style.flexGrow = 0;
            style.flexShrink = 0;
            style.position = Position.Absolute;
            style.width = 2; // For debugging
            style.height = 2; // For debugging
            transform.position = Vector3.zero;

            if (!_lineTex) _lineTex = Resources.Load<Texture2D>("AnimationGraph/LineNormal4");
            if (!_arrowTex) _arrowTex = Resources.Load<Texture2D>("AnimationGraph/ArrowNormal64");

            onGUIHandler = DrawEdge;
        }

        public void AddDirection(StateTransitionEdgeDirections direction)
        {
            EdgeDirections |= direction;
        }

        public void RemoveDirection(StateTransitionEdgeDirections direction)
        {
            EdgeDirections &= ~direction;
        }

        public override bool ContainsPoint(Vector2 localPoint)
        {
            if (!GetRect().Contains(localPoint))
            {
                return false;
            }

            // Check if the point is close to edge
            var vector0P = localPoint - EdgePoint0;
            var vector01 = EdgePoint1 - EdgePoint0;
            var dot = Vector2.Dot(vector0P, vector01);
            var sqrDist = vector0P.sqrMagnitude - dot * dot / vector01.sqrMagnitude;
            var contains = sqrDist < (EdgeWidth / 2 + ContainsExtension) * (EdgeWidth / 2 + ContainsExtension);

            return contains;
        }

        public override bool Overlaps(Rect rect)
        {
            if (GetRect().Overlaps(rect))
            {
                return RectUtils.IntersectsSegment(rect, EdgePoint0, EdgePoint1) || ContainsPoint(rect.position);
            }

            return false;
        }

        public void Indicate(StateTransitionEdgeDirections directions)
        {
            if (_indicationAlpha01 == null && (directions & StateTransitionEdgeDirections.Dir_0_1) != 0)
            {
                var anim = ValueAnimation<float>.Create(this,
                    (a, b, t) => Mathf.Lerp(a, b, Easing.InCubic(t)));
                anim.autoRecycle = true;
                anim.from = 1;
                anim.to = 0;
                anim.durationMs = IndicationDurationMs;
                anim.valueUpdated = (_, alpha) =>
                {
                    _indicationAlpha01 = alpha;
                    MarkDirtyRepaint();
                };
                anim.onAnimationCompleted = () => _indicationAlpha01 = null;
                anim.Start();
            }

            if (_indicationAlpha10 == null && (directions & StateTransitionEdgeDirections.Dir_1_0) != 0)
            {
                var anim = ValueAnimation<float>.Create(this,
                    (a, b, t) => Mathf.Lerp(a, b, Easing.InCubic(t)));
                anim.autoRecycle = true;
                anim.from = 1;
                anim.to = 0;
                anim.durationMs = IndicationDurationMs;
                anim.valueUpdated = (_, alpha) =>
                {
                    _indicationAlpha10 = alpha;
                    MarkDirtyRepaint();
                };
                anim.onAnimationCompleted = () => _indicationAlpha10 = null;
                anim.Start();
            }
        }


        private void DrawEdge()
        {
            var baseColor = Highlight ? HighlightColor : NormalColor;

            // Line
            Handles.color = baseColor;
            Handles.DrawAAPolyLine(_lineTex, EdgeWidth, EdgePoint0, EdgePoint1);

            // Arrow
            var arrowSize = EdgeWidth * 6;
            var arrowOffset = arrowSize / 2;
            var rotSign = (EdgePoint1.x - EdgePoint0.x) >= 0 ? 1 : -1;
            var guiColor = GUI.color;
            switch (EdgeDirections)
            {
                case StateTransitionEdgeDirections.Dir_0_1:
                {
                    GUI.color = _indicationAlpha01.HasValue
                        ? Color.Lerp(IndicationColor, baseColor, _indicationAlpha01.Value)
                        : baseColor;
                    DrawArrow01(arrowSize, Vector2.zero, rotSign);
                    break;
                }

                case StateTransitionEdgeDirections.Dir_1_0:
                {
                    GUI.color = _indicationAlpha10.HasValue
                        ? Color.Lerp(IndicationColor, baseColor, _indicationAlpha10.Value)
                        : baseColor;
                    DrawArrow10(arrowSize, Vector2.zero, rotSign);
                    break;
                }

                case StateTransitionEdgeDirections.Bidirectional:
                {
                    var lineDir = (EdgePoint1 - EdgePoint0).normalized;

                    GUI.color = _indicationAlpha01.HasValue
                        ? Color.Lerp(IndicationColor, baseColor, _indicationAlpha01.Value)
                        : baseColor;
                    DrawArrow01(arrowSize, lineDir * arrowOffset, rotSign);

                    GUI.color = _indicationAlpha10.HasValue
                        ? Color.Lerp(IndicationColor, baseColor, _indicationAlpha10.Value)
                        : baseColor;
                    DrawArrow10(arrowSize, lineDir * arrowOffset, rotSign);
                    break;
                }

                default:
                    GUI.color = guiColor;
                    throw new ArgumentOutOfRangeException();
            }

            GUI.color = guiColor;
        }

        private void DrawArrow01(float arrowSize, Vector2 arrowOffset, int rotationSign)
        {
            var rot01 = Vector2.Angle(Vector2.down, EdgePoint1 - EdgePoint0) * rotationSign;
            var pos01 = new Rect(ArrowPosition + arrowOffset, Vector2.one * arrowSize);
            GUIUtility.RotateAroundPivot(rot01, pos01.center);
            GUI.DrawTexture(pos01, _arrowTex);
            GUIUtility.RotateAroundPivot(-rot01, pos01.center);
        }

        private void DrawArrow10(float arrowSize, Vector2 arrowOffset, int rotationSign)
        {
            var rot10 = -Vector2.Angle(Vector2.down, EdgePoint0 - EdgePoint1) * rotationSign;
            var pos10 = new Rect(ArrowPosition - arrowOffset, Vector2.one * arrowSize);
            GUIUtility.RotateAroundPivot(rot10, pos10.center);
            GUI.DrawTexture(pos10, _arrowTex);
            GUIUtility.RotateAroundPivot(-rot10, pos10.center);
        }

        private Rect GetRect()
        {
            var x = Math.Min(EdgePoint0.x, EdgePoint1.x);
            var y = Math.Min(EdgePoint0.y, EdgePoint1.y);
            var width = Math.Abs(EdgePoint0.x - EdgePoint1.x);
            var height = Math.Abs(EdgePoint0.y - EdgePoint1.y);
            return new Rect(x, y, width, height);
        }
    }
}
