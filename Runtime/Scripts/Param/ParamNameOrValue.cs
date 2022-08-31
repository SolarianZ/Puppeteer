using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GBG.Puppeteer.Parameter
{
    [Serializable]
    public sealed class ParamNameOrValue : ICloneable
    {
        public string Name => _name;

        [SerializeField]
        private string _name;


        public float RawValue => _rawValue;

        [SerializeField]
        private float _rawValue = 1f;


        public bool IsValue => string.IsNullOrEmpty(Name);


        public ParamNameOrValue(string name, float rawRawValue)
        {
            _name = name;
            _rawValue = rawRawValue;
        }

        public ParamNameOrValue(ParamInfo paramInfo)
        {
            _name = paramInfo.Name;
            _rawValue = paramInfo.GetRawValue();
        }

        public float GetFloat()
        {
            Assert.IsTrue(IsValue);

            return _rawValue;
        }

        public int GetInt()
        {
            Assert.IsTrue(IsValue);

            return (int)Math.Round(_rawValue);
        }

        public bool GetBool()
        {
            Assert.IsTrue(IsValue);

            return Mathf.Approximately(_rawValue, 1);
        }

        public ParamInfo GetParamInfo(IDictionary<string, ParamInfo> paramTable, ParamType paramType = ParamType.Any)
        {
            if (IsValue)
            {
                return new ParamInfo(null, paramType, RawValue);
            }

            var paramInfo = paramTable[Name];
            Assert.IsTrue(paramType == ParamType.Any
                          || paramInfo.Type == ParamType.Any
                          || paramInfo.Type == paramType);

            return paramInfo;
        }

        public ParamInfo GetParamInfo(IList<ParamInfo> paramTable, ParamType paramType = ParamType.Any)
        {
            if (IsValue)
            {
                return new ParamInfo(null, paramType, RawValue);
            }

            for (int i = 0; i < paramTable.Count; i++)
            {
                if (paramTable[i].Name.Equals(Name))
                {
                    var paramInfo = paramTable[i];
                    Assert.IsTrue(paramType == ParamType.Any
                                  || paramInfo.Type == ParamType.Any
                                  || paramInfo.Type == paramType);
                    return paramInfo;
                }
            }

            return null;
        }

        public object Clone()
        {
            return new ParamNameOrValue(_name, _rawValue);
        }
    }
}
