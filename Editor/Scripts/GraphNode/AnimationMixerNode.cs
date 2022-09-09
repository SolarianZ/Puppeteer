using System.Collections.Generic;
using GBG.Puppeteer.Editor.GraphParam;
using GBG.Puppeteer.NodeData;
using GBG.Puppeteer.Parameter;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.GraphNode
{
    public sealed class AnimationMixerNode : PlayableNode
    {
        protected override ParamField<float> PlaybackSpeedField { get; }

        private const float _INPUT_LABEL_WIDTH = 76;


        public AnimationMixerNode(string guid, List<ParamInfo> paramTable) : base(guid, paramTable)
        {
            // Add input button
            var addInputButton = new Button(AddMixerInput)
            {
                text = "Add Input Port"
            };
            inputContainer.Add(addInputButton);

            // Playback speed
            PlaybackSpeedField = new ParamField<float>("Speed", labelWidth: _INPUT_LABEL_WIDTH);
            PlaybackSpeedField.SetParamInfo(ParamInfo.CreateLiteral(ParamType.Float, 1));
            PlaybackSpeedField.OnValueChanged += OnPlaybackSpeedValueChanged;
            inputContainer.Insert(inputContainer.childCount - 1, PlaybackSpeedField);

            // Default mixer input
            AddMixerInput();

            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        public override void PopulateView(AnimationNodeData nodeData)
        {
            var animMixerNodeData = (AnimationMixerNodeData)nodeData;
            var paramTable = (List<ParamInfo>)ParamTable;

            // Playback speed
            PlaybackSpeedField.SetParamChoices(paramTable);
            PlaybackSpeedField.SetParamInfo(animMixerNodeData.PlaybackSpeed.GetParamInfo(paramTable, ParamType.Float));

            // Mixer inputs
            // Remove default mixer inputs
            foreach (var defaultMixerInput in InternalMixerInputs)
            {
                inputContainer.Remove(defaultMixerInput);
            }

            InternalMixerInputs.Clear();

            // Restore mixer inputs
            foreach (var inputInfo in animMixerNodeData.InputInfos)
            {
                var mixerInput = new MixerInput(DeleteMixerInput);
                var mixerInputInfo = (MixerInputInfo)inputInfo;
                if (mixerInputInfo != null)
                {
                    var inputWeightParam = mixerInputInfo.InputWeightParam.GetParamInfo(paramTable, ParamType.Float);
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

        private void AddMixerInput()
        {
            var mixerInput = new MixerInput(DeleteMixerInput, _INPUT_LABEL_WIDTH);
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

        private void OnInputWeightValueChanged(ParamField<float> _)
        {
            RaiseNodeDataChangedEvent();
        }

        private void OnPlaybackSpeedValueChanged(ParamField<float> _)
        {
            RaiseNodeDataChangedEvent();
        }

        #endregion


        #region Deep Clone

        protected override AnimationNodeData CreateCloneNodeDataInstance()
        {
            return new AnimationMixerNodeData();
        }

        protected override void CloneNodeDataMembers(AnimationNodeData clone)
        {
            base.CloneNodeDataMembers(clone);

            var animMixerNodeData = (AnimationMixerNodeData)clone;
            var inputInfos = new List<InputInfo>();
            for (int i = 0; i < MixerInputs.Count; i++)
            {
                var inputInfo = MixerInputs[i].GetInputInfo();
                inputInfos.Add(inputInfo);
            }

            animMixerNodeData.InputInfos = inputInfos.ToArray();
        }

        #endregion
    }
}
