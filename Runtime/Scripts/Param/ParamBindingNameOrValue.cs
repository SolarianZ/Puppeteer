using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GBG.Puppeteer.Parameter
{
    [Serializable]
    public sealed class ParamBindingNameOrValue : ICloneable
    {
        public string TargetParamName => _targetParamName;

        [SerializeField]
        private string _targetParamName;


        public string SourceParamName => _sourceParamName;

        [SerializeField]
        private string _sourceParamName;


        public float SourceValue => _sourceValue;

        [SerializeField]
        private float _sourceValue = 1f;


        public bool IsValue => string.IsNullOrEmpty(SourceParamName);


        public ParamInfo GetParamBindingSource(Dictionary<string, ParamInfo> sourceParams,
            ParamType targetParamType = ParamType.Any)
        {
            // Literal
            if (IsValue)
            {
                return new ParamInfo(null, targetParamType, SourceValue);
            }

            // Invalid source
            if (!sourceParams.TryGetValue(SourceParamName, out var sourceParam))
            {
                Debug.LogError(
                    $"[Puppeteer::ParamBinding] Can not find source param info with name '{SourceParamName}'.");

                return null;
            }

            return sourceParam;
        }

        public ParamBinding GetParamBinding(Dictionary<string, ParamInfo> sourceParams,
            Dictionary<string, ParamInfo> targetParams)
        {
            // Invalid target
            if (!targetParams.TryGetValue(TargetParamName, out var targetParam))
            {
                Debug.LogError(
                    $"[Puppeteer::ParamBinding] Can not find target param info with name '{TargetParamName}'.");

                return null;
            }

            // Literal
            if (IsValue)
            {
                return new ParamBinding(new ParamInfo(null, targetParam.Type, SourceValue), targetParam);
            }

            // Invalid source
            if (!sourceParams.TryGetValue(SourceParamName, out var sourceParam))
            {
                Debug.LogError(
                    $"[Puppeteer::ParamBinding] Can not find source param info with name '{SourceParamName}'.");

                return null;
            }

            Assert.IsTrue(sourceParam.Type == targetParam.Type
                          || sourceParam.Type == ParamType.Any
                          || targetParam.Type == ParamType.Any);

            // Binding
            return new ParamBinding(sourceParam, targetParam);
        }

        public object Clone()
        {
            return new ParamBindingNameOrValue()
            {
                _targetParamName = this._targetParamName,
                _sourceParamName = this._sourceParamName,
                _sourceValue = this._sourceValue
            };
        }
    }
}
