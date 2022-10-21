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

        public bool IsValue()
        {
            return SrcParamGuidOrValue.IsValue;
        }
    }

    /// <summary>
    /// Only used for serialization.
    /// </summary>
    [Serializable]
    public class ParamBindingGuidOrValue_Old : ParamGuidOrValue
    {
        public string ToParamName => toParamName;

        [SerializeField]
        private string toParamName;


        public ParamBindingGuidOrValue_Old(string fromParamGuid, string toParamName, float rawValue)
            : base(fromParamGuid, rawValue)
        {
            this.toParamName = toParamName;
        }


        public ParamInfo GetParamBindingSource(IEnumerable<ParamInfo> fromParamTable, ParamType toParamType)
        {
            // Literal
            if (IsValue)
            {
                return new ParamInfo(null, null, toParamType, RawValue);
            }

            foreach (var fromParam in fromParamTable)
            {
                if (fromParam.Name.Equals(Guid))
                {
                    Assert.IsTrue(fromParam.Type == toParamType);
                    return fromParam;
                }
            }

            return null;
        }

        public ParamInfo GetParamBindingSource(Dictionary<string, ParamInfo> fromParamTable, ParamType toParamType)
        {
            // Literal
            if (IsValue)
            {
                return new ParamInfo(null, null, toParamType, RawValue);
            }

            // Invalid source
            if (!fromParamTable.TryGetValue(Guid, out var fromParam))
            {
                return null;
            }

            return fromParam;
        }


        public ParamBinding GetParamBinding(Dictionary<string, ParamInfo> fromParamTable,
            Dictionary<string, ParamInfo> toParamTable)
        {
            // Invalid target
            if (!toParamTable.TryGetValue(ToParamName, out var toParam))
            {
                return null;
            }

            // Literal
            if (IsValue)
            {
                return new ParamBinding(new ParamInfo(null, null, toParam.Type, RawValue), toParam);
            }

            // Invalid source
            if (!fromParamTable.TryGetValue(Guid, out var fromParam))
            {
                return null;
            }

            Assert.IsTrue(fromParam.Type == toParam.Type);

            // Binding
            return new ParamBinding(fromParam, toParam);
        }
    }
}
