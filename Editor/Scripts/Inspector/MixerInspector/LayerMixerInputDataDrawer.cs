using System.Collections.Generic;
using GBG.AnimationGraph.NodeData;
using GBG.AnimationGraph.Parameter;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class LayerMixerInputDataDrawer : MixerInputDataDrawer
    {
        public new const float DRAWER_HEIGHT = 88;

        private readonly Toggle _isAdditiveField;

        private readonly ObjectField _avatarMaskField;

        private LayerMixerInputData _layerMixerInputData;


        public LayerMixerInputDataDrawer(List<ParamInfo> paramTable, Length nameLabelWidth)
            : base(paramTable, nameLabelWidth)
        {
            style.height = DRAWER_HEIGHT;

            _isAdditiveField = new Toggle("Is Additive");
            _isAdditiveField.labelElement.style.minWidth = StyleKeyword.Auto;
            _isAdditiveField.labelElement.style.maxWidth = StyleKeyword.Auto;
            _isAdditiveField.labelElement.style.width = nameLabelWidth;
            _isAdditiveField.labelElement.style.overflow = Overflow.Hidden;
            _isAdditiveField.RegisterValueChangedCallback(OnAdditiveChanged);
            Add(_isAdditiveField);

            _avatarMaskField = new ObjectField("Avatar Mask")
            {
                objectType = typeof(AvatarMask),
            };
            _avatarMaskField.labelElement.style.minWidth = StyleKeyword.Auto;
            _avatarMaskField.labelElement.style.maxWidth = StyleKeyword.Auto;
            _avatarMaskField.labelElement.style.width = nameLabelWidth;
            _avatarMaskField.labelElement.style.overflow = Overflow.Hidden;
            _avatarMaskField.RegisterValueChangedCallback(OnAvatarMaskChanged);
            Add(_avatarMaskField);
        }

        public override void SetMixerInputData(MixerInputData mixerInputData, int mixerInputDataIndex)
        {
            base.SetMixerInputData(mixerInputData, mixerInputDataIndex);

            _layerMixerInputData = (LayerMixerInputData)mixerInputData;

            _isAdditiveField.SetValueWithoutNotify(_layerMixerInputData.IsAdditive);

            _avatarMaskField.SetValueWithoutNotify(_layerMixerInputData.AvatarMask);
        }


        private void OnAdditiveChanged(ChangeEvent<bool> evt)
        {
            if (_layerMixerInputData != null)
            {
                _layerMixerInputData.IsAdditive = evt.newValue;
                RaiseDataChangedEvent();
            }
        }

        private void OnAvatarMaskChanged(ChangeEvent<Object> evt)
        {
            if (_layerMixerInputData != null)
            {
                _layerMixerInputData.AvatarMask = (AvatarMask)evt.newValue;
                RaiseDataChangedEvent();
            }
        }
    }
}
