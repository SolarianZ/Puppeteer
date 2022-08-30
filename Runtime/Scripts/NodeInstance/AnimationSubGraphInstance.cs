using GBG.Puppeteer.Graph;
using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeInstance
{
    public class AnimationSubGraphInstance : AnimationNodeInstance
    {
        public override Playable Playable { get; }


        private readonly AnimationGraphInstance _animationGraphInstance;


        public AnimationSubGraphInstance(PlayableGraph graph, Animator animator,
            RuntimeAnimationGraph subGraph, ParamInfo[] paramBindingSources)
        {
            _animationGraphInstance = new AnimationGraphInstance(graph, animator, subGraph, paramBindingSources);
            Playable = _animationGraphInstance.RootPlayable;
        }

        public override void PrepareFrame(float deltaTime)
        {
            base.PrepareFrame(deltaTime);

            _animationGraphInstance.PrepareFrame(deltaTime);
        }

        public override void Dispose()
        {
            _animationGraphInstance.Dispose();

            base.Dispose();
        }
    }
}