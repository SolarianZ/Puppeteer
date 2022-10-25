using System;
using System.Collections.Generic;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public abstract class PlayableNodeBase : NodeBase
    {
        #region Serialization Data

        #endregion


        #region Non Serializable Input Data

        protected static string[] EmptyInputs = Array.Empty<string>();

        public abstract IList<string> GetInputNodeGuids();

        #endregion


        #region Runtime Properties

        // internal abstract Playable AnimationPlayable { get; }

        #endregion


        protected PlayableNodeBase(string guid) : base(guid)
        {
        }
    }
}
