using System;
using GBG.AnimationGraph.Component;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;

namespace GBG.AnimationGraph.Job
{
    public struct CustomMixerJob : IAnimationJob, IDisposable
    {
        public NativeArray<BoneInfo> BoneInfos;

        public NativeArray<float> Weights;

        private const float _EPSILON = 1E-5f;


        public CustomMixerJob(NativeArray<BoneInfo> boneInfos, NativeArray<float> weights)
        {
            BoneInfos = boneInfos;
            Weights = weights;
        }

        public void ProcessRootMotion(AnimationStream stream)
        {
            var inputCount = stream.inputStreamCount;
            if (inputCount == 0) return;
            Assert.AreEqual(inputCount, Weights.Length);

            var velocity = Vector3.zero;
            var angularVelocity = Vector3.zero;
            var weightAccum = 0f;
            for (int i = 0; i < inputCount; i++)
            {
                Assert.IsTrue(stream.GetInputWeight(i) > 1 - _EPSILON && stream.GetInputWeight(i) < 1 + _EPSILON);

                var inputWeight = Weights[i];
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
            Assert.AreEqual(inputCount, Weights.Length);

            // AnimationStream.GetInputStream() is expensive!
            // var streams = new NativeArray<AnimationStream>(Weights.Length, Allocator.Temp,
            //     NativeArrayOptions.UninitializedMemory);
            Span<AnimationStream> streams = stackalloc AnimationStream[Weights.Length];
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
                    var inputWeight = Weights[j];
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
            Weights.Dispose();
        }
    }
}
