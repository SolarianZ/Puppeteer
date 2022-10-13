using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GBG.Puppeteer.Parameter
{
    [Serializable]
    public sealed class ParamBindingNameOrValue : ParamNameOrValue
    {
        public string BindToName => _bindToName;

        [SerializeField]
        private string _bindToName;


        public ParamBindingNameOrValue(string bindToName, string inputParamName, float inputRawValue)
            : base(inputParamName, inputRawValue)
        {
            _bindToName = bindToName;
        }

        public ParamBindingNameOrValue(ParamBindingInfo paramBindingInfo)
            : this(paramBindingInfo.BindToName, paramBindingInfo.Name, paramBindingInfo.GetRawValue())
        {
        }

        public ParamInfo GetParamBindingSource(IEnumerable<ParamInfo> paramTable, ParamType targetParamType)
        {
            // Literal
            if (IsValue)
            {
                return new ParamInfo(null, targetParamType, RawValue);
            }

            foreach (var paramInfo in paramTable)
            {
                if (paramInfo.Name.Equals(Name))
                {
                    return paramInfo;
                }
            }

            Debug.LogError($"[Puppeteer::ParamBinding] Can not find input param info with name '{Name}'.");

            return null;
        }

        public ParamInfo GetParamBindingSource(Dictionary<string, ParamInfo> sourceParams, ParamType targetParamType)
        {
            // Literal
            if (IsValue)
            {
                return new ParamInfo(null, targetParamType, RawValue);
            }

            // Invalid source
            if (!sourceParams.TryGetValue(Name, out var sourceParam))
            {
                Debug.LogError(
                    $"[Puppeteer::ParamBinding] Can not find input param info with name '{Name}'.");

                return null;
            }

            return sourceParam;
        }

        public ParamBinding GetParamBinding(Dictionary<string, ParamInfo> inputParamTable,
            Dictionary<string, ParamInfo> outputParamTable)
        {
            // Invalid target
            if (!outputParamTable.TryGetValue(BindToName, out var targetParam))
            {
                Debug.LogError(
                    $"[Puppeteer::ParamBinding] Can not find output param info with name '{BindToName}'.");

                return null;
            }

            // Literal
            if (IsValue)
            {
                return new ParamBinding(new ParamInfo(null, targetParam.Type, RawValue), targetParam);
            }

            // Invalid source
            if (!inputParamTable.TryGetValue(Name, out var sourceParam))
            {
                Debug.LogError(
                    $"[Puppeteer::ParamBinding] Can not find input param info with name '{Name}'.");

                return null;
            }

            Assert.IsTrue(sourceParam.Type == targetParam.Type);

            // Binding
            return new ParamBinding(sourceParam, targetParam);
        }

        public override object Clone()
        {
            return new ParamBindingNameOrValue(_bindToName, Name, RawValue);
        }
    }
}
