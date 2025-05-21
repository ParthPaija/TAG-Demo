using UnityEngine;

namespace Tag.HexaStack
{
    public static class Constant
    {
        public const bool IsProductionBuid = true;
        public const bool CanByPassIAP = false;

        public const string GAME_NAME = "Hexa Stack";
        public const string PLAYER_DATA = "PlayerData";

        public static bool IsAdOn = true;

        private const string BuildVersionCodeFormat = "v{0} ({1})";

        public static string BuildVersionCodeString => string.Format(BuildVersionCodeFormat, Application.version, VersionCodeFetcher.GetBundleVersionCode());
    }

    public static class GamePlayConstant
    {
        public const float TWO_ITEM_DISTANCE = 0.20f;
    }

    public static class EditorCosntant
    {
        public const string MAPPING_IDS_PATH = "Assets/Hexa Stack/Data/IdMappings";
    }

    public static class CurrencyConstant
    {
        public const int COINS = 1;
        public const int ENERGY = 2;
        public const int HAMMER_BOOSTER = 3;
        public const int REFRESH_BOOSTER = 4;
        public const int SWAP_BOOSTER = 5;
        public const int META_CURRENCY = 6;
        public const int UNDO_BOOSTER = 7;
    }

    public static class SpawnAlgoConstant
    {
        public const int lessTileCount = 3;
        public const int winStreakCount = 3;
        public const int loseStreakCount = 2;
        public const int forceEasyModeLevel = 10;
    }

    public class UserPromptMessageConstants
    {
        public const string All_Levels_Completed_Header = "Levels Completed";
        public const string All_Levels_Completed_Mesasge = "Well done! You have completed all the Levels. We will be adding more levels to the game soon. Stay tuned for future updates.";

        public const string PurchaseFailedMessage = "Purchase canceled! Please try again later.";
        public const string PurchaseSuccessMessage = "Purchase success !";
        public const string ConnectingToStoreMessage = "Connecting to Store !";

        public const string NoAdsPurchaseSuccess = "No ads pack purchased successfully!";
        public const string NoAdsAlreadyPurchase = "No ads pack already purchased and active!";

        public const string NotEnoughCoins = "Not enough coins !";

        public const string RateUsDoneMessage = "Thank you !";

        public const string RewardedAdLoadingMessage = "The video ad will play soon";
        public const string RewardedNotAvailableMessage = "No video ads available\nPlease try again later.";
        public const string NoInternetConnection = "Please check your internet and try again.";

        public const string CantUseExtraBoltBoosterMessage = "Bolt cannot be extended anymore!";
        public const string CantUseUndoBoosterMessage = "No moves to undo!";

        public const string NextLeaderboardEventMessage = "Next Leaderboard Event Starts In : ";
    }
}
