using System;
using System.Collections.Generic;
using GBG.Puppeteer.Editor.GraphParam;
using GBG.Puppeteer.Graph;
using GBG.Puppeteer.NodeData;
using GBG.Puppeteer.Parameter;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;
using UDebug = UnityEngine.Debug;

namespace GBG.Puppeteer.Editor.GraphNode
{
    public sealed class AnimationSubGraphNode : PlayableNode
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

        // TODO: Disallow reference self graph asset
        private readonly ObjectField _subGraphField;

        private readonly List<IParamBindingField> _paramBindingFields = new List<IParamBindingField>();

        private const float _INPUT_LABEL_WIDTH = 90;


        public AnimationSubGraphNode(string guid, List<ParamInfo> readOnlyParamTable) : base(guid, readOnlyParamTable)
        {
            // Playback speed
            PlaybackSpeedField = new ParamField<float>("Speed", labelWidth: _INPUT_LABEL_WIDTH);
            PlaybackSpeedField.SetParamChoices(ReadOnlyParamTable);
            PlaybackSpeedField.SetParamInfo(ParamInfo.CreateLiteral(ParamType.Float, 1));
            PlaybackSpeedField.OnValueChanged += OnPlaybackSpeedValueChanged;
            inputContainer.Add(PlaybackSpeedField);

            // Sub graph
            _subGraphField = new ObjectField
            {
                label = "Sub Graph",
                objectType = typeof(RuntimeAnimationGraph),
                style =
                {
                    paddingRight = 0,
                    marginRight = 4,
                }
            };
            _subGraphField.labelElement.style.minWidth = 0;
            _subGraphField.RegisterValueChangedCallback(OnSubGraphChanged);
            inputContainer.Add(_subGraphField);
        }

        public override void PopulateView(AnimationNodeData nodeData)
        {
            var subGraphNodeData = (AnimationSubGraphNodeData)nodeData;

            // Playback speed
            PlaybackSpeedField.SetParamInfo(subGraphNodeData.PlaybackSpeed.GetParamInfo(ReadOnlyParamTable, ParamType.Float));

            // Sub graph
            _subGraphField.value = subGraphNodeData.SubGraph;

            // Param bindings
            if (subGraphNodeData.SubGraph)
            {
                for (int i = 0; i < subGraphNodeData.SubGraph.Parameters.Count; i++)
                {
                    var subGraphParamInfo = subGraphNodeData.SubGraph.Parameters[i];
                    if (!TryGetParamBindingSource(subGraphNodeData.ParamBindingSources, subGraphParamInfo.Name,
                            out var binding))
                    {
                        binding = new ParamBindingNameOrValue(subGraphParamInfo.Name, null, 0);
                    }

                    var inputParamInfo = binding.GetParamBindingSource(ReadOnlyParamTable, subGraphParamInfo.Type);
                    var bindingField = CreateParamBindingField(subGraphParamInfo.Name, subGraphParamInfo.Type);
                    bindingField.SetParamChoices(ReadOnlyParamTable);
                    bindingField.SetParamInfo(inputParamInfo);
                    bindingField.OnValueChanged += OnParamBindingChanged;
                    _paramBindingFields.Add((IParamBindingField)bindingField);
                    inputContainer.Add(bindingField);
                }
            }

            // Title
            _nodeTitle = nodeData.EditorName;
            if (string.IsNullOrEmpty(_nodeTitle))
            {
                title = _subGraphField.value ? _subGraphField.value.name : null;
            }
            else
            {
                title = _nodeTitle;
            }
        }


        private void ResetParamBindings()
        {
            // Remove all binding fields
            foreach (var paramBindingField in _paramBindingFields)
            {
                var paramField = (ParamField)paramBindingField;
                paramField.OnValueChanged -= OnParamBindingChanged;
                inputContainer.Remove(paramField);
            }

            _paramBindingFields.Clear();

            var subGraph = _subGraphField.value as RuntimeAnimationGraph;
            if (!subGraph)
            {
                return;
            }

            // Create new binding fields
            foreach (var paramInfo in subGraph.Parameters)
            {
                var bindingField = CreateParamBindingField(paramInfo.Name, paramInfo.Type);
                bindingField.SetParamChoices((List<ParamInfo>)ReadOnlyParamTable);
                bindingField.OnValueChanged += OnParamBindingChanged;
                _paramBindingFields.Add((IParamBindingField)bindingField);
                inputContainer.Add(bindingField);
            }
        }


        private static ParamField CreateParamBindingField(string bindToParamName, ParamType paramType)
        {
            switch (paramType)
            {
                case ParamType.Float:
                    return new ParamBindingField<float>(bindToParamName, _INPUT_LABEL_WIDTH);

                case ParamType.Int:
                    return new ParamBindingField<int>(bindToParamName, _INPUT_LABEL_WIDTH);

                case ParamType.Bool:
                    return new ParamBindingField<bool>(bindToParamName, _INPUT_LABEL_WIDTH);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool TryGetParamBindingSource(IEnumerable<ParamBindingNameOrValue> bindings,
            string bindToParamName, out ParamBindingNameOrValue binding)
        {
            foreach (var paramBindingNameOrValue in bindings)
            {
                if (paramBindingNameOrValue.BindToName.Equals(bindToParamName))
                {
                    binding = paramBindingNameOrValue;
                    return true;
                }
            }

            binding = default;
            return false;
        }


        #region Value Change Callbacks

        private void OnPlaybackSpeedValueChanged(ParamField<float> _)
        {
            RaiseNodeDataChangedEvent();
        }

        private void OnSubGraphChanged(ChangeEvent<UObject> _)
        {
            // Update node title(if there is not a explicit title)
            if (string.IsNullOrEmpty(_nodeTitle))
            {
                title = _subGraphField.value ? _subGraphField.value.name : null;
            }

            ResetParamBindings();

            RaiseNodeDataChangedEvent();
        }

        private void OnParamBindingChanged(ParamField _)
        {
            RaiseNodeDataChangedEvent();
        }

        #endregion


        #region Deep Clone

        protected override AnimationNodeData CreateCloneNodeDataInstance()
        {
            return new AnimationSubGraphNodeData();
        }

        protected override void CloneNodeDataMembers(AnimationNodeData clone)
        {
            base.CloneNodeDataMembers(clone);

            var subGraphNodeData = (AnimationSubGraphNodeData)clone;

            // Sub graph
            subGraphNodeData.SubGraph = (RuntimeAnimationGraph)_subGraphField.value;

            // Param bindings
            subGraphNodeData.ParamBindingSources = new ParamBindingNameOrValue[_paramBindingFields.Count];
            for (int i = 0; i < _paramBindingFields.Count; i++)
            {
                var paramBinding = (IParamBindingField)_paramBindingFields[i];
                if (!paramBinding.GetParamBindingInfo(out var paramBindingInfo))
                {
                    UDebug.LogError($"[Puppeteer::PlayableNode] Invalid param binding link on node '{title}'.");
                }

                subGraphNodeData.ParamBindingSources[i] = new ParamBindingNameOrValue(paramBindingInfo);
            }
        }

        #endregion
    }
}
