using System;
using GBG.AnimationGraph.Component;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    public struct CustomMixerJob : IAnimationJob, IDisposable
    {
        public NativeArray<BoneInfo> BoneInfos;

        public NativeArray<float> InputWeights;

        private const float _EPSILON = 1E-5f;


        public CustomMixerJob(NativeArray<BoneInfo> boneInfos, NativeArray<float> inputWeights)
        {
            BoneInfos = boneInfos;
            InputWeights = inputWeights;
        }

        public void ProcessRootMotion(AnimationStream stream)
        {
            var inputCount = stream.inputStreamCount;
            if (inputCount == 0) return;
            Assert.AreEqual(inputCount, InputWeights.Length);

            var velocity = Vector3.zero;
            var angularVelocity = Vector3.zero;
            var weightAccum = 0f;
            for (int i = 0; i < inputCount; i++)
            {
                Assert.IsTrue(stream.GetInputWeight(i) > 1 - _EPSILON && stream.GetInputWeight(i) < 1 + _EPSILON);

                var inputWeight = InputWeights[i];
                if (inputWeight < _EPSILON) continue;

                var inputStream = stream.GetInputStream(i);
                weightAccum += inputWeight;
                var weight = inputWeight / weightAccum;
                velocity = Vector3.Lerp(velocity, inputStream.velocity, weight);
                angularVelocity = Vector3.Lerp(angularVelocity, inputStream.angularVelocity, weight);
            }

            stream.velocity = velocity;
            stream.angularVelocity = angularVelocity;
        }

        public void ProcessAnimation(AnimationStream stream)
        {
            var inputCount = stream.inputStreamCount;
            if (inputCount == 0) return;
            Assert.AreEqual(inputCount, InputWeights.Length);

            // AnimationStream.GetInputStream() is expensive!
            // var streams = new NativeArray<AnimationStream>(Weights.Length, Allocator.Temp,
            //     NativeArrayOptions.UninitializedMemory);
            Span<AnimationStream> streams = stackalloc AnimationStream[InputWeights.Length];
            for (int i = 0; i < inputCount; i++)
            {
                Assert.IsTrue(stream.GetInputWeight(i) > 1 - _EPSILON && stream.GetInputWeight(i) < 1 + _EPSILON);

                streams[i] = stream.GetInputStream(i);
            }

            for (int i = 0; i < BoneInfos.Length; i++)
            {
                var handle = BoneInfos[i].BoneHandle;
                var boneWeight = BoneInfos[i].BoneWeight;

                var rot = Quaternion.identity;
                var pos = Vector3.zero;
                var scale = Vector3.one;
                var weightAccum = 0f;
                for (int j = 0; j < inputCount; j++)
                {
                    var inputWeight = InputWeights[j];
                    if (inputWeight < _EPSILON) continue;
                    weightAccum += inputWeight;
                    var weight = inputWeight / weightAccum * boneWeight;
                    var inputStream = streams[j];

                    // Rotation
                    var inputRot = handle.GetLocalRotation(inputStream);
                    rot = Quaternion.Slerp(rot, inputRot, weight);

                    // Position
                    var inputPos = handle.GetLocalPosition(inputStream);
                    pos = Vector3.Lerp(pos, inputPos, weight);

                    // Scale
                    var inputScale = handle.GetLocalScale(inputStream);
                    inputScale.x = Mathf.Pow(inputScale.x, weight);
                    inputScale.y = Mathf.Pow(inputScale.y, weight);
                    inputScale.z = Mathf.Pow(inputScale.z, weight);
                    scale.Scale(inputScale);
                }

                if (weightAccum > 0)
                {
                    handle.SetLocalRotation(stream, rot);
                    handle.SetLocalPosition(stream, pos);
                    handle.SetLocalScale(stream, scale);
                }
            }

            // streams.Dispose();
        }

        public void Dispose()
        {
            BoneInfos.Dispose();
            InputWeights.Dispose();
        }
    }

    public class CustomMixerAnimationScriptAsset : AnimationScriptAsset
    {
        public override bool NormalizeInputWeights => true;


        private NativeArray<float> _inputWeights;

        private AnimationScriptPlayable _animationScriptPlayable;


        public override AnimationScriptPlayable CreatePlayable(Skeleton skeleton,
            PlayableGraph playableGraph, int inputCount)
        {
            _inputWeights = new NativeArray<float>(inputCount, Allocator.Persistent);
            var customMixerJob = new CustomMixerJob(skeleton.GetOrAllocateBoneInfos(), _inputWeights);

            _animationScriptPlayable = AnimationScriptPlayable.Create(playableGraph, customMixerJob, inputCount);
            return _animationScriptPlayable;
        }

        public override void PrepareFrame(Playable playable, in FrameData info, PrepareFrameArgs args)
        {
            if (args.IsInputWeightDirty)
            {
                var customMixerJob = _animationScriptPlayable.GetJobData<CustomMixerJob>();
                for (int i = 0; i < customMixerJob.InputWeights.Length; i++)
                {
                    customMixerJob.InputWeights[i] = args.InputWeights[i];
                }

                _animationScriptPlayable.SetJobData(customMixerJob);
            }
        }

        public override void Dispose()
        {
            if (_inputWeights.IsCreated)
            {
                _inputWeights.Dispose();
            }
        }
    }
}
