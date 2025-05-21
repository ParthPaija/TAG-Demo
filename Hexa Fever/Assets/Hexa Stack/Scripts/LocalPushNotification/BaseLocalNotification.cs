using UnityEngine;
using Unity.Notifications.Android;
using System.Collections.Generic;

namespace Tag.HexaStack
{
    public abstract class BaseLocalNotification
    {
        #region PRIVATE_VARS

        [SerializeField] public string channelId;
        private bool IsEnable
        {
            get => true;
        }

        #endregion

        #region PUBLIC_VARS
        public NotificationData notificationData = new NotificationData();
        #endregion

        #region Propertices
        #endregion

        #region Virtual_Method
        public virtual AndroidNotification GetAndroidNotification()
        {
            return new AndroidNotification()
            {
                Title = notificationData.title,
                Text = notificationData.GetText(),
                SmallIcon = "small_icon",
                LargeIcon = "large_icon",
                ShowTimestamp = false,
            };
        }

        public virtual bool CanSendNotification()
        {
            return IsEnable;
        }

        public abstract double GetFireTime();

        public abstract void ScheduleNotification();

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

[System.Serializable]
public class NotificationData
{
    public string title;
    public string text;

    public virtual string GetText()
    {
        return text;
    }
}

public class RendomTextNotificationData : NotificationData
{
    public List<string> textList = new List<string>();

    public override string GetText()
    {
        text = textList[Random.Range(0, textList.Count)];
        return base.GetText();
    }
}