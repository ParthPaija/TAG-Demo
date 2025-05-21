using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine.Android;

namespace Tag.HexaStack
{
    public class LocalPushNotificationManager : SerializedManager<LocalPushNotificationManager>
    {
        #region PRIVATE_VARS

        public List<BaseLocalNotification> localNotifications = new List<BaseLocalNotification>();
        private AndroidNotificationChannel notificationChannel;

        #endregion

        #region PUBLIC_VARS

        #endregion

        #region Propertices

        private bool IsLocalNotificationEnable => true;
        private bool IsNotificationRequestPermissionPopUpEnable => true;

        #endregion

        #region Overrided_Method
        #endregion

        #region UNITY_CALLBACKS

        public void Start()
        {
            if (IsNotificationRequestPermissionPopUpEnable)
                CheckAndRequestPermission();
            ClearAllNotificationData();
        }

        public void OnApplicationPause(bool pause)
        {
            if (IsLocalNotificationEnable)
                ScheduleNotification(pause);
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void ScheduleNotification(bool pause)
        {
            //if (!MainSceneUIManager.Instance || !IsLocalNotificationEnable)
            //    return;

            //if (pause)
            //{
            //    for (int i = 0; i < localNotifications.Count; i++)
            //    {
            //        CreateChannel(localNotifications[i].channelId);
            //        localNotifications[i].ScheduleNotification();
            //    }
            //}
            //else
            //{
            //    ClearAllNotificationData();
            //}
        }

        public void ClearAllNotificationData()
        {
            //AndroidNotificationCenter.CancelAllNotifications();
            //for (int i = 0; i < localNotifications.Count; i++)
            //{
            //    AndroidNotificationCenter.DeleteNotificationChannel(localNotifications[i].channelId);
            //}
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void CreateChannel(string channelId)
        {
            //notificationChannel = GetNotificationChannelSetting(channelId);
            //AndroidNotificationCenter.RegisterNotificationChannel(notificationChannel);
        }

        private AndroidNotificationChannel GetNotificationChannelSetting(string channelId)
        {
            AndroidNotificationChannel notificationChannel = new AndroidNotificationChannel()
            {
                Id = channelId,
                Name = channelId + " channel",
                Description = "Hexa Fever notification",
                EnableVibration = true,
                LockScreenVisibility = LockScreenVisibility.Public,
                Importance = Importance.High,
            };
            return notificationChannel;
        }


        void CheckAndRequestPermission()
        {
            //if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            //{
            //    Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            //}
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        #endregion
    }
}