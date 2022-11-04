using System.Linq;
using GBG.AnimationGraph.Parameter;
using UnityEngine;

namespace GBG.AnimationGraph.Tests
{
    [RequireComponent(typeof(AnimationGraphBrain))]
    internal class AnimationGraphTester : MonoBehaviour
    {
        public AnimationGraphBrain Brain { get; private set; }

        public int ParamIndex { get; set; }

        public float ParamRawValue { get; set; }

        public bool LiveParamEnabled { get; set; }

        
        private string[] _paramNames;


        private void Start()
        {
            Brain = GetComponent<AnimationGraphBrain>();
        }

        private void Update()
        {
            if (LiveParamEnabled && ParamIndex > -1)
            {
                SetRawValue(GetParamNames()[ParamIndex], ParamRawValue);
            }
        }


        #region 参数

        public string[] GetParamNames()
        {
            if (_paramNames == null)
            {
                var parameters = Brain.EditorTest_GetParameters();
                _paramNames = (from param in parameters select param.Name).ToArray();
            }

            return _paramNames;
        }

        public ParamType GetParamType(int index)
        {
            var parameters = Brain.EditorTest_GetParameters();
            return parameters[index].Type;
        }

        public float GetFloat(string paramName)
        {
            return Brain.GetFloat(paramName);
        }

        public int GetInt(string paramName)
        {
            return Brain.GetInt(paramName);
        }

        public bool GetBool(string paramName)
        {
            return Brain.GetBool(paramName);
        }

        public float GetRawValue(string paramName)
        {
            return Brain.GetRawValue(paramName);
        }

        public void SetFloat(string paramName, float value)
        {
            Brain.SetFloat(paramName, value);
        }

        public void SetInt(string paramName, int value)
        {
            Brain.SetInt(paramName, value);
        }

        public void SetBool(string paramName, bool value)
        {
            Brain.SetBool(paramName, value);
        }

        public void SetRawValue(string paramName, float value)
        {
            Brain.SetRawValue(paramName, value);
        }

        #endregion
    }
}
