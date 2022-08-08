using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer
{
    public class PuppeteerPlayableBehaviour : PlayableBehaviour
    {
        private bool _isGraphDirty = true;


        private void RebuildGraph()
        {

        }


        public void Play(string layerName, string stateName,
            float timeOffset = 0f, TimeMode timeMode = TimeMode.NormalizedTime)
        {

        }

        public void CrossFadeState(string layerName, string targetStateName, float fadeTime,
            float timeOffset = 0f, TimeMode timeMode = TimeMode.NormalizedTime)
        {

        }


        #region Layer

        public byte LayerNameToIndex(string layerName)
        {
            return 0;
        }

        public bool HasLayer(string layerName)
        {
            return false;
        }

        public int AddLayer(string layerName, AvatarMask avatarMask = null,
            AnimationBlendMode blendMode = AnimationBlendMode.Blend, float weight = 0f) // ik?
        {
            return -1;
        }

        public bool RemoveLayer(string layerName)
        {
            return false;
        }

        public void SetLayerWeight(string layerName, float weight) { }

        public void SetLayerAvatarMask(string layerName, AvatarMask avatarMask) { }

        #endregion


        #region State

        public bool HasState(string layerName, string stateName)
        {
            return false;
        }

        public void AddState(string layerName, string stateName, AnimationClip animClip)
        {
        }

        //public void AddBlendTree1DState(string layerName, string stateName, BlendTree1DInfo)
        //{
        //    // BlendTree1DInfo: AnimationClip - blendParam threshold
        //    throw new System.NotImplementedException();
        //}

        //public void AddBlendTree2DState(string layerName, string stateName, BlendTree2DInfo)
        //{
        //    throw new System.NotImplementedException();
        //}

        public bool RemoveState(string layerName, string stateName)
        {
            return false;
        }

        public void SetStateWeight(string layerName, string stateName, float weight)
        {

        }

        public void SetStatePlaybackSpeed(string layerName, string stateName, float speed)
        {

        }

        #endregion


        #region Playable Behaviour Callbacks

        // https://docs.unity3d.com/ScriptReference/Playables.Playable.html
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);

            // todo build playable graph hierarchy
        }

        // may not use this method
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);
        }

        #endregion
    }
}
