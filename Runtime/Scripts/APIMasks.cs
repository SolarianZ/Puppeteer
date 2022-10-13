namespace UnityEngine.Playables
{
    /** Why API Masks?
     * When the following events occur, Unity will reset animation Playables to Playing state:
     *  1. When binding a Playable to AnimationPlayableOutput(call 'SetSourcePlayable' method);
     *  2. When Animator's renderer become visible or invisible and its cullingMode is not 'AlwaysAnimate' .
     * So we disabled these relevant methods and given alternative implementations.
     */
    public static class PlayableExtensions
    {
        [System.Obsolete("Please use AnimationNodeInstance.Play() instead.", true)]
        public static void Play(this Playable playable)
        {
        }

        [System.Obsolete("Please use AnimationNodeInstance.Pause() instead.", true)]
        public static void Pause(this Playable playable)
        {
        }

        [System.Obsolete("Please use AnimationGraphInstance.SetFloat() instead.", true)]
        public static void SetSpeed(this Playable playable, double value)
        {
        }
    }
}
