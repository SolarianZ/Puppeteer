using System;
using UPort = UnityEditor.Experimental.GraphView.Port;

namespace GBG.AnimationGraph.Editor
{
    public static class PortBridge
    {
        public static void AddOnConnectListener(this UPort port, Action<UPort> onConnect)
        {
            port.OnConnect += onConnect;
        }

        public static void RemoveOnConnectListener(this UPort port, Action<UPort> onConnect)
        {
            port.OnConnect -= onConnect;
        }

        public static void AddOnDisconnectListener(this UPort port, Action<UPort> onDisconnect)
        {
            port.OnDisconnect += onDisconnect;
        }

        public static void RemoveOnDisconnectListener(this UPort port, Action<UPort> onDisconnect)
        {
            port.OnDisconnect -= onDisconnect;
        }
    }
}
