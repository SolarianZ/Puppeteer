using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GBG.AnimationGraph.Parameter
{
    /// <summary>
    /// Give the source param's value or the raw value to the destination param.
    /// </summary>
    [Serializable]
    public class ParamBindingGuidOrValue
    {
        public ParamGuidOrValue SrcParamGuidOrValue
        {
            get => _srcParamGuidOrValue;
            internal set => _srcParamGuidOrValue = value;
        }

        [SerializeField]
        private ParamGuidOrValue _srcParamGuidOrValue;

        public string DestParamGuid
        {
            get => _destParamGuid;
            internal set => _destParamGuid = value;
        }

        [SerializeField]
        private string _destParamGuid;

        public ParamBindingGuidOrValue(string srcParamGuid, string destParamGuid)
        {
            SrcParamGuidOrValue = new ParamGuidOrValue(srcParamGuid, 0);
            DestParamGuid = destParamGuid;
        }

        public ParamBindingGuidOrValue(float srcRawValue, string destParamGuid)
        {
            SrcParamGuidOrValue = new ParamGuidOrValue(null, srcRawValue);
            DestParamGuid = destParamGuid;
        }

        public bool IsLiteral()
        {
            return SrcParamGuidOrValue.IsLiteral;
        }

        public string GetSrcParamGuid()
        {
            Assert.IsFalse(IsLiteral(), "Source param is literal.");
            return SrcParamGuidOrValue.Guid;
        }

        public float GetRawValue()
        {
            Assert.IsTrue(IsLiteral(), "Source param is not literal.");
            return SrcParamGuidOrValue.RawValue;
        }
    }
}
