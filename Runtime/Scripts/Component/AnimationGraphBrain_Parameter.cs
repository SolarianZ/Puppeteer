using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;

namespace GBG.AnimationGraph
{
    public partial class AnimationGraphBrain
    {
        private readonly Dictionary<string, ParamInfo> _paramNameTable = new();


        public float GetFloat(string paramName)
        {
            return _paramNameTable[paramName].GetFloat();
        }

        public int GetInt(string paramName)
        {
            return _paramNameTable[paramName].GetInt();
        }

        public bool GetBool(string paramName)
        {
            return _paramNameTable[paramName].GetBool();
        }

        public float GetRawValue(string paramName)
        {
            return _paramNameTable[paramName].RawValue;
        }

        public void SetFloat(string paramName, float value)
        {
            _paramNameTable[paramName].SetFloat(value);
        }

        public void SetInt(string paramName, int value)
        {
            _paramNameTable[paramName].SetInt(value);
        }

        public void SetBool(string paramName, bool value)
        {
            _paramNameTable[paramName].SetBool(value);
        }

        public void SetRawValue(string paramName, float value)
        {
            _paramNameTable[paramName].SetRawValue(value);
        }
    }
}
