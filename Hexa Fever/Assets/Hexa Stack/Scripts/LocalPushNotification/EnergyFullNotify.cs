using System;
using Unity.Notifications.Android;

namespace Tag.HexaStack
{
    public class EnergyFullNotify : BaseLocalNotification
    {
        #region PRIVATE_VARS
        #endregion

        #region PUBLIC_VARS
        #endregion

        #region Propertices
        #endregion

        #region Overrided_Method
        public override void ScheduleNotification()
        {
            if (CanSendNotification() && GetFireTime() > 0)
            {
                AndroidNotificationCenter.SendNotification(GetAndroidNotification(), channelId);
            }
        }

        public override AndroidNotification GetAndroidNotification()
        {
            AndroidNotification notification = base.GetAndroidNotification();
            notification.FireTime = TimeManager.Now.AddSeconds(GetFireTime());
            return notification;
        }

        public override double GetFireTime()
        {
            int remainEnergy = 0;
            CurrencyTimeBase energy = (CurrencyTimeBase)DataManager.Instance.GetCurrency(CurrencyConstant.ENERGY);
            if (energy.Value < energy.defaultValue)
            {
                remainEnergy = energy.defaultValue - energy.Value;

                return new TimeSpan(0, 0, remainEnergy * energy.unitTimeUpdate).TotalSeconds;
            }

            return 0;
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