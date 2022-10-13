namespace GBG.AnimationGraph
{
    public class AnimationGraphBrain
    {
    }
}

/*************************************************************************************************************
 * For the same type playable trees, they're processed in the same order that they were added.
 * 
 *     Unity always processes **ScriptPlayable** trees after `Update` message.
 * 
 *     When `Animator.updateMode==AnimatorUpdateMode.AnimatePhysics` , 
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
