using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace GBG.Puppeteer.Parameter
{
    public enum ParamType
    {
        Float,

        Int,

        Bool,
    }

    [Serializable]
    public class ParamInfo : ICloneable
    {
        public string Guid => _guid;

        [SerializeField]
        private string _guid;


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
        private float _rawValue;


        public bool IsLiteral => string.IsNullOrEmpty(Name);

        public event Action<ParamInfo> OnValueChanged;


        public ParamInfo(string name, ParamType type, float rawValue = 0)
        {
            _name = name;
            _type = type;
            _rawValue = rawValue;
        }

        public ParamInfo(string name, Type valueType, float rawValue = 0)
        {
            _name = name;
            _rawValue = rawValue;

            if (valueType == typeof(float))
            {
                _type = ParamType.Float;
            }
            else if (valueType == typeof(int))
            {
                _type = ParamType.Int;
            }
            else if (valueType == typeof(bool))
            {
                _type = ParamType.Bool;
            }
            else
            {
                throw new ArgumentException($"Unsupported value type: {valueType.AssemblyQualifiedName}.",
                    nameof(valueType));
            }
        }

        public void SetFloat(float value)
        {
            Assert.IsFalse(IsLiteral);
            Assert.IsTrue(Type == ParamType.Float);

            if (!Mathf.Approximately(_rawValue, value))
            {
                _rawValue = value;
                OnValueChanged?.Invoke(this);
            }
        }

        public float GetFloat()
        {
            Assert.IsTrue(Type == ParamType.Float);

            return _rawValue;
        }

        public void SetInt(int value)
        {
            Assert.IsFalse(IsLiteral);
            Assert.IsTrue(Type == ParamType.Int);

            if (!Mathf.Approximately(_rawValue, value))
            {
                _rawValue = value;
                OnValueChanged?.Invoke(this);
            }
        }

        public int GetInt()
        {
            Assert.IsTrue(Type == ParamType.Int);

            return (int)Math.Round(_rawValue);
        }

        public void SetBool(bool value)
        {
            Assert.IsFalse(IsLiteral);
            Assert.IsTrue(Type == ParamType.Bool);

            var newValue = value ? 1 : 0;
            if (!Mathf.Approximately(_rawValue, newValue))
            {
                _rawValue = newValue;
                OnValueChanged?.Invoke(this);
            }
        }

        public bool GetBool()
        {
            Assert.IsTrue(Type == ParamType.Bool);

            return !Mathf.Approximately(_rawValue, 0);
        }

        public void SetRawValue(float value)
        {
            Assert.IsFalse(IsLiteral);

            if (!Mathf.Approximately(_rawValue, value))
            {
                _rawValue = value;
                OnValueChanged?.Invoke(this);
            }
        }

        public float GetRawValue()
        {
            return _rawValue;
        }

        public object Clone()
        {
            return new ParamInfo(_name, _type, _rawValue);
        }

        public static ParamInfo CreateLiteral(ParamType type, float rawValue)
        {
            return new ParamInfo(null, type, rawValue);
        }


#if UNITY_EDITOR
        public event Action<ParamInfo> EditorOnNameChanged;

        public void EditorSetName(string name)
        {
            if (name != _name)
            {
                _name = name;
                EditorOnNameChanged?.Invoke(this);
            }
        }
#endif
    }
}
