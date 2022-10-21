using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.AnimationGraph
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class AnimationGraphBrain : MonoBehaviour
    {
        [SerializeField]
        private AnimationGraphAsset _graphAsset;

        private Animator _animator;

        private PlayableGraph _playableGraph;

        private AnimationGraphDriver _driver;

        private readonly Dictionary<string, Playable> _animationPlayableTable = new();


        #region Mono Messages

        private void OnEnable()
        {
            if (!_playableGraph.IsValid())
            {
                _animator = GetComponent<Animator>();
                _playableGraph = PlayableGraph.Create($"{name}.{nameof(AnimationGraphBrain)}");

                var driverPlayable = ScriptPlayable<AnimationGraphDriver>.Create(_playableGraph);
                _driver = driverPlayable.GetBehaviour();
                var scriptOutput = ScriptPlayableOutput.Create(_playableGraph, "Script Output");
                scriptOutput.SetSourcePlayable(driverPlayable);

                BuildAnimationPlayableGraph();
            }

            _playableGraph.Play();
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
                _playableGraph.Destroy();
            }
        }

        #endregion


        private void BuildAnimationPlayableGraph()
        {
            var animOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation Output", _animator);
            if (!_graphAsset)
            {
                return;
            }

            var rootGraphData = _graphAsset.Graphs.First(g => g.Guid.Equals(_graphAsset.RootGraphGuid));
            // animOutput.SetSourcePlayable();
        }
    }

    public class AnimationGraphDriver : PlayableBehaviour
    {
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);
        }
    }
}

/*************************************************************************************************************
 * For the same type playable trees, they're processed in the same order that they were added.
 * 
 * Unity always processes **ScriptPlayable** trees after `Update` message.
 * 
 * When `Animator.updateMode==AnimatorUpdateMode.AnimatePhysics` , 
 * Unity processes **AnimationScriptPlayable** trees after `FixedUpdate` message and before `Update` message, 
 * otherwise Unity processes **AnimationScriptPlayable** trees after all **ScriptPlayable** trees.
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
