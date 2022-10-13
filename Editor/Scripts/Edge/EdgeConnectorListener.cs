using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UGraphView = UnityEditor.Experimental.GraphView.GraphView;
using UEdge = UnityEditor.Experimental.GraphView.Edge;
using UPort = UnityEditor.Experimental.GraphView.Port;

// ReSharper disable All

namespace GBG.AnimationGraph.Editor.GraphEdge
{
    // Upstream: https://github.com/Unity-Technologies/UnityCsReference/blob/2021.3/Modules/GraphViewEditor/Elements/Port.cs
    // Unity C# reference source
    // Copyright (c) Unity Technologies. For terms of use, see
    // https://unity3d.com/legal/licenses/Unity_Reference_Only_License
    public class EdgeConnectorListener : IEdgeConnectorListener
    {
        private GraphViewChange m_GraphViewChange;
        private List<UEdge> m_EdgesToCreate;
        private List<GraphElement> m_EdgesToDelete;

        public EdgeConnectorListener()
        {
            m_EdgesToCreate = new List<UEdge>();
            m_EdgesToDelete = new List<GraphElement>();

            m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
        }

        public void OnDropOutsidePort(UEdge edge, Vector2 position)
        {
        }

        public void OnDrop(UGraphView graphView, UEdge edge)
        {
            m_EdgesToCreate.Clear();
            m_EdgesToCreate.Add(edge);

            // We can't just add these edges to delete to the m_GraphViewChange
            // because we want the proper deletion code in GraphView to also
            // be called. Of course, that code (in DeleteElements) also
            // sends a GraphViewChange.
            m_EdgesToDelete.Clear();
            if (edge.input.capacity == UPort.Capacity.Single)
                foreach (UEdge edgeToDelete in edge.input.connections)
                    if (edgeToDelete != edge)
                        m_EdgesToDelete.Add(edgeToDelete);
            if (edge.output.capacity == UPort.Capacity.Single)
                foreach (UEdge edgeToDelete in edge.output.connections)
                    if (edgeToDelete != edge)
                        m_EdgesToDelete.Add(edgeToDelete);
            if (m_EdgesToDelete.Count > 0)
                graphView.DeleteElements(m_EdgesToDelete);

            var edgesToCreate = m_EdgesToCreate;
            if (graphView.graphViewChanged != null)
            {
                edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
            }

            foreach (UEdge e in edgesToCreate)
            {
                graphView.AddElement(e);
                edge.input.Connect(e);
                edge.output.Connect(e);
            }
        }
    }
}
