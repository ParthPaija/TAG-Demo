using System;
using UnityEngine;
using Unity.Notifications.Android;

namespace Tag.HexaStack
{
    public class UserGoneNotify : BaseLocalNotification
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private int time;

        #endregion

        #region Propertices

        #endregion

        #region Overrided_Method

        public override AndroidNotification GetAndroidNotification()
        {
            DateTime today = TimeManager.Now.Date;
            DateTime notificationTime = today.AddHours(time);

            if (TimeManager.Now >= notificationTime)
            {
                notificationTime = notificationTime.AddDays(1);
            }
            AndroidNotification notification = base.GetAndroidNotification();
            notification.FireTime = notificationTime;
            notification.RepeatInterval = new TimeSpan(24, 0, 0);
            return notification;
        }

        public override double GetFireTime()
        {
            DateTime today = TimeManager.Now.Date;
            DateTime notificationTime = today.AddHours(time);

            if (TimeManager.Now >= notificationTime)
            {
                notificationTime = notificationTime.AddDays(1);
            }

            return notificationTime.Subtract(TimeManager.Now).TotalSeconds;
        }

        public override void ScheduleNotification()
        {
            if (CanSendNotification() && GetFireTime() > 0)
                AndroidNotificationCenter.SendNotification(GetAndroidNotification(), channelId);
        }

        #endregion

        #region UNITY_CALLBACKS
        #endregion

        #region PUBLIC_FUNCTIONS
        #endregion

        #region PRIVATE_FUNCTIONS
        #endregion

        #region CO-ROUTINES
        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region UI_CALLBACKS
        #endregion

    }
}