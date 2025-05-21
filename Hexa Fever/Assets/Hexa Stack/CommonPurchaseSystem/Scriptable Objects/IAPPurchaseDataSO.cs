using UnityEngine;

namespace Tag.CommonPurchaseSystem
{
    [CreateAssetMenu(fileName = "IAPPurchaseData", menuName = "Scriptable Objects/CommonPurchase/IAPPurchaseData")]
    public class IAPPurchaseDataSO : BasePurchaseDataSO
    {
        public string defaultPriceText;
        public override PurchaseType PurchaseType { get { return PurchaseType.IAP; } }
        [SerializeField] private string _packId;
        [SerializeField] private string iosPackId;
        //public ProductType productType;

        public string PackId
        {
            get
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    return iosPackId;
                }
                else
                {
                    return _packId;
                }

            }
        }
        public override void CopyFrom(BasePurchaseDataSO purchaseDataSO)
        {
            IAPPurchaseDataSO iapPurchaseDataSO = purchaseDataSO as IAPPurchaseDataSO;
            if (iapPurchaseDataSO != null)
            {
                _packId = iapPurchaseDataSO._packId;
                //productType = iapPurchaseDataSO.productType;
            }
        }

        #region PUBLIC_FUNCTIONS
        #endregion
    }
}