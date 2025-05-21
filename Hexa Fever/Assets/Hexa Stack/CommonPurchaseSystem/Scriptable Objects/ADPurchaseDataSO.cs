using UnityEngine;

namespace Tag.CommonPurchaseSystem
{
    [CreateAssetMenu(fileName = "ADPurchaseData", menuName = "Scriptable Objects/CommonPurchase/ADPurchaseData")]
    public class ADPurchaseDataSO : BasePurchaseDataSO
    {
        public override PurchaseType PurchaseType { get { return PurchaseType.Ad; } }

        public override void CopyFrom(BasePurchaseDataSO purchaseDataSO)
        {
            ADPurchaseDataSO adPurchaseDataSO = purchaseDataSO as ADPurchaseDataSO;
            if (adPurchaseDataSO != null)
            {
            }
        }
    }
}
