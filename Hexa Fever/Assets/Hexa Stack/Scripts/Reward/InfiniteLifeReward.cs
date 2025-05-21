using UnityEngine;

namespace Tag.HexaStack
{
    public class InfiniteLifeReward : BaseReward
    {
        [CurrencyId] public int currencyID;
        public int infiniteLifeInSecond;

        public override void GiveReward()
        {
            CurrencyTimeBase currencyTimeBase = (CurrencyTimeBase)DataManager.Instance.GetCurrency(currencyID);
            currencyTimeBase.ActivateInfiniteEnergy(infiniteLifeInSecond);
        }

        public override void RemoveReward()
        {
        }

        public override int GetAmount()
        {
            return infiniteLifeInSecond / 60;
        }

        public override string GetAmountStringForUI()
        {
            int totalSeconds = infiniteLifeInSecond;
            
            int hours = totalSeconds / 3600; // 3600 seconds = 1 hour
            totalSeconds %= 3600;
            
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            
            string result = "";
            
            if (hours > 0)
                result += $"{hours} Hours ";
                
            if (minutes > 0)
                result += $"{minutes} Min ";
                
            if (seconds > 0 || result == "")
                result += $"{seconds} Sec";
                
            return result.Trim();
        }

        public override RewardType GetRewardType()
        {
            return RewardType.InfiniteLife;
        }

        public override void AddRewardValue(int amount)
        {
        }

        public override int GetCurrencyId()
        {
            return currencyID;
        }

        public override Sprite GetRewardImageSprite()
        {
            return ResourceManager.Instance.GetCurrencySprite(currencyID);
        }

        public override string GetName()
        {
            return DataManager.Instance.GetCurrency(currencyID).currencyName;
        }
    }
}
