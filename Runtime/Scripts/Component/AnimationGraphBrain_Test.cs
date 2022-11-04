#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;

namespace GBG.AnimationGraph
{
    public partial class AnimationGraphBrain
    {
        internal IReadOnlyList<ParamInfo> EditorTest_GetParameters()
        {
            if (!_graphAsset)
            {
                return Array.Empty<ParamInfo>();
            }

            return _graphAsset.Parameters;
        }
    }
}
#endif
