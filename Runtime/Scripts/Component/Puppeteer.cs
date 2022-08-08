using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.Puppeteer
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class Puppeteer : MonoBehaviour
    {
        [SerializeField]
        private DirectorUpdateMode _updateMode = DirectorUpdateMode.GameTime;

        //[SerializeField]
        //private bool _applyRootMotion = true;

        private Animator _animator;

        private PlayableGraph _graph;

        private AnimationLayerMixerPlayable _layerMixerPlayable;

        private bool _isInitialized;


        private void OnEnable()
        {
            Initialize();

            _graph.Play();
        }

        private void OnDisable()
        {
            _graph.Stop();
        }

        private void OnDestroy()
        {
            if (_graph.IsValid())
            {
                _graph.Destroy();
            }
        }


        private void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _animator = GetComponent<Animator>();

            _graph = PlayableGraph.Create(nameof(Puppeteer));
            _graph.SetTimeUpdateMode(_updateMode);

            _layerMixerPlayable = AnimationLayerMixerPlayable.Create(_graph);

            var animOutput = AnimationPlayableOutput.Create(_graph, "PuppeteerAnimationOutput", _animator);
            animOutput.SetSourcePlayable(_layerMixerPlayable);

            _isInitialized = true;
        }


        public void ManualUpdate(float deltaTime)
        {
            _graph.Evaluate(deltaTime);
        }
    }
}
