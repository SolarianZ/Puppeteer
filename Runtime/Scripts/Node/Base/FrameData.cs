using UFrameData = UnityEngine.Playables.FrameData;

namespace GBG.AnimationGraph.Node
{
    public readonly struct FrameData
    {
        public readonly ulong FrameId;
        public readonly float DeltaTime;
        public readonly float Weight;

        public readonly float EffectiveWeight;
        // public readonly float EffectiveParentSpeed;
        // public readonly float EffectiveSpeed;


        public FrameData(in FrameData parentData, float weight /*, float speed=1*/)
        {
            FrameId = parentData.FrameId;
            DeltaTime = parentData.DeltaTime;
            Weight = weight;
            EffectiveWeight = parentData.EffectiveWeight * weight;
            // EffectiveParentSpeed = parentData.EffectiveSpeed;
            // EffectiveSpeed = parentData.EffectiveSpeed * speed;
        }

        public FrameData(UFrameData frameData)
        {
            FrameId = frameData.frameId;
            DeltaTime = frameData.deltaTime;
            Weight = frameData.weight;
            EffectiveWeight = frameData.effectiveWeight;
            // EffectiveParentSpeed = frameData.effectiveParentSpeed;
            // EffectiveSpeed = frameData.effectiveSpeed;
        }


        public static implicit operator FrameData(UFrameData frameData)
        {
            return new FrameData(frameData);
        }
    }
}
