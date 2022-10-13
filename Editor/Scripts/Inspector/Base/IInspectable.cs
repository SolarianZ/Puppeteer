using UnityEditor.Experimental.GraphView;

namespace GBG.AnimationGraph.Editor.Inspector
{
    public interface IInspectable<in TTarget> where TTarget : GraphElement, IInspectable<TTarget>
    {
        public string Guid { get; }

        public string TypeName => GetType().Name;


        public IInspector<TTarget> GetInspector();
    }
}
