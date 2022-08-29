using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace GBG.Puppeteer.Parameter
{
    // TODO: Move to Editor
    public enum ParamSource
    {
        Literal,

        Variable
    }

    public enum ParamType
    {
        Float,

        Int,

        Bool,

        Any,
    }

    [Serializable]
    public class ParamInfo
    {
        /// <summary>
        /// Unique name of a variable.
        /// For a literal, its name is empty.
        /// </summary>
        public string Name => _name;

        [SerializeField]
        private string _name;


        public ParamType Type => _type;

        [SerializeField]
        private ParamType _type;


        [SerializeField]
        private float _value;


        public event Action<ParamInfo> OnValueChanged;


        public ParamInfo(string name, ParamType type, float value = 0)
        {
            // TODO: Check name rule

            _name = name;
            _type = type;
            _value = value;
        }

        public void SetFloat(float value)
        {
            Assert.IsTrue(Type == ParamType.Float || Type == ParamType.Any);

            _value = value;
            OnValueChanged?.Invoke(this);
        }

        public float GetFloat()
        {
            Assert.IsTrue(Type == ParamType.Float || Type == ParamType.Any);

            return _value;
        }

        public void SetInt(int value)
        {
            Assert.IsTrue(Type == ParamType.Int || Type == ParamType.Any);

            _value = value;
            OnValueChanged?.Invoke(this);
        }

        public int GetInt()
        {
            Assert.IsTrue(Type == ParamType.Int || Type == ParamType.Any);

            return (int)Math.Round(_value);
        }

        public void SetBool(bool value)
        {
            Assert.IsTrue(Type == ParamType.Bool || Type == ParamType.Any);

            _value = value ? 1 : 0;
            OnValueChanged?.Invoke(this);
        }

        public bool GetBool()
        {
            Assert.IsTrue(Type == ParamType.Bool || Type == ParamType.Any);

            return Mathf.Approximately(_value, 1);
        }
    }
}