using System;
using System.Collections.Generic;
using GBG.AnimationGraph.NodeData;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class MixerInputDataDrawer : VisualElement
    {
        public const float DRAWER_HEIGHT = 44;

        public event Action OnDataChanged;


        private readonly List<ParamInfo> _paramTable;

        private readonly TextField _inputNodeField;

        private readonly ParamField _inputWeightParamField;


        public MixerInputDataDrawer(List<ParamInfo> paramTable, Length nameLabelWidth)
        {
            _paramTable = paramTable;

            style.height = DRAWER_HEIGHT;
            style.justifyContent = Justify.SpaceAround;

            _inputNodeField = new TextField("Input Node");
            _inputNodeField.labelElement.style.minWidth = StyleKeyword.Auto;
            _inputNodeField.labelElement.style.maxWidth = StyleKeyword.Auto;
            _inputNodeField.labelElement.style.width = nameLabelWidth;
            _inputNodeField.labelElement.style.overflow = Overflow.Hidden;
            _inputNodeField.SetEnabled(false);
            Add(_inputNodeField);

            _inputWeightParamField = new ParamField(nameLabelWidth);
            _inputWeightParamField.OnParamChanged += _ => RaiseDataChangedEvent();
            Add(_inputWeightParamField);
        }

        public virtual void SetMixerInputData(MixerInputData mixerInputData, int mixerInputDataIndex)
        {
            _inputNodeField.label = $"Input Node {mixerInputDataIndex.ToString()}";
            _inputNodeField.SetValueWithoutNotify(mixerInputData.InputNodeGuid);

            _inputWeightParamField.SetParamTarget($"Input Weight {mixerInputDataIndex.ToString()}",
                mixerInputData.InputWeightParam, ParamType.Float, _paramTable,
                mixerInputData.InputWeightParam.IsValue ? ParamLinkState.Unlinked : ParamLinkState.Linked,
                ParamActiveState.ActiveLocked, new Vector2(0, 1));
        }


        protected void RaiseDataChangedEvent()
        {
            OnDataChanged?.Invoke();
        }
    }
}
