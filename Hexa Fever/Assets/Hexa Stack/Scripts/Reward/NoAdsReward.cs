using UnityEngine;
using System.Collections.Generic;

namespace Tag.HexaStack
{
    public class NoAdsReward : BaseReward
    {
        public int noOfMinutes;

        public override void GiveReward()
        {
            var playerData = DataManager.PlayerData;
            playerData.AddNoAdsPurchase(noOfMinutes);
            DataManager.Instance.SaveData(playerData);
        }

        public override int GetAmount()
        {
            return noOfMinutes;
        }

        public override RewardType GetRewardType()
        {
            return RewardType.NoAds;
        }

        public override string GetAmountStringForUI()
        {
            int days = noOfMinutes / (60 * 24);
            int remainingMinutesAfterDays = noOfMinutes % (60 * 24);
            int hours = remainingMinutesAfterDays / 60;
            int minutes = remainingMinutesAfterDays % 60;

            // Build the display string in a flexible way:
            List<string> parts = new List<string>();
            if (days > 0)
                parts.Add($"{days}d");
            if (hours > 0)
                parts.Add($"{hours}h");
            if (minutes > 0)
                parts.Add($"{minutes}m");

            // If there's no time at all, just show 0m
            if (parts.Count == 0)
                parts.Add("0m");

            return string.Join(" ", parts);
        }

        public override void AddRewardValue(int amount)
        {
            noOfMinutes += amount;
        }

        public override Sprite GetRewardImageSprite()
        {
            return ResourceManager.Instance.noAdsSprite;
        }

        public override string GetName()
        {
            return "Remove Ads";
        }
    }
}
