using System;
using Tag.CommonPurchaseSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.MetaGame
{
    public class CurrencyDecoreOptionView : BaseDecoreOptionView
    {
        #region PUBLIC_VARS

        public override PurchaseType PrefabType
        {
            get { return PurchaseType.Currency; }
        }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Image _currencyImage;
        [SerializeField] private Text _costText;
        //[SerializeField] private RewardData _costdata;

        //private SpriteHandler SpriteHandler { get { return CustomBehaviour.FindObjFromLibrary<SpriteHandler>(); } }
        //private DataManager DataManager { get { return CustomBehaviour.FindObjFromLibrary<DataManager>(); } }
        //private ShopView ShopView { get { return CustomBehaviour.FindObjFromLibrary<ShopView>(); } }
        
        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void SetView(int index, OptionData optionData, Action<int> onSelectAction)
        {
            base.SetView(index, optionData, onSelectAction);
            //_costdata = ((CurrencyPurchaseDataSO)optionData.basePurchaseDataSO).currencyData;
            //_costText.text = _costdata.quantity.ToString();
            //_currencyImage.sprite = SpriteHandler.GetItemSprite(_costdata.rewardId);
        }

        public override void BuyOption(Action onSuccessAction)
        {
            //DataManager.SubtractRewardValue(_costdata.rewardId.stringId, _costdata.quantity, true, OptionPurchaseResponse);

            void OptionPurchaseResponse(bool isSuccess)
            {
                if (isSuccess)
                {
                    onSuccessAction.Invoke();
                }
                else
                {
                    //ShopView.OpenPopupOnContent(_costdata.rewardId.stringId);
                }
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
