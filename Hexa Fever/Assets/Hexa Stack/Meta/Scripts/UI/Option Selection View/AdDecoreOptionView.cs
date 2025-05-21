using System;
using Tag.CommonPurchaseSystem;

namespace Tag.MetaGame
{
    public class AdDecoreOptionView : BaseDecoreOptionView
    {
        #region PUBLIC_VARS

        public override PurchaseType PrefabType
        {
            get { return PurchaseType.Ad; }
        }

        #endregion

        #region PRIVATE_VARS

        //private BufferingScreen BufferingScreen { get { return CustomBehaviour.FindObjFromLibrary<BufferingScreen>(); } }
        //private InternetChecker InternetChecker { get { return CustomBehaviour.FindObjFromLibrary<InternetChecker>(); } }
        //private AdManager AdManager { get { return CustomBehaviour.FindObjFromLibrary<AdManager>(); } }

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void BuyOption(Action onSuccessAction)
        {
            //BufferingScreen.ShowView();
            //InternetChecker.IsInternetAvailable(delegate
            //{
            //    RewardAdData rewardAdData = new RewardAdData
            //    {
            //        rewardAdPlace = RewardAdPlaceType.BUY_DECORE_OPTION,
            //        adCompletedAction = SuccessResponse
            //    };
            //    AdManager.ShowRewardAd(rewardAdData);
            //});
            void SuccessResponse()
            {
                onSuccessAction.Invoke();
            }
        }

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
