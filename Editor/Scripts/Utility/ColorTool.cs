using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer.Editor.Utility
{
    public static class ColorTool
    {
        private static readonly Dictionary<Type, Color> _colors = new Dictionary<Type, Color>
        {
            { typeof(Playable), Color.cyan }
        };


        public static void SetColor(Type type, Color color)
        {
            _colors[type] = color;
        }

        public static Color GetColor(Type type, Color? defaultColor = null)
        {
            if (_colors.TryGetValue(type, out var color))
            {
                return color;
            }

            return defaultColor ?? Color.white;
        }
    }
}
