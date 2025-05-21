using UnityEngine;

namespace Tag.CommonPurchaseSystem
{
    public abstract class BasePurchaseDataSO : ScriptableObject
    {
        public abstract PurchaseType PurchaseType { get; }

        public abstract void CopyFrom(BasePurchaseDataSO purchaseDataSO);
    }
}