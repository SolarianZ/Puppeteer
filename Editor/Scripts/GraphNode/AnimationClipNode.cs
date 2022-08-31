using System;
using System.Collections.Generic;
using GBG.Puppeteer.Editor.GraphEdge;
using GBG.Puppeteer.Editor.GraphParam;
using GBG.Puppeteer.Editor.GraphPort;
using GBG.Puppeteer.Editor.Utility;
using GBG.Puppeteer.NodeData;
using GBG.Puppeteer.Parameter;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;
using GraphViewPort = UnityEditor.Experimental.GraphView.Port;

namespace GBG.Puppeteer.Editor.GraphNode
{
    public class AnimationClipNode : PlayableNode
    {
        /// <summary>
        ///   <para>Node's title element.</para>
        /// </summary>
        public override string title
        {
            get => base.title;
            set
            {
                _nodeTitle = value;
                base.title = _nodeTitle;
            }
        }

        protected sealed override ParamField<float> PlaybackSpeedField { get; }

        private readonly ObjectField _clipField;

        private readonly ParamField<bool> _useExplicitTimeField;

        private readonly ParamField<float> _explicitTimeField;

        private string _nodeTitle;

        private const float _inputLabelWidth = 92;


        public AnimationClipNode(string guid) : base(guid)
        {
            // Playback speed
            PlaybackSpeedField = new ParamField<float>("Speed", labelWidth: _inputLabelWidth);
            inputContainer.Add(PlaybackSpeedField);

            // Clip
            _clipField = new ObjectField
            {
                label = "Clip",
                objectType = typeof(AnimationClip)
            };
            _clipField.labelElement.style.minWidth = 0;
            _clipField.RegisterValueChangedCallback(OnClipChanged);
            inputContainer.Add(_clipField);

            // Use explicit time
            _useExplicitTimeField = new ParamField<bool>("Use Explicit Time", labelWidth: _inputLabelWidth);
            inputContainer.Add(_useExplicitTimeField);

            // Explicit time
            _explicitTimeField = new ParamField<float>("Explicit Time", labelWidth: _inputLabelWidth);
            inputContainer.Add(_explicitTimeField);
        }

        private void OnClipChanged(ChangeEvent<UObject> _)
        {
            // Update node title(if there is not a explicit title)
            if (string.IsNullOrEmpty(_nodeTitle))
            {
                title = _clipField.value ? _clipField.value.name : null;
            }
        }

        public override void PopulateView(AnimationNodeData nodeData, List<ParamInfo> parameters)
        {
            var clipNodeData = (AnimationClipNodeData)nodeData;

            // Playback speed
            PlaybackSpeedField.SetParamChoices(parameters);
            PlaybackSpeedField.SetParamInfo(clipNodeData.PlaybackSpeed.GetParamInfo(parameters, ParamType.Float));

            // Clip
            _clipField.value = clipNodeData.AnimationClip;

            // Title
            _nodeTitle = nodeData.EditorName;
            if (string.IsNullOrEmpty(_nodeTitle))
            {
                title = _clipField.value ? _clipField.value.name : null;
            }

            // Use explicit time
            _useExplicitTimeField.SetParamChoices(parameters);
            _useExplicitTimeField.SetParamInfo(clipNodeData.UseExplicitTime.GetParamInfo(parameters, ParamType.Bool));

            // Explicit time
            _explicitTimeField.SetParamChoices(parameters);
            _explicitTimeField.SetParamInfo(clipNodeData.ExplicitTime.GetParamInfo(parameters, ParamType.Float));
        }


        #region Deep Clone

        protected override AnimationNodeData CreateCloneNodeDataInstance()
        {
            return new AnimationClipNodeData();
        }

        protected override void CloneNodeDataMembers(AnimationNodeData clone)
        {
            base.CloneNodeDataMembers(clone);

            var clipNodeData = (AnimationClipNodeData)clone;

            // Clip
            clipNodeData.AnimationClip = (AnimationClip)_clipField.value;

            // Use explicit time
            if (!_useExplicitTimeField.GetParamInfo(out var useExplicitTimeParam))
            {
                Debug.LogError($"[Puppeteer::PlayableNode] Invalid 'UseExplicitTime' param link on node '{title}'.");
            }

            clipNodeData.UseExplicitTime = new ParamNameOrValue(useExplicitTimeParam);

            // Explicit time
            if (!_explicitTimeField.GetParamInfo(out var explicitTimeParam))
            {
                Debug.LogError($"[Puppeteer::PlayableNode] Invalid 'ExplicitTime' param link on node '{title}'.");
            }

            clipNodeData.ExplicitTime = new ParamNameOrValue(explicitTimeParam);
        }

        #endregion
    }
}
