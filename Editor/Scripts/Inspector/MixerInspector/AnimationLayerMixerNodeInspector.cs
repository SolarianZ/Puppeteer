using System;
using System.Collections.Generic;
using GBG.AnimationGraph.NodeData;
using GBG.AnimationGraph.Parameter;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class AnimationLayerMixerNodeInspector : AnimationMixerNodeInspector
    {
        public AnimationLayerMixerNodeInspector(List<ParamInfo> paramTable, Action<int> addInputPortElement,
            Action<int> removeInputPortElement, Action<int, int> reorderInputPortElement,
            float inputDrawerHeight = LayerMixerInputDataDrawer.DRAWER_HEIGHT)
            : base(paramTable, addInputPortElement, removeInputPortElement, reorderInputPortElement, inputDrawerHeight)
        {
        }


        protected override MixerInputDataDrawer CreateMixerInputDataDrawer()
        {
            return new LayerMixerInputDataDrawer(ParamTable, FieldLabelWidth);
        }

        protected override MixerInputData CreateMixerInputData()
        {
            return new LayerMixerInputData();
        }
    }
}
