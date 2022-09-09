using System;
using GBG.Puppeteer.Parameter;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.GraphParam
{
    public interface IParamBindingField
    {
        string BindToParamName { get; }


        bool GetParamBindingInfo(out ParamBindingInfo paramBindingInfo);
    }

    public class ParamBindingField<TValue> : ParamField<TValue>, IParamBindingField
        where TValue : struct
    {
        public string BindToParamName { get; }

        public ParamBindingField(string bindToParamName, Length? labelWidth = null)
            : base(bindToParamName, labelWidth)
        {
            BindToParamName = bindToParamName;
        }

        public bool GetParamBindingInfo(out ParamBindingInfo paramBindingInfo)
        {
            if (Linked && LinkedParam != null)
            {
                paramBindingInfo = new ParamBindingInfo(BindToParamName, LinkedParam);
                return true;
            }

            object boxedValue = Linked ? 0f : ValueField.value;
            var valueType = typeof(TValue);
            if (valueType == typeof(float))
            {
                paramBindingInfo = ParamBindingInfo.CreateLiteral(BindToParamName,
                    ParamType.Float, (float)boxedValue);
                return true;
            }

            if (valueType == typeof(int))
            {
                paramBindingInfo = ParamBindingInfo.CreateLiteral(BindToParamName,
                    ParamType.Int, (int)boxedValue);
                return true;
            }

            if (valueType == typeof(bool))
            {
                var rawValue = (bool)boxedValue ? 1 : 0;
                paramBindingInfo = ParamBindingInfo.CreateLiteral(BindToParamName,
                    ParamType.Bool, rawValue);
                return true;
            }

            throw new ArgumentException($"Unsupported value type: {valueType.AssemblyQualifiedName}.",
                nameof(valueType));
        }
    }
}
