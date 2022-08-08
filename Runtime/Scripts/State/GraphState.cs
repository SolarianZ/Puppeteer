namespace GBG.Puppeteer
{
    public interface IGraphState
    {
        string Name { get; }

        int NameHash { get; }

        float Weight { get; }

        float PlaybackSpeed { get; }


        void SetFloatParam(string paramName, float paramValue);
    }

    public class GraphState
    {

    }
}
