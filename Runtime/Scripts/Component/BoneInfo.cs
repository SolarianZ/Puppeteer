using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;

namespace GBG.AnimationGraph.Component
{
    public readonly struct BoneInfo
    {
        public TransformStreamHandle BoneHandle { get; }

        public float BoneWeight { get; }

        public int BoneNameHash { get; }

        public int ParentIndex { get; }

        public bool IsValid { get; }


        public BoneInfo(TransformStreamHandle boneHandle, float boneWeight, int boneNameHash, int parentIndex)
        {
            BoneHandle = boneHandle;
            BoneWeight = boneWeight;
            BoneNameHash = boneNameHash;
            ParentIndex = parentIndex;

            IsValid = BoneNameHash != 0;
        }

        /// <summary>
        /// Allocate a instance of <see cref="NativeArray{BoneInfo}"/> of <see cref="BoneInfo"/>.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="bones">Bone transforms under the <see cref="Animator"/> component.</param>
        /// <param name="nameToHash">Method for calculate hash from name. Default is <see cref="Animator.StringToHash"/>.</param>
        /// <returns>The instance of <see cref="NativeArray{BoneInfo}"/> of <see cref="BoneInfo"/>.</returns>
        public static NativeArray<BoneInfo> AllocateBoneInfos(Animator animator, Transform[] bones,
            Func<string, int> nameToHash = null)
        {
            var boneInfos = new NativeArray<BoneInfo>(bones.Length,
                Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < bones.Length; i++)
            {
                // Editor only assertions
                Assert.IsTrue(bones[i]);
                Assert.IsTrue(IsInHierarchy(animator.transform, bones[i]));

                if (bones[i])
                {
                    // TODO: BoneWeight is always 1
                    boneInfos[i] = new BoneInfo(animator.BindStreamTransform(bones[i]), 1,
                        nameToHash?.Invoke(bones[i].name) ?? Animator.StringToHash(bones[i].name),
                        FindParentIndex(bones, bones[i].parent, i));
                }
            }

            return boneInfos;

            static int FindParentIndex(Transform[] bones, Transform parent, int fromIndex)
            {
                if (!parent) return -1;
                for (int i = fromIndex - 1; i >= 0; i--)
                {
                    if (bones[i] == parent) return i;
                }

                return -1;
            }

            static bool IsInHierarchy(Transform root, Transform other)
            {
                while (other)
                {
                    if (other == root) return true;
                    other = other.parent;
                    if (other == root.parent) return false;
                }

                return false;
            }
        }

        /// <summary>
        /// Try to find bone node info.
        /// </summary>
        /// <param name="bones">NativeArray of BoneInfo. Will find target info from this NativeArray.</param>
        /// <param name="boneNameHash">Hash of the target bone.</param>
        /// <param name="index">The index of the target bone.</param>
        /// <returns>Whether it is successful or not to find target info.</returns>
        public static bool TryFindBone(NativeArray<BoneInfo> bones, int boneNameHash, out int index)
        {
            for (int i = 0; i < bones.Length; i++)
            {
                if (bones[i].BoneNameHash == boneNameHash)
                {
                    index = i;
                    return true;
                }
            }

            index = default;
            return false;
        }
    }
}
