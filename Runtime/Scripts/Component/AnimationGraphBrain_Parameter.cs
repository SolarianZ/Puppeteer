namespace GBG.AnimationGraph
{
    public partial class AnimationGraphBrain
    {
        public float GetFloat(string paramName)
        {
            return _graphAsset.GetFloat(paramName);
        }

        public int GetInt(string paramName)
        {
            return _graphAsset.GetInt(paramName);
        }

        public bool GetBool(string paramName)
        {
            return _graphAsset.GetBool(paramName);
        }

        public float GetRawValue(string paramName)
        {
            return _graphAsset.GetRawValue(paramName);
        }

        public void SetFloat(string paramName, float value)
        {
            _graphAsset.SetFloat(paramName, value);
        }

        public void SetInt(string paramName, int value)
        {
            _graphAsset.SetInt(paramName, value);
        }

        public void SetBool(string paramName, bool value)
        {
            _graphAsset.SetBool(paramName, value);
        }

        public void SetRawValue(string paramName, float value)
        {
            _graphAsset.SetRawValue(paramName, value);
        }
    }
}
