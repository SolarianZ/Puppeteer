using UnityEditor;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    public partial class AnimationGraphEditorWindow
    {
        private void SaveAsset()
        {
            // todo save
            // nodes
            _asset.Nodes.Clear();
            foreach (var node in _graphView.nodes)
            {
                var nodeData = ((AnimationGraphNode)node).NodeData;
                nodeData.Position = node.GetPosition().position;
                nodeData.SortPorts();
                _asset.Nodes.Add(nodeData);
            }

            // ports
            _asset.Ports.Clear();
            foreach (var port in _graphView.ports)
            {
                //port.ConnectTo
            }

            // edges
            _asset.Edges.Clear();
            foreach (var edge in _graphView.edges)
            {
                if (edge.input == null || edge.output == null)
                {
                    continue;
                }

                var fromNode = (AnimationGraphNode)edge.output.node;
                var toNode = (AnimationGraphNode)edge.input.node;

                var edgeData = new EdgeData
                {
                    FromNodeGuid = fromNode.NodeData.Guid,
                    FromPortGuid = ((PortData)edge.output.userData).Guid,
                    ToNodeGuid = toNode.NodeData.Guid,
                    ToPortGuid = ((PortData)edge.input.userData).Guid
                };
                _asset.Edges.Add(edgeData);
            }


            EditorUtility.SetDirty(_asset);
            AssetDatabase.SaveAssetIfDirty(_asset);

            hasUnsavedChanges = false;
        }


        public override void SaveChanges()
        {
            SaveAsset();
            base.SaveChanges();
        }

        //public override void DiscardChanges()
        //{
        //    base.DiscardChanges();
        //}
    }
}