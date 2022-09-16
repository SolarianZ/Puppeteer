using System;
using UnityEngine.Assertions;

namespace GBG.AnimationGraph.Parameter
{
    public class ParamBinding : IDisposable
    {
        public ParamInfo FromParam { get; }

        public ParamInfo ToParam { get; }


        public ParamBinding(ParamInfo fromParam, ParamInfo toParam)
        {
            Assert.IsTrue((fromParam == null) || (fromParam.Type == toParam.Type));

            FromParam = fromParam;
            ToParam = toParam;

            if (FromParam != null)
            {
                FromParam.OnValueChanged += OnSourceValueChanged;
            }
        }


        private void OnSourceValueChanged(ParamInfo fromParam)
        {
            ToParam.SetRawValue(fromParam.RawValue);
        }


        public void Dispose()
        {
            if (FromParam != null)
            {
                FromParam.OnValueChanged -= OnSourceValueChanged;
            }
        }
    }
}
