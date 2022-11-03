using System;
using UnityEngine.Playables;

namespace GBG.AnimationGraph
{
    internal static class PlayableExtensions
    {
        #region Play & Pause Mask

        /** Why mask Play & Pause?
         * When the following events occur, Unity will reset animation Playables to Playing state:
         *  1. When binding a Playable to AnimationPlayableOutput(call 'SetSourcePlayable' method);
         *  2. When Animator's renderer become visible or invisible and its cullingMode is not 'AlwaysAnimate' .
         * So we disabled these relevant methods and given alternative implementations.
         * See: https://issuetracker.unity3d.com/issues/paused-playable-starts-playing-when-a-gameobjects-visible-state-is-changed-and-animator-syncplaystatetoculling-is-called
         */
        [Obsolete("Please use NodeBase.Play() instead.", true)]
        public static void Play(this Playable playable)
        {
        }

        [Obsolete("Please use NodeBase.Pause() instead.", true)]
        public static void Pause(this Playable playable)
        {
        }

        #endregion

        [Obsolete("Please use NodeBase.SetSpeed() instead.", true)]
        public static void SetSpeed(this Playable playable, double value)
        {
        }
    }
}
