namespace GBG.Puppeteer
{
    public readonly struct StateInfo
    {
        public readonly string LayerName;

        public readonly string StateName;

        public readonly bool IsPlaying;

        public readonly float Time;

        public readonly float ClipLength;

        public readonly float PlaybackSpeed;

        //public readonly float Weight;

        private readonly bool _isValid;


        internal StateInfo(string layerName, string stateName,
           bool isPlaying, float time, float clipLength,
            float playbackSpeed)
        {
            LayerName = layerName;
            StateName = stateName;
            IsPlaying = isPlaying;
            Time = time;
            ClipLength = clipLength;
            PlaybackSpeed = playbackSpeed;
            _isValid = true;
        }

        internal StateInfo(string layerName, GraphClipState state)
        {
            LayerName = layerName;
            StateName = state.StateName;
            IsPlaying = state.IsPlaying;
            Time = state.Time;
            ClipLength = state.Clip ? state.Clip.length : 0;
            PlaybackSpeed = state.PlaybackSpeed;
            _isValid = true;
        }

        public bool IsValid()
        {
            return _isValid;
        }
    }
}