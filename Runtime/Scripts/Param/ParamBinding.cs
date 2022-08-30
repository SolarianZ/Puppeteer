using System;
using GBG.Puppeteer.Parameter;
using UnityEngine.Assertions;

namespace GBG.Puppeteer.Parameter
{
    public class ParamBinding : IDisposable
    {
        public ParamInfo Source { get; }

        public ParamInfo Target { get; }


        public ParamBinding(ParamInfo source, ParamInfo target)
        {
            Assert.IsTrue((source.Type == target.Type)
                          || (source.Type == ParamType.Any)
                          || (target.Type == ParamType.Any));

            Source = source;
            Target = target;

            Source.OnValueChanged += OnSourceValueChanged;
        }


        private void OnSourceValueChanged(ParamInfo source)
        {
            Target.SetRawValue(source.GetRawValue());
        }


        public void Dispose()
        {
            Source.OnValueChanged -= OnSourceValueChanged;
        }
    }
}