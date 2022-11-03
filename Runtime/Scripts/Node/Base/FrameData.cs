using UnityEngine.Playables;
using UFrameData = UnityEngine.Playables.FrameData;

namespace GBG.AnimationGraph.Node
{
    public struct FrameData
    {
        public ulong FrameId;
        public float DeltaTime;
        public float Weight;
        public float EffectiveWeight;
        public float EffectiveParentSpeed;
        public float EffectiveSpeed;
        public PlayState EffectivePlayState;

        public static implicit operator FrameData(UFrameData frameData)
        {
            return new FrameData
            {
                FrameId = frameData.frameId,
                DeltaTime = frameData.deltaTime,
                Weight = frameData.weight,
                EffectiveWeight = frameData.effectiveWeight,
                EffectiveParentSpeed = frameData.effectiveParentSpeed,
                EffectiveSpeed = frameData.effectiveSpeed,
                EffectivePlayState = frameData.effectivePlayState,
            };
        }
    }
}
