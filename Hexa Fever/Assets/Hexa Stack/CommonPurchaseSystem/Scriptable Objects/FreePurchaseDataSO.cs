using UnityEngine;

namespace Tag.CommonPurchaseSystem
{
    [CreateAssetMenu(fileName = "FreePurchaseData", menuName = "Scriptable Objects/CommonPurchase/FreePurchaseData")]
    public class FreePurchaseDataSO : BasePurchaseDataSO
    {
        public override PurchaseType PurchaseType { get { return PurchaseType.Free; } }

        public override void CopyFrom(BasePurchaseDataSO purchaseDataSO)
        {
            FreePurchaseDataSO freePurchaseDataSO = purchaseDataSO as FreePurchaseDataSO;
            if (freePurchaseDataSO != null)
            {
            }
        }
    }
}