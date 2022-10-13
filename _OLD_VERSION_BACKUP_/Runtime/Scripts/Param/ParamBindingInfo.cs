using System;
using UnityEngine;

namespace GBG.Puppeteer.Parameter
{
    [Serializable]
    public class ParamBindingInfo : ParamInfo
    {
        public string BindToName => _bindToName;

        [SerializeField]
        private string _bindToName;


        public ParamBindingInfo(string bindToName, string inputName, ParamType type, float rawValue = 0)
            : base(inputName, type, rawValue)
        {
            _bindToName = bindToName;
        }

        public ParamBindingInfo(string bindToName, string inputName, Type valueType, float rawValue = 0)
            : base(inputName, valueType, rawValue)
        {
            _bindToName = bindToName;
        }

        public ParamBindingInfo(string bindToName, ParamInfo paramInfo)
            : this(bindToName, paramInfo.Name, paramInfo.Type, paramInfo.GetRawValue())
        {
        }

        public static ParamBindingInfo CreateLiteral(string bindToName, ParamType type, float rawValue)
        {
            return new ParamBindingInfo(bindToName, null, type, rawValue);
        }
    }
}
