using System;
using GBG.AnimationGraph.Parameter;
using UnityEngine;

namespace GBG.AnimationGraph.Editor.Blackboard
{
    [Serializable]
    public class ParamBindingInfo : ParamInfo
    {
        public string BindToName => _bindToName;

        [SerializeField]
        private string _bindToName;


        public ParamBindingInfo(string guid, string bindToName, string inputName, ParamType type, float rawValue)
            : base(guid, inputName, type, rawValue)
        {
            _bindToName = bindToName;
        }

        public ParamBindingInfo(string guid, string bindToName, string inputName, Type valueType, float rawValue)
            : base(guid, inputName, valueType, rawValue)
        {
            _bindToName = bindToName;
        }

        public ParamBindingInfo(string guid, string bindToName, ParamInfo paramInfo)
            : this(guid, bindToName, paramInfo.Name, paramInfo.Type, paramInfo.RawValue)
        {
        }


        public ParamBindingGuidOrValue_Old CreateParamBindingNameOrValue()
        {
            return new ParamBindingGuidOrValue_Old(Name, BindToName, RawValue);
        }


        public static ParamBindingInfo CreateLiteral(string bindToName, ParamType type, float rawValue)
        {
            return new ParamBindingInfo(null, bindToName, null, type, rawValue);
        }
    }
}
