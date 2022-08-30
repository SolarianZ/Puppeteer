using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GBG.Puppeteer.Parameter
{
    [Serializable]
    public class ParamNameOrValue : ICloneable
    {
        public string Name => _name;

        [SerializeField]
        private string _name;


        public float Value => _value;

        [SerializeField]
        private float _value = 1f;


        public bool IsValue => string.IsNullOrEmpty(Name);


        public float GetFloat()
        {
            Assert.IsTrue(IsValue);

            return _value;
        }

        public int GetInt()
        {
            Assert.IsTrue(IsValue);

            return (int)Math.Round(_value);
        }

        public bool GetBool()
        {
            Assert.IsTrue(IsValue);

            return Mathf.Approximately(_value, 1);
        }

        public ParamInfo GetParamInfo(Dictionary<string, ParamInfo> parameters, ParamType paramType = ParamType.Any)
        {
            if (IsValue)
            {
                return new ParamInfo(null, paramType, Value);
            }

            var paramInfo = parameters[Name];
            Assert.IsTrue(paramInfo.Type == paramType || paramInfo.Type == ParamType.Any);

            return paramInfo;
        }

        public object Clone()
        {
            return new ParamNameOrValue()
            {
                _name = this._name,
                _value = this._value
            };
        }
    }
}