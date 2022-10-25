using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Editor.Node;
using GBG.AnimationGraph.Node;
using GBG.AnimationGraph.Parameter;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public class AnimationLayerMixerNodeInspector : AnimationMixerNodeInspector
    {
        public AnimationLayerMixerNodeInspector(List<ParamInfo> paramTable, Action<int> addInputPortElement,
            Action<int> removeInputPortElement, Action<int, int> reorderInputPortElement,
            float inputDrawerHeight = LayeredNodeInputDrawer.DRAWER_HEIGHT)
            : base(paramTable, addInputPortElement, removeInputPortElement, reorderInputPortElement, inputDrawerHeight)
        {
        }


        protected override List<WeightedNodeInput> GetMixerInputs()
        {
            return ((AnimationLayerMixerEditorNode)Target).Node.MixerInputs;
        }

        protected override WeightedNodeInputDrawer CreateMixerInputDataDrawer()
        {
            return new LayeredNodeInputDrawer(ParamTable, FieldLabelWidth);
        }

        protected override WeightedNodeInput CreateMixerInputData()
        {
            return new LayeredNodeInput();
        }
    }
}
