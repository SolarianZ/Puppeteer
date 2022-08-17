using UnityEngine;
using UnityEngine.Playables;

namespace GBG.Puppeteer
{
    public static class PlayableOutputExtensionsSpecialized
    {
        public static bool IsOutputNull(PlayableOutput output)
        {
            return output.IsOutputNull();
        }

        public static bool IsOutputValid(PlayableOutput output)
        {
            return output.IsOutputValid();
        }

        public static Object GetReferenceObject(PlayableOutput output)
        {
            return output.GetReferenceObject();
        }

        public static void SetReferenceObject(PlayableOutput output, Object value)
        {
            output.SetReferenceObject(value);
        }

        public static Object GetUserData(PlayableOutput output)
        {
            return output.GetUserData();
        }

        public static void SetUserData(PlayableOutput output, Object value)
        {
            output.SetUserData(value);
        }

        public static Playable GetSourcePlayable(PlayableOutput output)
        {
            return output.GetSourcePlayable();
        }

        public static void SetSourcePlayable(PlayableOutput output, Playable value)
        {
            output.SetSourcePlayable(value);
        }

        public static void SetSourcePlayable(PlayableOutput output, Playable value, int port)
        {
            output.SetSourcePlayable(value, port);
        }

        public static int GetSourceOutputPort(PlayableOutput output)
        {
            return output.GetSourceOutputPort();
        }

        public static float GetWeight(PlayableOutput output)
        {
            return output.GetWeight();
        }

        public static void SetWeight(PlayableOutput output, float value)
        {
            output.SetWeight(value);
        }

        public static void PushNotification(PlayableOutput output, Playable origin, INotification notification, object context = null)
        {
            output.PushNotification(origin, notification, context);
        }

        public static INotificationReceiver[] GetNotificationReceivers(PlayableOutput output)
        {
            return output.GetNotificationReceivers();
        }

        public static void AddNotificationReceiver(PlayableOutput output, INotificationReceiver receiver)
        {
            output.AddNotificationReceiver(receiver);
        }

        public static void RemoveNotificationReceiver(PlayableOutput output, INotificationReceiver receiver)
        {
            output.RemoveNotificationReceiver(receiver);
        }
    }
}