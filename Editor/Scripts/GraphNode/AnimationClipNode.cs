using System.Collections.Generic;
using GBG.Puppeteer.Editor.GraphParam;
using GBG.Puppeteer.NodeData;
using GBG.Puppeteer.Parameter;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;
using UDebug = UnityEngine.Debug;
using GraphViewPort = UnityEditor.Experimental.GraphView.Port;

namespace GBG.Puppeteer.Editor.GraphNode
{
    public sealed class AnimationClipNode : PlayableNode
    {
        protected override string NodeName
        {
            get => _nodeTitle;
            set
            {
                _nodeTitle = value;
                title = _nodeTitle;
            }
        }

        private string _nodeTitle;


        protected override ParamField<float> PlaybackSpeedField { get; }

        private readonly ObjectField _clipField;

        private readonly ParamField<bool> _useExplicitTimeField;

        private readonly ParamField<float> _explicitTimeField;

        private const float _INPUT_LABEL_WIDTH = 90;


        public AnimationClipNode(string guid, List<ParamInfo> paramTable) : base(guid, paramTable)
        {
            // Playback speed
            PlaybackSpeedField = new ParamField<float>("Speed", labelWidth: _INPUT_LABEL_WIDTH);
            PlaybackSpeedField.SetParamInfo(ParamInfo.CreateLiteral(ParamType.Float, 1));
            PlaybackSpeedField.OnValueChanged += OnPlaybackSpeedValueChanged;
            inputContainer.Add(PlaybackSpeedField);

            // Clip
            _clipField = new ObjectField
            {
                label = "Clip",
                objectType = typeof(AnimationClip),
                style =
                {
                    paddingRight = 0,
                    marginRight = 4,
                }
            };
            _clipField.labelElement.style.minWidth = 0;
            _clipField.RegisterValueChangedCallback(OnClipChanged);
            inputContainer.Add(_clipField);

            // Use explicit time
            _useExplicitTimeField = new ParamField<bool>("Use Explicit Time", labelWidth: _INPUT_LABEL_WIDTH);
            _useExplicitTimeField.OnValueChanged += OnUseExplicitTimeValueChanged;
            inputContainer.Add(_useExplicitTimeField);

            // Explicit time
            _explicitTimeField = new ParamField<float>("Explicit Time", labelWidth: _INPUT_LABEL_WIDTH);
            _explicitTimeField.OnValueChanged += OnExplicitTimeValueChanged;
            inputContainer.Add(_explicitTimeField);
        }

        public override void PopulateView(AnimationNodeData nodeData)
        {
            var clipNodeData = (AnimationClipNodeData)nodeData;
            var paramTable = (List<ParamInfo>)ParamTable;

            // Playback speed
            PlaybackSpeedField.SetParamChoices(paramTable);
            PlaybackSpeedField.SetParamInfo(clipNodeData.PlaybackSpeed.GetParamInfo(paramTable, ParamType.Float));

            // Clip
            _clipField.value = clipNodeData.AnimationClip;

            // Title
            _nodeTitle = nodeData.EditorName;
            if (string.IsNullOrEmpty(_nodeTitle))
            {
                title = _clipField.value ? _clipField.value.name : null;
            }
            else
            {
                title = _nodeTitle;
            }

            // Use explicit time
            _useExplicitTimeField.SetParamChoices(paramTable);
            _useExplicitTimeField.SetParamInfo(clipNodeData.UseExplicitTime.GetParamInfo(paramTable, ParamType.Bool));

            // Explicit time
            _explicitTimeField.SetParamChoices(paramTable);
            _explicitTimeField.SetParamInfo(clipNodeData.ExplicitTime.GetParamInfo(paramTable, ParamType.Float));
        }


        #region Value Change Callbacks

        private void OnPlaybackSpeedValueChanged(ParamField<float> _)
        {
            RaiseNodeDataChangedEvent();
        }

        private void OnClipChanged(ChangeEvent<UObject> _)
        {
            // Update node title(if there is not a explicit title)
            if (string.IsNullOrEmpty(_nodeTitle))
            {
                title = _clipField.value ? _clipField.value.name : null;
            }

            RaiseNodeDataChangedEvent();
        }

        private void OnUseExplicitTimeValueChanged(ParamField<bool> _)
        {
            RaiseNodeDataChangedEvent();
        }

        private void OnExplicitTimeValueChanged(ParamField<float> _)
        {
            RaiseNodeDataChangedEvent();
        }

        #endregion


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
                UDebug.LogError($"[Puppeteer::PlayableNode] Invalid 'UseExplicitTime' param link on node '{title}'.");
            }

            clipNodeData.UseExplicitTime = new ParamNameOrValue(useExplicitTimeParam);

            // Explicit time
            if (!_explicitTimeField.GetParamInfo(out var explicitTimeParam))
            {
                UDebug.LogError($"[Puppeteer::PlayableNode] Invalid 'ExplicitTime' param link on node '{title}'.");
            }

            clipNodeData.ExplicitTime = new ParamNameOrValue(explicitTimeParam);
        }

        #endregion
    }
}
