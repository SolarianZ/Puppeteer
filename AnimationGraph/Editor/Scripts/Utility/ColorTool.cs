using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

namespace GBG.AnimationGraph.Editor.Utility
{
    public static class ColorTool
    {
        private static readonly Dictionary<Type, Color> _colors = new Dictionary<Type, Color>
        {
            { typeof(Playable), Color.cyan }
        };


        public static Color GetColor(Type type)
        {
            if (_colors.TryGetValue(type, out var color))
            {
                return color;
            }

            return GetRandomColor();
        }

        public static Color GetRandomColor(byte rgbMax = 200, byte minInterval = 25, int? randomSeed = null)
        {
            if (randomSeed != null)
            {
                Random.InitState(randomSeed.Value);
            }

            byte r, g, b;
            while (true)
            {
                r = (byte)Random.Range(0, rgbMax + 1);
                g = (byte)Random.Range(0, rgbMax + 1);
                b = (byte)Random.Range(0, rgbMax + 1);

                if (Math.Abs(r - g) >= minInterval &&
                    Math.Abs(g - b) >= minInterval &&
                    Math.Abs(b - r) >= minInterval)
                {
                    break;
                }
            }

            return new Color(r / 255f, g / 255f, b / 255f, 1.0f);
        }
    }
}
