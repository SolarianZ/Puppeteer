using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Graph;
using GBG.AnimationGraph.Node;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using FrameData = GBG.AnimationGraph.Node.FrameData;
using UFrameData = UnityEngine.Playables.FrameData;

namespace GBG.AnimationGraph
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public partial class AnimationGraphBrain : MonoBehaviour
    {
        #region Serialization Data

        [SerializeField]
        private AnimationGraphAsset _graphAsset;

        #endregion


        #region Runtime Properties

        public bool IsValid => _playableGraph.IsValid();

        public bool IsPlaying => IsValid && _playableGraph.IsPlaying();

        public bool IsDone => IsValid && _playableGraph.IsDone();


        private Animator _animator;

        private PlayableGraph _playableGraph;

        private Dictionary<string, GraphLayer> _graphGuidTable;

        private NodeBase _rootNode;

        #endregion


        #region Graph Control

        public void Evaluate(float deltaTime)
        {
            _playableGraph.Evaluate(deltaTime);
        }

        public void Play()
        {
            _playableGraph.Play();
        }

        public void Stop()
        {
            _playableGraph.Stop();
        }

        #endregion


        #region Mono Messages

        private void OnEnable()
        {
            if (!_playableGraph.IsValid() && _graphAsset)
            {
                _animator = GetComponent<Animator>();
                _graphGuidTable = new Dictionary<string, GraphLayer>(_graphAsset.GraphLayers.Count);
                _playableGraph = PlayableGraph.Create($"{name}.{nameof(AnimationGraphBrain)}");
                _graphAsset = Instantiate(_graphAsset);
                _graphAsset.Initialize(_animator, _playableGraph, _graphGuidTable);

                // State driver
                var driverPlayable = ScriptPlayable<AnimationGraphStateDriver>.Create(_playableGraph);
                driverPlayable.GetBehaviour().Initialize(frameData => _rootNode?.PrepareFrame(frameData));
                var scriptOutput = ScriptPlayableOutput.Create(_playableGraph, "Script Output");
                scriptOutput.SetSourcePlayable(driverPlayable);

                // Animation
                var animOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation Output", _animator);
                _rootNode = _graphAsset.RuntimeRootNode;
                if (_rootNode != null)
                {
                    animOutput.SetSourcePlayable(_rootNode.Playable);
                }

                Evaluate(0);
            }

            Play();
        }

        private void OnDisable()
        {
            if (_playableGraph.IsValid())
            {
                _playableGraph.Stop();
            }
        }

        private void OnDestroy()
        {
            if (_playableGraph.IsValid())
            {
                foreach (var graph in _graphGuidTable.Values)
                {
                    graph.Dispose();
                }

                _playableGraph.Destroy();
                Destroy(_graphAsset);
            }
        }

        #endregion


        class AnimationGraphStateDriver : PlayableBehaviour
        {
            private Action<FrameData> _onPrepareFrame;


            public void Initialize(Action<FrameData> onPrepareFrame)
            {
                _onPrepareFrame = onPrepareFrame;
            }

            public override void PrepareFrame(Playable playable, UFrameData info)
            {
                base.PrepareFrame(playable, info);

                _onPrepareFrame(info);
            }
        }
    }
}

/*************************************************************************************************************
 * Unity always processes **ScriptPlayableOutput** trees after `Update` message.
 * 
 * When `Animator.updateMode==AnimatorUpdateMode.AnimatePhysics` , 
 * Unity processes **AnimationPlayableOutput** tree after `FixedUpdate` message and before `Update` message, 
 * otherwise Unity processes **AnimationPlayableOutput** tree after all **ScriptPlayableOutput** trees.
 *
 * Nodes in same tree will be processed in the same order as they were added.
 * If a ScriptPlayable node is NOT in **ScriptPlayableOutput** hierarchy,
 * its **ProcessFrame** method won't be called.
 * 
 * ***********************************************************************************************************
 * 
 * Tree nodes traversal order:
 * 
 * **[On Create]** Invoked by preorder traversal:
 *  - ForEach: OnPlayableCreate
 *  - ForEach: OnGraphStart -> OnBehaviourPlay
 * 
 * **[Every Frame]** Invoked by preorder traversal:
 *  - ForEach: PrepareFrame
 * 
 * **[Every Frame]** Invoked by postorder traversal
 *  - ForEach: ProcessFrame
 *  - ForEach: ProcessRootMotion
 *  - ForEach: ProcessAnimation
 * 
 * **[On Destroy]** Invoked by preorder traversal:
 *  - ForEach: OnBehaviourPause
 *  - ForEach: OnGraphStop -> OnPlayableDestroy
 ************************************************************************************************************/
