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


        public FrameData(in FrameData parentFrameData, in float weight, in float speed)
        {
            FrameId = parentFrameData.FrameId;
            DeltaTime = parentFrameData.DeltaTime;
            Weight = parentFrameData.Weight;
            EffectiveWeight = parentFrameData.EffectiveWeight * weight;
            EffectiveParentSpeed = parentFrameData.EffectiveSpeed;
            EffectiveSpeed = parentFrameData.EffectiveSpeed * speed;
        }


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
            };
        }
    }
}
