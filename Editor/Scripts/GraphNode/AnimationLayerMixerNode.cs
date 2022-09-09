using System.Collections.Generic;
using GBG.Puppeteer.Editor.GraphParam;
using GBG.Puppeteer.NodeData;
using GBG.Puppeteer.Parameter;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;

namespace GBG.Puppeteer.Editor.GraphNode
{
    public sealed class AnimationLayerMixerNode : PlayableNode
    {
        protected override ParamField<float> PlaybackSpeedField { get; }

        private const float _INPUT_LABEL_WIDTH = 76;


        public AnimationLayerMixerNode(string guid, List<ParamInfo> paramTable) : base(guid, paramTable)
        {
            // Add input button
            var addInputButton = new Button(AddLayerMixerInput)
            {
                text = "Add Input Port"
            };
            inputContainer.Add(addInputButton);

            // Playback speed
            PlaybackSpeedField = new ParamField<float>("Speed", labelWidth: _INPUT_LABEL_WIDTH);
            PlaybackSpeedField.SetParamInfo(ParamInfo.CreateLiteral(ParamType.Float, 1));
            PlaybackSpeedField.OnValueChanged += OnPlaybackSpeedValueChanged;
            inputContainer.Insert(inputContainer.childCount - 1, PlaybackSpeedField);

            // Default layer mixer input
            AddLayerMixerInput();

            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        public override void PopulateView(AnimationNodeData nodeData)
        {
            var animLayerMixerNodeData = (AnimationLayerMixerNodeData)nodeData;
            var paramTable = (List<ParamInfo>)ParamTable;

            // Playback speed
            PlaybackSpeedField.SetParamChoices(paramTable);
            PlaybackSpeedField.SetParamInfo(animLayerMixerNodeData.PlaybackSpeed
                .GetParamInfo(paramTable, ParamType.Float));

            // Mixer inputs
            // Remove default mixer inputs
            foreach (var defaultMixerInput in InternalMixerInputs)
            {
                inputContainer.Remove(defaultMixerInput);
            }

            InternalMixerInputs.Clear();

            // Restore mixer inputs
            foreach (var inputInfo in animLayerMixerNodeData.InputInfos)
            {
                var layerMixerInput = new LayerMixerInput(DeleteMixerInput, _INPUT_LABEL_WIDTH);
                var layerMixerInputInfo = (LayerMixerInputInfo)inputInfo;
                if (layerMixerInputInfo != null)
                {
                    var inputWeightParam = layerMixerInputInfo.InputWeightParam
                        .GetParamInfo(paramTable, ParamType.Float);
                    layerMixerInput.InputWeightField.SetParamInfo(inputWeightParam);
                    layerMixerInput.IsAdditive.value = layerMixerInputInfo.IsAdditive;
                    layerMixerInput.AvatarMask.value = layerMixerInputInfo.AvatarMask;
                }

                inputContainer.Insert(inputContainer.childCount - 1, layerMixerInput);
                InternalMixerInputs.Add(layerMixerInput);
            }

            for (var i = 0; i < InternalMixerInputs.Count; i++)
            {
                InternalMixerInputs[i].UpdateIndex(i, InternalMixerInputs.Count > 1);
            }
        }

        private void AddLayerMixerInput()
        {
            var layerMixerInput = new LayerMixerInput(DeleteMixerInput, _INPUT_LABEL_WIDTH);
            layerMixerInput.InputWeightField.OnValueChanged += OnInputWeightValueChanged;
            layerMixerInput.IsAdditive.RegisterValueChangedCallback(OnAdditiveValueChanged);
            layerMixerInput.AvatarMask.RegisterValueChangedCallback(OnAvatarMaskChanged);
            inputContainer.Insert(inputContainer.childCount - 1, layerMixerInput);
            InternalMixerInputs.Add(layerMixerInput);

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

        private void OnInputWeightValueChanged(ParamField<float> _)
        {
            RaiseNodeDataChangedEvent();
        }

        private void OnAdditiveValueChanged(ChangeEvent<bool> _)
        {
            RaiseNodeDataChangedEvent();
        }

        private void OnAvatarMaskChanged(ChangeEvent<UObject> _)
        {
            RaiseNodeDataChangedEvent();
        }

        #endregion


        #region Deep Clone

        protected override AnimationNodeData CreateCloneNodeDataInstance()
        {
            return new AnimationLayerMixerNodeData();
        }

        protected override void CloneNodeDataMembers(AnimationNodeData clone)
        {
            base.CloneNodeDataMembers(clone);

            var animLayerMixerNodeData = (AnimationLayerMixerNodeData)clone;
            var inputInfos = new List<InputInfo>();
            for (int i = 0; i < MixerInputs.Count; i++)
            {
                var inputInfo = MixerInputs[i].GetInputInfo();
                inputInfos.Add(inputInfo);
            }

            animLayerMixerNodeData.InputInfos = inputInfos.ToArray();
        }

        #endregion
    }
}
