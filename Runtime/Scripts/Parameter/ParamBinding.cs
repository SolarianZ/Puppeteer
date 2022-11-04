using System;
using UnityEngine.Assertions;

namespace GBG.AnimationGraph.Parameter
{
    /// <summary>
    /// Give the source param's value or the raw value to the destination param.
    /// </summary>
    public class ParamBinding : IDisposable
    {
        public ParamInfo SrcParam { get; }

        public ParamInfo DestParam { get; }


        public ParamBinding(ParamInfo srcParam, ParamInfo destParam, bool syncParamValue)
        {
            Assert.IsTrue((srcParam == null) || (srcParam.Type == destParam.Type));

            SrcParam = srcParam;
            DestParam = destParam;

            if (SrcParam != null)
            {
                if (syncParamValue)
                {
                    DestParam.SetRawValue(SrcParam.RawValue);
                }

                SrcParam.OnValueChanged += OnSourceValueChanged;
            }
        }


        private void OnSourceValueChanged(ParamInfo fromParam)
        {
            DestParam.SetRawValue(fromParam.RawValue);
        }


        public void Dispose()
        {
            if (SrcParam != null)
            {
                SrcParam.OnValueChanged -= OnSourceValueChanged;
            }
        }
    }
}
