using UnityEngine;

namespace Tag.CommonPurchaseSystem
{
    [CreateAssetMenu(fileName = "CurrencyPurchaseData", menuName = "Scriptable Objects/CommonPurchase/CurrencyPurchaseData")]
    public class CurrencyPurchaseDataSO : BasePurchaseDataSO
    {
        public override PurchaseType PurchaseType { get { return PurchaseType.Currency; } }
        //public RewardData currencyData;

        public override void CopyFrom(BasePurchaseDataSO purchaseDataSO)
        {
            CurrencyPurchaseDataSO currencyPurchaseDataSO = purchaseDataSO as CurrencyPurchaseDataSO;
            if (currencyPurchaseDataSO != null)
            {
                //currencyData = new RewardData();
                //currencyData.rewardId = new YondaimeFramework.ComponentId { stringId = currencyPurchaseDataSO.currencyData.rewardId.stringId };
                //currencyData.quantity = currencyPurchaseDataSO.currencyData.quantity;
            }
        }
    }
}