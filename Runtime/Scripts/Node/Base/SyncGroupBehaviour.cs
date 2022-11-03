using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.Playables;
using UFrameData = UnityEngine.Playables.FrameData;

namespace GBG.AnimationGraph.Node
{
    public enum SyncGroupRole
    {
        /** This node can be the leader, as long as it has a higher blend weight than the previous best leader. */
        CanBeLeader,

        /** This node will always be a follower (unless there are only followers, in which case the first one ticked wins). */
        AlwaysFollower,

        /** This node will always be a leader (if more than one node is AlwaysLeader, the last one ticked wins). */
        AlwaysLeader,

        /** This node will be excluded from the sync group while blending in. Once blended in it will be the sync group leader until blended out*/
        TransitionLeader,

        /** This node will be excluded from the sync group while blending in. Once blended in it will be a follower until blended out*/
        TransitionFollower,
    }

    public enum SyncMethod
    {
        // Don't sync ever
        DoNotSync,

        // Use a named sync group
        SyncGroup,

        // Use the graph structure to provide a sync group to apply
        Graph
    }

    public sealed class SyncGroupBehaviour : PlayableBehaviour
    {
        private Dictionary<string, List<AnimationAssetPlayerNodeBase>> _syncGroupNameTable;


        public void Initialize(Dictionary<string, List<AnimationAssetPlayerNodeBase>> syncGroupNameTable)
        {
            _syncGroupNameTable = syncGroupNameTable;
        }

        public override void PrepareFrame(Playable playable, UFrameData info)
        {
            base.PrepareFrame(playable, info);

            foreach (var syncGroup in _syncGroupNameTable.Values)
            {
                // Find leader animation, determine the duration of target animation
                AnimationAssetPlayerNodeBase leaderNode = null;
                for (int i = 0; i < syncGroup.Count; i++)
                {
                    var node = syncGroup[i];
                    if (leaderNode == null)
                    {
                        leaderNode = node;
                        continue;
                    }

                    switch (node.SyncGroupRole)
                    {
                        case SyncGroupRole.CanBeLeader:
                        {
                            switch (leaderNode.SyncGroupRole)
                            {
                                case SyncGroupRole.CanBeLeader:
                                {
                                    if (node.FrameData.EffectiveWeight > leaderNode.FrameData.EffectiveWeight)
                                    {
                                        leaderNode = node;
                                    }

                                    continue;
                                }
                                case SyncGroupRole.AlwaysFollower:
                                {
                                    leaderNode = node;

                                    continue;
                                }

                                case SyncGroupRole.AlwaysLeader:
                                {
                                    continue;
                                }

                                case SyncGroupRole.TransitionLeader:
                                case SyncGroupRole.TransitionFollower:
                                // TODO: TransitionLeader & TransitionFollower

                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }

                        case SyncGroupRole.AlwaysFollower:
                        {
                            switch (leaderNode.SyncGroupRole)
                            {
                                case SyncGroupRole.CanBeLeader:
                                case SyncGroupRole.AlwaysFollower:
                                case SyncGroupRole.AlwaysLeader:
                                {
                                    continue;
                                }

                                case SyncGroupRole.TransitionLeader:
                                case SyncGroupRole.TransitionFollower:
                                // TODO: TransitionLeader & TransitionFollower

                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }

                        case SyncGroupRole.AlwaysLeader:
                        {
                            switch (leaderNode.SyncGroupRole)
                            {
                                case SyncGroupRole.CanBeLeader:
                                case SyncGroupRole.AlwaysFollower:
                                case SyncGroupRole.AlwaysLeader:
                                {
                                    leaderNode = node;

                                    continue;
                                }

                                case SyncGroupRole.TransitionLeader:
                                case SyncGroupRole.TransitionFollower:
                                // TODO: TransitionLeader & TransitionFollower

                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }

                        case SyncGroupRole.TransitionLeader:
                        case SyncGroupRole.TransitionFollower:
                        // TODO: TransitionLeader & TransitionFollower

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                // Set time of each animation in the group
                var leaderAnimLen = leaderNode!.GetScaledAnimationLength();
                var animProgress = leaderNode.Playable.GetTime() % leaderAnimLen / leaderAnimLen;
                for (int i = 0; i < syncGroup.Count; i++)
                {
                    var node = syncGroup[i];
                    if (node == leaderNode)
                    {
                        continue;
                    }

                    var time = node.GetScaledAnimationLength() * animProgress;
                    node.Playable.SetTime(time);
                }
            }
        }

        public static void ResolveSyncGroup(AnimationAssetPlayerNodeBase playerNode, string graphSyncGroupName,
            Dictionary<string, List<AnimationAssetPlayerNodeBase>> outSyncGroupNameTable)
        {
            if (playerNode.MotionTimeParamActive)
            {
                return;
            }

            switch (playerNode.SyncMethod)
            {
                case SyncMethod.DoNotSync:
                    return;

                case SyncMethod.SyncGroup:
                {
                    Assert.IsFalse(string.IsNullOrEmpty(playerNode.SyncGroupName),
                        "Sync group name is empty. " +
                        $"Node type: {playerNode.GetType().Name}, node guid: {playerNode.Guid}");

                    if (!outSyncGroupNameTable.TryGetValue(playerNode.SyncGroupName, out var syncGroup))
                    {
                        syncGroup = new List<AnimationAssetPlayerNodeBase>();
                        outSyncGroupNameTable.Add(playerNode.SyncGroupName, syncGroup);
                    }

                    syncGroup.Add(playerNode);

                    return;
                }

                case SyncMethod.Graph:
                {
                    Assert.IsFalse(string.IsNullOrEmpty(graphSyncGroupName),
                        "Graph sync group name is empty. " +
                        $"Node type: {playerNode.GetType().Name}, node guid: {playerNode.Guid}");

                    if (!outSyncGroupNameTable.TryGetValue(graphSyncGroupName, out var group))
                    {
                        group = new List<AnimationAssetPlayerNodeBase>();
                        outSyncGroupNameTable.Add(graphSyncGroupName, group);
                    }

                    group.Add(playerNode);

                    return;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
