﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.FirebasePushNotification.Abstractions;

namespace Plugin.FirebasePushNotification
{
    [BroadcastReceiver]
    public class PushNotificationActionReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            var extras = intent.Extras;

            if (extras != null && !extras.IsEmpty)
            {
                foreach (var key in extras.KeySet())
                {
                    parameters.Add(key, $"{extras.Get(key)}");
                    System.Diagnostics.Debug.WriteLine(key, $"{extras.Get(key)}");
                }
            }

            CrossFirebasePushNotification.Current.NotificationHandler?.OnReceived(parameters);
            FirebasePushNotificationManager.RegisterData(parameters);

            NotificationManager manager = context.GetSystemService(Context.NotificationService) as NotificationManager;
            var notificationId = extras.GetInt(DefaultPushNotificationHandler.ActionNotificationIdKey, -1);
            if (notificationId != -1)
            {
                var notificationTag = extras.GetString(DefaultPushNotificationHandler.ActionNotificationTagKey, string.Empty);

                if (notificationTag == null)
                    manager.Cancel(notificationId);
                else
                    manager.Cancel(notificationTag, notificationId);

            }
            /*Intent resultIntent = context.PackageManager.GetLaunchIntentForPackage(context.PackageName);
            resultIntent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(resultIntent);*/

            context.UnregisterReceiver(FirebasePushNotificationManager.ActionReceiver);
            FirebasePushNotificationManager.ActionReceiver = null;
        }
    }
}