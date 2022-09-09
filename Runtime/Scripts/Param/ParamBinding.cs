using System;
using UnityEngine.Assertions;

namespace GBG.Puppeteer.Parameter
{
    public class ParamBinding : IDisposable
    {
        public ParamInfo Source { get; }

        public ParamInfo Target { get; }


        public ParamBinding(ParamInfo source, ParamInfo target)
        {
            Assert.IsTrue((source == null) || (source.Type == target.Type));

            Source = source;
            Target = target;

            if (Source != null)
            {
                Source.OnValueChanged += OnSourceValueChanged;
            }
        }


        private void OnSourceValueChanged(ParamInfo source)
        {
            Target.SetRawValue(source.GetRawValue());
        }


        public void Dispose()
        {
            if (Source != null)
            {
                Source.OnValueChanged -= OnSourceValueChanged;
            }
        }
    }
}
