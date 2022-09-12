using System;
using System.Collections.Generic;
using GBG.Puppeteer.Editor.GraphParam;
using GBG.Puppeteer.Parameter;

namespace GBG.Puppeteer.Editor.Utility
{
    public static class ParamBindingTool
    {
        public static ParamField CreateParamBindingField(string bindToParamName, ParamType paramType,
            float inputLabelWidth)
        {
            switch (paramType)
            {
                case ParamType.Float:
                    return new ParamBindingField<float>(bindToParamName, inputLabelWidth);

                case ParamType.Int:
                    return new ParamBindingField<int>(bindToParamName, inputLabelWidth);

                case ParamType.Bool:
                    return new ParamBindingField<bool>(bindToParamName, inputLabelWidth);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static bool TryGetParamBindingSource(IEnumerable<ParamBindingNameOrValue> bindings,
            string bindToParamName, out ParamBindingNameOrValue binding)
        {
            foreach (var paramBindingNameOrValue in bindings)
            {
                if (paramBindingNameOrValue.BindToName.Equals(bindToParamName))
                {
                    binding = paramBindingNameOrValue;
                    return true;
                }
            }

            binding = default;
            return false;
        }
    }
}