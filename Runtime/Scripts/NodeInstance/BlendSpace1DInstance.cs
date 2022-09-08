using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeInstance
{
    [Serializable]
    public class MotionField1D
    {
        [SerializeField]
        public AnimationClip Clip;

        [SerializeField]
        public float Position;

        [SerializeField]
        public float PlaybackSpeed = 1.0f;

        // [SerializeField]
        // public bool Mirror; // Not yet supported
    }

    public class BlendSpace1DInstance
    {
        public AnimationMixerPlayable Mixer { get; }

        public float Position { get; private set; }


        private readonly MotionField1D[] _motionFields;


        public BlendSpace1DInstance(PlayableGraph graph, MotionField1D[] motionFields, float position)
        {
            _motionFields = motionFields;
            SortMotionFields(_motionFields);

            Mixer = AnimationMixerPlayable.Create(graph, _motionFields.Length);
            for (int i = 0; i < _motionFields.Length; i++)
            {
                var clipPlayable = AnimationClipPlayable.Create(graph, _motionFields[i].Clip);
                clipPlayable.SetSpeed(_motionFields[i].PlaybackSpeed);
                Mixer.ConnectInput(i, clipPlayable, 0);
            }

            SetPosition(position);
        }

        public void SetPosition(float position)
        {
            Position = position;

            var leftIndex = new int?();
            for (int i = 0; i < _motionFields.Length; i++)
            {
                if (leftIndex == null)
                {
                    // The left most motion
                    if (Position < _motionFields[i].Position)
                    {
                        Assert.AreEqual(i, 0);
                        Mixer.SetInputWeight(i, 1);
                        leftIndex = -1;
                        continue;
                    }

                    if (i < _motionFields.Length - 1)
                    {
                        // Not in the interval
                        if (Position > _motionFields[i + 1].Position)
                        {
                            Mixer.SetInputWeight(i, 0);
                            continue;
                        }

                        // In the interval
                        var rightWeight = (Position - _motionFields[i].Position) /
                                          (_motionFields[i + 1].Position - _motionFields[i].Position);
                        var leftWeight = 1 - rightWeight;
                        Mixer.SetInputWeight(i, leftWeight);
                        Mixer.SetInputWeight(i + 1, rightWeight);
                        leftIndex = i;
                        continue;
                    }

                    // The most right motion
                    Assert.AreEqual(i, _motionFields.Length - 1);
                    Mixer.SetInputWeight(i, 1);
                    leftIndex = i;
                }
                else if (leftIndex.Value + 1 != i)
                {
                    // Not in the interval
                    Mixer.SetInputWeight(i, 0);
                }
            }
        }


        private static void SortMotionFields(MotionField1D[] motionFields)
        {
            for (int i = 0; i < motionFields.Length; i++)
            {
                var swapped = false;
                for (int j = 0; j < motionFields.Length - 1 - i; j++)
                {
                    if (motionFields[j].Position > motionFields[j + 1].Position)
                    {
                        (motionFields[j], motionFields[j + 1]) = (motionFields[j + 1], motionFields[j]);
                        swapped = true;
                    }
                }

                // Ordered data
                if (!swapped)
                {
                    return;
                }
            }
        }
    }
}
