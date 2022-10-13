using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GBG.AnimationGraph.Parameter
{
    /// <summary>
    /// Only used for serialization.
    /// </summary>
    [Serializable]
    public class ParamBindingGuidOrValue : ParamGuidOrValue
    {
        public string ToParamName => toParamName;

        [SerializeField]
        private string toParamName;


        public ParamBindingGuidOrValue(string fromParamGuid, string toParamName, float rawValue)
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
