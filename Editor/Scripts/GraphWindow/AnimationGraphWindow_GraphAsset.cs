using GBG.Puppeteer.Editor.GraphView;
using GBG.Puppeteer.Graph;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.GraphWindow
{
    public partial class AnimationGraphWindow
    {
        private void OnGraphGeometryChanged(GeometryChangedEvent evt)
        {
            _graphView.UnregisterCallback<GeometryChangedEvent>(OnGraphGeometryChanged);
            _graphView.FrameAll();
        }


        private RuntimeAnimationGraph _graphAsset;

        private void SetAsset(RuntimeAnimationGraph graphAsset)
        {
            Assert.IsNotNull(graphAsset);
            Assert.IsNull(_graphView);

            _graphAsset = graphAsset;
            _graphView = new AnimationGraphView(_graphAsset)
            {
                style =
                {
                    width = Length.Percent(100),
                    height = Length.Percent(100),
                }
            };
            _graphView.RegisterCallback<GeometryChangedEvent>(OnGraphGeometryChanged);
            _graphView.OnGraphChanged += OnGraphChanged;

            rootVisualElement.Add(_graphView);
        }


        public override void SaveChanges()
        {
            base.SaveChanges();

            _graphView.SaveToGraphAsset(_graphAsset);
            EditorUtility.SetDirty(_graphAsset);
            AssetDatabase.SaveAssetIfDirty(_graphAsset);
        }

        
        private void OnGraphChanged()
        {
            hasUnsavedChanges = true;
        }

        private void OnProjectChange()
        {
            if (!_graphAsset)
            {
                hasUnsavedChanges = false;
                Close();
                return;
            }

            titleContent = new GUIContent(_graphAsset.name);
        }
    }
}
