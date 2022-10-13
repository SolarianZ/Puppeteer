using System;
using GBG.AnimationGraph.Editor.GraphEditor;
using UnityEditor.Experimental.GraphView;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public interface IInspector
    {
        public event Action<DataCategories> OnDataChanged;
    }

    public interface IInspector<in TTarget> : IInspector where TTarget : GraphElement, IInspectable<TTarget>
    {
        public void SetTarget(TTarget target);
    }
}
