using System;
using System.Collections.Generic;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public abstract class MixerNodeBase : NodeBase
    {
        #region Serialization Data

        #endregion


        #region Runtime Properties

        protected static string[] EmptyInputs = Array.Empty<string>();

        // internal abstract Playable Playable { get; }

        #endregion


        protected MixerNodeBase(string guid) : base(guid)
        {
        }


        public abstract IList<string> GetInputNodeGuids();
    }
}
