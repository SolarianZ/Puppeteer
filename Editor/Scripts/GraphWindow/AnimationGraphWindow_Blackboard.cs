using System.Collections.Generic;
using GBG.Puppeteer.Parameter;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.Puppeteer.Editor.GraphWindow
{
    public partial class AnimationGraphWindow
    {
        private readonly List<ParamInfo> _paramInfos = new List<ParamInfo>();

        private VisualElement _blackboardPanel;


        private void CreateBlackboardPanel()
        {
            _blackboardPanel = new VisualElement
            {
                name = "blackboard-panel",
                style =
                {
                    width = 300,
                    height = Length.Percent(100),
                    borderRightWidth = 1,
                    borderRightColor = Color.black
                }
            };
            _layoutContainer.Add(_blackboardPanel);
        }
    }
}
