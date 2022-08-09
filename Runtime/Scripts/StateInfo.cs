namespace GBG.Puppeteer
{
    public readonly struct StateInfo
    {
        public readonly bool IsValid;

        public readonly string LayerName;

        public readonly string StateName;

        public readonly float NormalizedTime;

        public readonly float ClipLength;

        public readonly float PlaybackSpeed;

        public readonly float Weight;


        public StateInfo(string layerName, string stateName,
            float normalizedTime, float clipLength,
            float playbackSpeed, float weight)
        {
            LayerName = layerName;
            StateName = stateName;
            NormalizedTime = normalizedTime;
            ClipLength = clipLength;
            PlaybackSpeed = playbackSpeed;
            Weight = weight;
            IsValid = true;
        }
    }
}