using System;
using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;
using UnityEngine.Playables;

namespace GBG.Puppeteer.NodeInstance
{
    [Serializable]
    public class MotionField1D : ICloneable
    {
        [SerializeField]
        public AnimationClip Clip;

        [SerializeField]
        public float Position;

        [SerializeField]
        public float PlaybackSpeed = 1.0f;

        // [SerializeField]
        // public bool Mirror; // Not yet supported


        public object Clone()
        {
            return new MotionField1D
            {
                Clip = this.Clip,
                Position = this.Position,
                PlaybackSpeed = this.PlaybackSpeed
            };
        }
    }

    public class BlendSpace1DInstance : AnimationNodeInstance
    {
        public override Playable Playable { get; }


        private readonly ParamInfo _position;

        private readonly MotionField1D[] _motionFields;


        public BlendSpace1DInstance(PlayableGraph graph, MotionField1D[] motionFields, ParamInfo position,
            ParamInfo playbackSpeed) : base(playbackSpeed)
        {
            _position = position;
            _position.OnValueChanged += OnPositionValueChanged;

            _motionFields = motionFields;
            SortMotionFields(_motionFields);

            Playable = AnimationMixerPlayable.Create(graph, _motionFields.Length);
            for (int i = 0; i < _motionFields.Length; i++)
            {
                var clipPlayable = AnimationClipPlayable.Create(graph, _motionFields[i].Clip);
                clipPlayable.SetSpeed(_motionFields[i].PlaybackSpeed);
                Playable.ConnectInput(i, clipPlayable, 0);
            }

            SetPosition(position.GetFloat());
        }


        private void OnPositionValueChanged(ParamInfo _)
        {
            SetPosition(_position.GetFloat());
        }

        private void SetPosition(float position)
        {
            var leftIndex = new int?();
            for (int i = 0; i < _motionFields.Length; i++)
            {
                if (leftIndex == null)
                {
                    // The left most motion
                    if (position < _motionFields[i].Position)
                    {
                        Assert.AreEqual(i, 0);
                        Playable.SetInputWeight(i, 1);
                        leftIndex = -1;
                        continue;
                    }

                    if (i < _motionFields.Length - 1)
                    {
                        // Not in the interval
                        if (position > _motionFields[i + 1].Position)
                        {
                            Playable.SetInputWeight(i, 0);
                            continue;
                        }

                        // In the interval
                        var rightWeight = (position - _motionFields[i].Position) /
                                          (_motionFields[i + 1].Position - _motionFields[i].Position);
                        var leftWeight = 1 - rightWeight;
                        Playable.SetInputWeight(i, leftWeight);
                        Playable.SetInputWeight(i + 1, rightWeight);
                        leftIndex = i;
                        continue;
                    }

                    // The most right motion
                    Assert.AreEqual(i, _motionFields.Length - 1);
                    Playable.SetInputWeight(i, 1);
                    leftIndex = i;
                }
                else if (leftIndex.Value + 1 != i)
                {
                    // Not in the interval
                    Playable.SetInputWeight(i, 0);
                }
            }
        }


        public override void Dispose()
        {
            _position.OnValueChanged -= OnPositionValueChanged;

            base.Dispose();
        }


        public static void SortMotionFields(MotionField1D[] motionFields)
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
