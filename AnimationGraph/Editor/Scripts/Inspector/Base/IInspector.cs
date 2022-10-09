using System;
using UnityEditor.Experimental.GraphView;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public interface IInspector
    {
        // TODO: Add DataCategories parameter
        public event Action OnParamChanged;
    }

    public interface IInspector<in TTarget> : IInspector where TTarget : GraphElement, IInspectable<TTarget>
    {
        public void SetTarget(TTarget target);
    }
}
