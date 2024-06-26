﻿using GBG.AnimationGraph.Editor.Port;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UEdge = UnityEditor.Experimental.GraphView.Edge;

namespace GBG.AnimationGraph.Editor.GraphEdge
{
    public class FlowingGraphEdge : UEdge
    {
        public GraphPort InputPort => input as GraphPort;

        public GraphPort OutputPort => output as GraphPort;

        public bool EnableFlow
        {
            get => _isEnableFlow;
            set
            {
                if (_isEnableFlow == value)
                {
                    return;
                }

                _isEnableFlow = value;
                if (_isEnableFlow)
                {
                    Add(FlowImage);
                }
                else
                {
                    Remove(FlowImage);
                }
            }
        }

        private bool _isEnableFlow;

        public float FlowSize
        {
            get => _flowSize;
            set
            {
                _flowSize = value;
                FlowImage.style.width = new Length(_flowSize, LengthUnit.Pixel);
                FlowImage.style.height = new Length(_flowSize, LengthUnit.Pixel);
            }
        }

        private float _flowSize = 6f;

        public float FlowSpeed { get; set; } = 150f;

        protected Image FlowImage { get; }


        public FlowingGraphEdge()
        {
            FlowImage = new Image
            {
                name = "flow-image",
                style =
                {
                    width = new Length(FlowSize, LengthUnit.Pixel),
                    height = new Length(FlowSize, LengthUnit.Pixel),
                    borderTopLeftRadius = new Length(FlowSize / 2, LengthUnit.Pixel),
                    borderTopRightRadius = new Length(FlowSize / 2, LengthUnit.Pixel),
                    borderBottomLeftRadius = new Length(FlowSize / 2, LengthUnit.Pixel),
                    borderBottomRightRadius = new Length(FlowSize / 2, LengthUnit.Pixel),
                }
            };
            // Add(_flowImg);

            edgeControl.RegisterCallback<GeometryChangedEvent>(OnEdgeControlGeometryChanged);

            EnableFlow = true;
        }

        public override bool UpdateEdgeControl()
        {
            if (!base.UpdateEdgeControl())
            {
                return false;
            }

            UpdateFlow();
            return true;
        }


        #region Flow

        private float _totalEdgeLength;

        private float _passedEdgeLength;

        private int _flowPhaseIndex;

        private double _flowPhaseStartTime;

        private double _flowPhaseDuration;

        private float _currentPhaseLength;


        public void UpdateFlow()
        {
            if (!EnableFlow)
            {
                return;
            }

            // Position
            var posProgress = (EditorApplication.timeSinceStartup - _flowPhaseStartTime) / _flowPhaseDuration;
            var flowStartPoint = edgeControl.controlPoints[_flowPhaseIndex];
            var flowEndPoint = edgeControl.controlPoints[_flowPhaseIndex + 1];
            var flowPos = Vector2.Lerp(flowStartPoint, flowEndPoint, (float)posProgress);
            FlowImage.transform.position = flowPos - Vector2.one * FlowSize / 2;

            // TODO: Flow direction
            
            
            // Color
            var colorProgress = (_passedEdgeLength + _currentPhaseLength * posProgress) / _totalEdgeLength;
            var startColor = edgeControl.outputColor;
            var endColor = edgeControl.inputColor;
            var flowColor = Color.Lerp(startColor, endColor, (float)colorProgress);
            FlowImage.style.backgroundColor = flowColor;

            // Enter next phase
            if (posProgress >= 0.99999f)
            {
                _passedEdgeLength += _currentPhaseLength;

                _flowPhaseIndex++;
                if (_flowPhaseIndex >= edgeControl.controlPoints.Length - 1)
                {
                    // Restart flow
                    _flowPhaseIndex = 0;
                    _passedEdgeLength = 0;
                }

                _flowPhaseStartTime = EditorApplication.timeSinceStartup;
                _currentPhaseLength = Vector2.Distance(edgeControl.controlPoints[_flowPhaseIndex],
                    edgeControl.controlPoints[_flowPhaseIndex + 1]);
                _flowPhaseDuration = _currentPhaseLength / FlowSpeed;
            }
        }

        private void OnEdgeControlGeometryChanged(GeometryChangedEvent evt)
        {
            // Restart flow
            _flowPhaseIndex = 0;
            _passedEdgeLength = 0;
            _flowPhaseStartTime = EditorApplication.timeSinceStartup;
            _currentPhaseLength = Vector2.Distance(edgeControl.controlPoints[_flowPhaseIndex],
                edgeControl.controlPoints[_flowPhaseIndex + 1]);
            _flowPhaseDuration = _currentPhaseLength / FlowSpeed;

            // Calculate edge path length
            _totalEdgeLength = 0;
            for (int i = 0; i < edgeControl.controlPoints.Length - 1; i++)
            {
                var p = edgeControl.controlPoints[i];
                var pNext = edgeControl.controlPoints[i + 1];
                var phaseLen = Vector2.Distance(p, pNext);
                _totalEdgeLength += phaseLen;
            }
        }

        #endregion
    }
}
