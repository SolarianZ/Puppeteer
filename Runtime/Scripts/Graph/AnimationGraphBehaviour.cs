using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer.Graph
{
    /// <summary>
    /// Used for driving animation graph instance.
    /// </summary>
    public class AnimationGraphBehaviour : PlayableBehaviour
    {
        private PlayableGraph _graph;

        private AnimationGraphInstance _animationGraphInstance;


        public AnimationGraphInstance Initialize(Animator animator, RuntimeAnimationGraph graphAsset)
        {
            _animationGraphInstance = new AnimationGraphInstance(_graph, animator, graphAsset);
            return _animationGraphInstance;
        }


        #region Lifecycle

        public override void OnPlayableCreate(Playable playable)
        {
            _graph = playable.GetGraph();
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            _animationGraphInstance?.Dispose();
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            _animationGraphInstance.PrepareFrame(info.deltaTime);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            _animationGraphInstance.ProcessFrame(info.deltaTime);
        }

        #endregion
    }
}