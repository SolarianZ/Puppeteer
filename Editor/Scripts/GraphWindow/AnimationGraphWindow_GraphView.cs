using GBG.Puppeteer.Editor.GraphView;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.GraphWindow
{
    public partial class AnimationGraphWindow
    {
        private AnimationGraphView _graphView;


        private void CreateGraphViewPanel()
        {
        }

        private void BuildGraph()
        {
            _graphView = new AnimationGraphView
            {
                style =
                {
                    width = Length.Percent(100),
                    height = Length.Percent(100),
                }
            };
            _graphView.RegisterCallback<GeometryChangedEvent>(OnGraphGeometryChanged);
            _graphView.OnGraphChanged += OnGraphChanged;
            _graphView.SuppressGraphViewChangedEvent = true;
            _graphView.RebuildGraph(_graphAsset.EditorNodes, _graphAsset.EditorIsolatedNodes, _paramInfos);
            _graphView.SuppressGraphViewChangedEvent = false;
            _layoutContainer.MiddlePane.Add(_graphView);
        }
        
        private void OnGraphChanged()
        {
            hasUnsavedChanges = true;
        }

        private void OnGraphGeometryChanged(GeometryChangedEvent evt)
        {
            _graphView.UnregisterCallback<GeometryChangedEvent>(OnGraphGeometryChanged);
            _graphView.FrameAll();
        }
    }
}
