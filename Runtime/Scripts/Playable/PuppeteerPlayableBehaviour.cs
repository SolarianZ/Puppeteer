using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;
using UnityEngine.Playables;

namespace GBG.Puppeteer
{
    public partial class PuppeteerPlayableBehaviour : PlayableBehaviour
    {
        private const string _logPerfix = "[Puppeteer]";

        private Animator _animator;

        private PlayableGraph _graph;

        private AnimationLayerMixerPlayable _layerMixerPlayable;


        public void Initialize(PlayableGraph graph, Animator animator)
        {
            Assert.IsTrue(graph.IsValid());

            _animator = animator;
            _graph = graph;
            _layerMixerPlayable = AnimationLayerMixerPlayable.Create(_graph);

            var animOutput = AnimationPlayableOutput.Create(_graph, "PuppeteerAnimationOutput", _animator);
            animOutput.SetSourcePlayable(_layerMixerPlayable);
        }


        #region Playable Behaviour Callbacks

        // https://docs.unity3d.com/ScriptReference/Playables.Playable.html
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);

            for (int i = 0; i < _activeCrossFades.Count; i++)
            {
                var crossFadeInfo = _activeCrossFades[i];
                crossFadeInfo.Evaluate(info.deltaTime);
                if (crossFadeInfo.IsDone())
                {
                    _activeCrossFades.RemoveAt(i--);

                    SwapInputs(crossFadeInfo.Receiver, 1, 0);
                    if (crossFadeInfo.From.IsValid())
                    {
                        crossFadeInfo.From.Destroy();
                    }
                }
                else
                {
                    _activeCrossFades[i] = crossFadeInfo;
                }
            }
        }

        #endregion
    }
}
