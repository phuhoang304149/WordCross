using System;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR

using Unity.Notifications.Android;

#endif

public static class NotificationManager
{
    public const string ChannelId = "default";

    /// <summary>
    /// Schedule notification with app icon.
    /// </summary>
    public static int SendWithAppIcon(TimeSpan delay, string title, string message, Color smallIconColor)
    {
        int id = UnityEngine.Random.Range(0, int.MaxValue);

        #if UNITY_ANDROID && !UNITY_EDITOR

        var notification = new AndroidNotification(title, message, DateTime.Now.Add(delay));
        notification.Color = smallIconColor;
        notification.LargeIcon = "app_icon";
        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, ChannelId, id);

        #endif

        return id;
    }

    /// <summary>
    /// Cancel all notifications.
    /// </summary>
    public static void CancelAll()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR

        AndroidNotificationCenter.CancelAllNotifications();

        #endif
    }
}
