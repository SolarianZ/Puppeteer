using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UDebug = UnityEngine.Debug;

namespace GBG.AnimationGraph.Component
{
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class Skeleton : MonoBehaviour
    {
        [Tooltip("Bones under Animator.")]
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
        /// Get or allocate a instance of <see cref="NativeArray{BoneInfo}"/> of <see cref="BoneInfo"/>.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="nameToHash">Method for calculate hash from name.</param>
        /// <returns>The instance of <see cref="NativeArray{BoneInfo}"/> of <see cref="BoneInfo"/>.</returns>
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
