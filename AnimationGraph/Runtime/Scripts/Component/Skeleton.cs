using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;
using UDebug = UnityEngine.Debug;

namespace GBG.AnimationGraph.Component
{
    /// <summary>
    /// 骨骼节点数据。
    /// </summary>
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
        /// 分配一个存储 <see cref="BoneInfo"/> 的 <see cref="NativeArray{T}"/> 实例。
        /// NativeArray使用完成后，需要对其调用 Dispose 方法将其释放。
        /// </summary>
        /// <param name="animator">Animator组件。</param>
        /// <param name="bones">Animator组件所属对象的骨骼节点。骨骼节点必须是Animator组件所属节点的直接或间接子节点。</param>
        /// <param name="nameToHash">将骨骼节点名称转为Hash值的方法。默认为 <see cref="Animator.StringToHash"/> 。</param>
        /// <returns>含有骨骼节点数据的 <see cref="NativeArray{T}"/> 实例。</returns>
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
        /// 尝试查找骨骼节点数据。
        /// </summary>
        /// <param name="bones">骨骼节点数据数组。将从此数据数组中查找目标数据。</param>
        /// <param name="boneNameHash">目标骨骼名称Hash值。</param>
        /// <param name="index">目标骨骼节点数据在 <see cref="bones"/> 中的索引。</param>
        /// <returns>是否成功找到目标骨骼节点数据。</returns>
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

    /// <summary>
    /// 骨骼组件。
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class Skeleton : MonoBehaviour
    {
        [Tooltip("骨骼节点数组。骨骼节点必须是Animator组件所属节点的直接或间接子节点。")]
        [SerializeField]
        [NonReorderable]
        private Transform[] _bones = Array.Empty<Transform>();

        private NativeArray<BoneInfo> _boneInfos;


        private void Reset()
        {
            CollectBonesFromHierarchy(this, true);
        }

        private void Start()
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                if (!_bones[i])
                {
                    UDebug.LogError($"[Puppeteer::Skeleton] Skeleton bone at index '{i}' is null.");
                }
            }
        }

        private void OnDestroy()
        {
            if (_boneInfos.IsCreated)
            {
                _boneInfos.Dispose();
            }
        }

        /// <summary>
        /// 分配一个存储 <see cref="BoneInfo"/> 的 <see cref="NativeArray{T}"/> 实例。
        /// NativeArray使用完成后，需要对其调用 Dispose 方法将其释放。
        /// </summary>
        /// <param name="animator">Animator组件。</param>
        /// <param name="nameToHash">将骨骼节点名称转为Hash值的方法。默认为 <see cref="Animator.StringToHash"/> 。</param>
        /// <returns>含有骨骼节点数据的 <see cref="NativeArray{T}"/> 实例。</returns>
        public NativeArray<BoneInfo> GetOrAllocateBoneInfos(Animator animator, Func<string, int> nameToHash = null)
        {
            if (!_boneInfos.IsCreated)
            {
                _boneInfos = BoneInfo.AllocateBoneInfos(animator, _bones, nameToHash);
            }

            return _boneInfos;
        }

        public static bool CollectBonesFromHierarchy(Skeleton skeleton, bool noRenderer)
        {
            var hierarchy = new List<Transform>(200);
            if (!noRenderer || !skeleton.GetComponent<Renderer>())
            {
                hierarchy.Add(skeleton.transform);
            }

            GetChildren(skeleton.transform, hierarchy, noRenderer);
            skeleton._bones = hierarchy.ToArray();

            return true;

            static void GetChildren(Transform node, List<Transform> result, bool noRenderer)
            {
                foreach (Transform child in node)
                {
                    if (child == node)
                    {
                        continue;
                    }

                    if (!noRenderer || !child.GetComponent<Renderer>())
                    {
                        result.Add(child);
                    }

                    GetChildren(child, result, noRenderer);
                }
            }
        }

        public static bool CollectBonesFromAvatar(Skeleton skeleton, bool noRenderer)
        {
            var avatar = skeleton.GetComponent<Animator>().avatar;
            if (!avatar)
            {
                return false;
            }

            var boneDict = new Dictionary<string, Transform>(skeleton._bones.Length);
            GetChildren(skeleton.transform, boneDict, noRenderer);

            var skeletonBones = avatar.humanDescription.skeleton;
            var bones = new List<Transform>(skeletonBones.Length);
            for (int i = 0; i < skeletonBones.Length; i++)
            {
                if (!boneDict.TryGetValue(skeletonBones[i].name, out var bone))
                {
                    UDebug.LogError($"[Puppeteer::Skeleton] Bone '{skeletonBones[i].name}' not found.");
                    continue;
                }

                bones.Add(bone);
            }

            skeleton._bones = bones.ToArray();

            return true;

            static void GetChildren(Transform node, Dictionary<string, Transform> result, bool noRenderer)
            {
                foreach (Transform child in node)
                {
                    if (child == node)
                    {
                        continue;
                    }

                    if (!noRenderer || !child.GetComponent<Renderer>())
                    {
                        result.Add(child.name, child);
                    }

                    GetChildren(child, result, noRenderer);
                }
            }
        }
    }
}
