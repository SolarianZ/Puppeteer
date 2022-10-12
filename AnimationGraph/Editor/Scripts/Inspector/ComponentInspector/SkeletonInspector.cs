using System;
using GBG.AnimationGraph.Component;
using UnityEditor;
using UnityEngine;

namespace GBG.AnimationGraph.Editor.Inspector
{
    [CustomEditor(typeof(Skeleton))]
    internal class SkeletonInspector : UnityEditor.Editor
    {
        enum BoneCollectMode
        {
            FromHierarchyNoRenderer,
            FromHierarchy,
            FromAvatarNoRenderer,
            FromAvatar,
        }


        private BoneCollectMode _boneCollectMode;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    _boneCollectMode = (BoneCollectMode)EditorGUILayout.EnumPopup(_boneCollectMode);

                    if (GUILayout.Button("Re-Collect Bones"))
                    {
                        var skeleton = (Skeleton)target;
                        Undo.RecordObject(skeleton, "Re-Collect Bones");

                        switch (_boneCollectMode)
                        {
                            case BoneCollectMode.FromHierarchy:
                                Skeleton.CollectBonesFromHierarchy(skeleton, false);
                                break;

                            case BoneCollectMode.FromHierarchyNoRenderer:
                                Skeleton.CollectBonesFromHierarchy(skeleton, true);
                                break;

                            case BoneCollectMode.FromAvatar:
                                Skeleton.CollectBonesFromAvatar(skeleton, false);
                                break;

                            case BoneCollectMode.FromAvatarNoRenderer:
                                Skeleton.CollectBonesFromAvatar(skeleton, true);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }
    }
}
