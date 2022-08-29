using System;
using System.Collections.Generic;
using GBG.Puppeteer.Node;
using GBG.Puppeteer.NodeInstance;
using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer.Graph
{
    public class AnimationGraphInstance : IDisposable
    {
        public Playable RootPlayable { get; }


        private readonly AnimationNodeInstance _rootInstance;


        public AnimationGraphInstance(PlayableGraph graph, Animator animator, RuntimeAnimationGraph graphAsset)
        {
            // Record all params
            foreach (var param in graphAsset.Parameters)
            {
                if (!string.IsNullOrEmpty(param.Name))
                {
                    _params.Add(param.Name, param);
                }
            }

            // Record all nodes
            var nodeDict = new Dictionary<string, AnimationNode>(graphAsset.Nodes.Count);
            foreach (var node in graphAsset.Nodes)
            {
                nodeDict.Add(node.Guid, node);
            }

            // Initialize node graph
            var rootNode = graphAsset.Nodes[0];
            _rootInstance = rootNode.CreateNodeInstance(graph, animator, nodeDict, _params);
            RootPlayable = _rootInstance.Playable;
        }

        public void PrepareFrame(float deltaTime)
        {
            _rootInstance.PrepareFrame(deltaTime);
        }

        public void ProcessFrame(float deltaTime)
        {
            _rootInstance.ProcessFrame(deltaTime);
        }

        public void Dispose()
        {
            _rootInstance?.Dispose();
        }


        #region Params

        private readonly Dictionary<string, ParamInfo> _params = new Dictionary<string, ParamInfo>();


        public void SetFloat(string paramName, float paramValue)
        {
            _params[paramName].SetFloat(paramValue);
        }

        public bool TryGetFloat(string paramName, out float paramValue)
        {
            if (_params.TryGetValue(paramName, out var paramInfo) && paramInfo.Type == ParamType.Float)
            {
                paramValue = paramInfo.GetFloat();
                return true;
            }

            paramValue = default;
            return false;
        }

        public void SetInt(string paramName, int paramValue)
        {
            _params[paramName].SetInt(paramValue);
        }

        public bool TryGetInt(string paramName, out int paramValue)
        {
            if (_params.TryGetValue(paramName, out var paramInfo) && paramInfo.Type == ParamType.Int)
            {
                paramValue = paramInfo.GetInt();
                return true;
            }

            paramValue = default;
            return false;
        }

        public void SetBool(string paramName, bool paramValue)
        {
            _params[paramName].SetBool(paramValue);
        }

        public bool TryGetFloat(string paramName, out bool paramValue)
        {
            if (_params.TryGetValue(paramName, out var paramInfo) && paramInfo.Type == ParamType.Bool)
            {
                paramValue = paramInfo.GetBool();
                return true;
            }

            paramValue = default;
            return false;
        }

        #endregion
    }
}