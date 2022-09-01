using GBG.Puppeteer.Graph;
using GBG.Puppeteer.Parameter;
using UnityEditor;
using UnityEngine.Assertions;

namespace GBG.Puppeteer.Editor.GraphWindow
{
    public partial class AnimationGraphWindow
    {
        private RuntimeAnimationGraph _graphAsset;


        public override void SaveChanges()
        {
            // Parameters
            _graphAsset.EditorParameters = new ParamInfo[_paramInfos.Count];
            for (int i = 0; i < _paramInfos.Count; i++)
            {
                _graphAsset.EditorParameters[i] = (ParamInfo)_paramInfos[i].Clone();
            }

            // Nodes
            _graphView.SaveNodesToGraphAsset(_graphAsset);

            // Save asset
            EditorUtility.SetDirty(_graphAsset);
            AssetDatabase.SaveAssetIfDirty(_graphAsset);

            base.SaveChanges();
        }


        private void SetAsset(RuntimeAnimationGraph graphAsset)
        {
            _graphAsset = graphAsset;
            Assert.IsNull(_graphView);

            // Parameter
            _paramInfos.Clear();
            foreach (var param in _graphAsset.Parameters)
            {
                _paramInfos.Add((ParamInfo)param.Clone());
            }

            // Graph
            BuildGraph();
        }
    }
}
