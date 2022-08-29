using GBG.Puppeteer.Graph;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeInstance
{
    public class AnimationSubGraphInstance : AnimationNodeInstance
    {
        public override Playable Playable { get; }


        private readonly RuntimeAnimationGraph _subGraph;

        private readonly AnimationGraphInstance _animationGraphInstance;


        public AnimationSubGraphInstance(PlayableGraph graph, Animator animator,
            RuntimeAnimationGraph subGraph)
        {
            _animationGraphInstance = new AnimationGraphInstance(graph, animator, subGraph);
            Playable = _animationGraphInstance.RootPlayable;
        }

        public override void Dispose()
        {
            _animationGraphInstance.Dispose();
            
            base.Dispose();
        }
    }
}