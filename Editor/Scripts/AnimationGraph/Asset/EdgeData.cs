using System;

namespace GBG.Puppeteer.Editor.AnimationGraph
{
    [Serializable]
    public class EdgeData
    {
        public string FromNodeGuid;

        public string FromPortGuid;

        public string ToNodeGuid;

        public string ToPortGuid;
    }
}