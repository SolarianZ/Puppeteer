using System.Collections.Generic;
using GBG.Puppeteer.Editor.GraphParam;
using GBG.Puppeteer.Editor.Utility;
using GBG.Puppeteer.Graph;
using GBG.Puppeteer.NodeData;
using GBG.Puppeteer.Parameter;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;
using UDebug = UnityEngine.Debug;

namespace GBG.Puppeteer.Editor.GraphNode
{
    public class AnimationScriptNode : PlayableNode
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

        private readonly ObjectField _scriptableField;

        private readonly List<IParamBindingField> _paramBindingFields = new List<IParamBindingField>();

        private const float _INPUT_LABEL_WIDTH = 90;


        public AnimationScriptNode(string guid, List<ParamInfo> readOnlyParamTable) : base(guid, readOnlyParamTable)
        {
            // Playback speed
            PlaybackSpeedField = new ParamField<float>("Speed", labelWidth: _INPUT_LABEL_WIDTH);
            PlaybackSpeedField.SetParamChoices(ReadOnlyParamTable);
            PlaybackSpeedField.SetParamInfo(ParamInfo.CreateLiteral(ParamType.Float, 1));
            PlaybackSpeedField.OnValueChanged += OnPlaybackSpeedValueChanged;
            inputContainer.Add(PlaybackSpeedField);

            // Sub graph
            _scriptableField = new ObjectField
            {
                label = "Sub Graph",
                objectType = typeof(AnimationScriptableObject),
                style =
                {
                    paddingRight = 0,
                    marginRight = 4,
                }
            };
            _scriptableField.labelElement.style.minWidth = 0;
            _scriptableField.RegisterValueChangedCallback(OnScriptableChanged);
            inputContainer.Add(_scriptableField);

            // Add input button
            var addInputButton = new Button(AddMixerInput)
            {
                text = "Add Input Port"
            };
            inputContainer.Add(addInputButton);

            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        public override void PopulateView(AnimationNodeData nodeData)
        {
            var scriptNodeData = (AnimationScriptNodeData)nodeData;

            // Playback speed
            PlaybackSpeedField.SetParamInfo(scriptNodeData.PlaybackSpeed
                .GetParamInfo(ReadOnlyParamTable, ParamType.Float));

            // Sub graph
            _scriptableField.value = scriptNodeData.AnimationScriptable;

            // Param bindings
            if (scriptNodeData.AnimationScriptable)
            {
                var scriptParams = scriptNodeData.AnimationScriptable.GetParameters();
                for (int i = 0; i < scriptParams.Length; i++)
                {
                    var subGraphParamInfo = scriptParams[i];
                    if (!ParamBindingTool.TryGetParamBindingSource(scriptNodeData.ParamBindingSources,
                            subGraphParamInfo.Name, out var binding))
                    {
                        binding = new ParamBindingNameOrValue(subGraphParamInfo.Name, null, 0);
                    }

                    var inputParamInfo = binding.GetParamBindingSource(ReadOnlyParamTable, subGraphParamInfo.Type);
                    var bindingField = ParamBindingTool.CreateParamBindingField(subGraphParamInfo.Name,
                        subGraphParamInfo.Type, _INPUT_LABEL_WIDTH);
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
                title = _scriptableField.value ? _scriptableField.value.name : null;
            }
            else
            {
                title = _nodeTitle;
            }

            // Mixer inputs
            // Remove default mixer inputs
            foreach (var defaultMixerInput in InternalMixerInputs)
            {
                inputContainer.Remove(defaultMixerInput);
            }

            InternalMixerInputs.Clear();

            // Restore mixer inputs
            foreach (var inputInfo in scriptNodeData.InputInfos)
            {
                var mixerInput = new MixerInput(DeleteMixerInput);
                var mixerInputInfo = (MixerInputInfo)inputInfo;
                if (mixerInputInfo != null)
                {
                    var inputWeightParam =
                        mixerInputInfo.InputWeightParam.GetParamInfo(ReadOnlyParamTable, ParamType.Float);
                    mixerInput.InputWeightField.SetParamInfo(inputWeightParam);
                }

                inputContainer.Insert(inputContainer.childCount - 1, mixerInput);
                InternalMixerInputs.Add(mixerInput);
            }

            for (var i = 0; i < InternalMixerInputs.Count; i++)
            {
                InternalMixerInputs[i].UpdateIndex(i, InternalMixerInputs.Count > 1);
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

            var subGraph = _scriptableField.value as RuntimeAnimationGraph;
            if (!subGraph)
            {
                return;
            }

            // Create new binding fields
            foreach (var paramInfo in subGraph.Parameters)
            {
                var bindingField = ParamBindingTool.CreateParamBindingField(paramInfo.Name, paramInfo.Type,
                    _INPUT_LABEL_WIDTH);
                bindingField.SetParamChoices((List<ParamInfo>)ReadOnlyParamTable);
                bindingField.OnValueChanged += OnParamBindingChanged;
                _paramBindingFields.Add((IParamBindingField)bindingField);
                inputContainer.Add(bindingField);
            }
        }

        private void AddMixerInput()
        {
            var mixerInput = new MixerInput(DeleteMixerInput, _INPUT_LABEL_WIDTH);
            mixerInput.InputWeightField.SetParamChoices(ReadOnlyParamTable);
            mixerInput.InputWeightField.OnValueChanged += OnInputWeightValueChanged;
            inputContainer.Insert(inputContainer.childCount - 1, mixerInput);
            InternalMixerInputs.Add(mixerInput);

            for (var i = 0; i < InternalMixerInputs.Count; i++)
            {
                InternalMixerInputs[i].UpdateIndex(i, InternalMixerInputs.Count > 1);
            }
        }

        private void DeleteMixerInput(MixerInput mixerInput)
        {
            mixerInput.Disconnect();

            inputContainer.Remove(mixerInput);
            InternalMixerInputs.Remove(mixerInput);
            for (var i = 0; i < InternalMixerInputs.Count; i++)
            {
                InternalMixerInputs[i].UpdateIndex(i, InternalMixerInputs.Count > 1);
            }
        }

        private void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            // Since we put input ports into a separate visual element,
            // edges will be remain in graph view when deleting the node,
            // so we need to remove edges manually.
            foreach (var mixerInput in MixerInputs)
            {
                mixerInput.Disconnect();
            }
        }


        #region Value Change Callbacks

        private void OnPlaybackSpeedValueChanged(ParamField<float> _)
        {
            RaiseNodeDataChangedEvent();
        }

        private void OnScriptableChanged(ChangeEvent<UObject> _)
        {
            // Update node title(if there is not a explicit title)
            if (string.IsNullOrEmpty(_nodeTitle))
            {
                title = _scriptableField.value ? _scriptableField.value.name : null;
            }

            ResetParamBindings();

            RaiseNodeDataChangedEvent();
        }

        private void OnParamBindingChanged(ParamField _)
        {
            RaiseNodeDataChangedEvent();
        }

        private void OnInputWeightValueChanged(ParamField<float> _)
        {
            RaiseNodeDataChangedEvent();
        }

        #endregion


        #region Deep Clone

        protected override AnimationNodeData CreateCloneNodeDataInstance()
        {
            return new AnimationScriptNodeData();
        }

        protected override void CloneNodeDataMembers(AnimationNodeData clone)
        {
            base.CloneNodeDataMembers(clone);

            var scriptableNodeData = (AnimationScriptNodeData)clone;

            // Scriptable
            scriptableNodeData.AnimationScriptable = (AnimationScriptableObject)_scriptableField.value;

            // Input infos
            var inputInfos = new List<InputInfo>();
            for (int i = 0; i < MixerInputs.Count; i++)
            {
                var inputInfo = MixerInputs[i].GetInputInfo();
                inputInfos.Add(inputInfo);
            }

            scriptableNodeData.InputInfos = inputInfos.ToArray();

            // Param bindings
            scriptableNodeData.ParamBindingSources = new ParamBindingNameOrValue[_paramBindingFields.Count];
            for (int i = 0; i < _paramBindingFields.Count; i++)
            {
                var paramBinding = _paramBindingFields[i];
                if (!paramBinding.GetParamBindingInfo(out var paramBindingInfo))
                {
                    UDebug.LogError($"[Puppeteer::PlayableNode] Invalid param binding link on node '{title}'.");
                }

                scriptableNodeData.ParamBindingSources[i] = new ParamBindingNameOrValue(paramBindingInfo);
            }
        }

        #endregion
    }
}