using I2.Loc;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class VIPLeaderboardView : BaseView
    {
        #region PUBLIC_VARIABLES

        public List<Sprite> RankImages => rankImages;
        public List<Sprite> GiftboxImages => giftboxImages;
        public LeaderboardPlayerScoreInfoUITheme InactiveTheme => inactiveTheme;
        public LeaderboardPlayerScoreInfoUITheme ActiveTheme => activeTheme;

        #endregion

        #region PRIVATE_VARIABLES

        [SerializeField] private RectTransform timerParent;
        [SerializeField] private RectTransform timerAndMessageParent;
        [SerializeField] private Text leaderBoardTimerText;

        [Space]
        [SerializeField] private LeaderboardPlayerScoreInfoUITheme activeTheme;
        [SerializeField] private LeaderboardPlayerScoreInfoUITheme inactiveTheme;
        [SerializeField] private List<Sprite> rankImages;
        [SerializeField] private List<Sprite> giftboxImages;
        [SerializeField] private RectTransform topMostScrollPos;
        [SerializeField] private RectTransform botMostScrollPos;

        [Space]
        [SerializeField] private VIPLeaderboardPlayerScoreInfo leaderboardPlayerScoreInfoPrefab;
        [SerializeField] private ScrollRect leaderboardPlayersListScroll;

        [SerializeField] private VIPLeaderboardPlayerScoreInfo myPlayerScoreCardInfo;
        [SerializeField] private RectTransform followerScrollPos;
        [SerializeField] private ReusableVerticalScrollView reusableVerticalScrollView;
        [SerializeField] private RectTransformFollower rectTransformFollower;


        [Space(30), Header("Complate Leaderboard")]

        [SerializeField] private GameObject[] onLeaderboardComplate;
        [SerializeField] private Text playerRankText;
        [SerializeField] private Localize cliamButtonText;
        [SerializeField] private Text noRewardTextText;
        [SerializeField] private Button cliamButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Dictionary<int, LeaderboardPlayerRewardInfoUITheme> rewardTheams = new Dictionary<int, LeaderboardPlayerRewardInfoUITheme>();

        private Action actionToCallOnHide;

        bool isEventResult = false;

        #endregion

        #region PROPERTIES
        #endregion

        #region UNITY_CALLBACKS

        [Button]
        public override void Awake()
        {
            base.Awake();
            rectTransformFollower.heightClamp = new Vector2(botMostScrollPos.position.y, topMostScrollPos.position.y);
        }

        private void OnEnable()
        {
            VIPLeaderboardManager.onLeaderboardEventRunTimerOver += LeaderboardManager_onLeaderboardEventRunTimerOver;
        }

        private void OnDisable()
        {
            VIPLeaderboardManager.onLeaderboardEventRunTimerOver -= LeaderboardManager_onLeaderboardEventRunTimerOver;
            UnregisterLeaderBoardTimer();
        }

        #endregion

        #region PUBLIC_METHODS

        public override void Show(Action action = null, bool isForceShow = false)
        {
            base.Show(action, isForceShow);
            RefreshView();
        }

        public void ShowWithHideAction(Action actionToCallOnHide)
        {
            this.actionToCallOnHide = actionToCallOnHide;
            Show();
        }

        public override void OnBackButtonPressed()
        {
        }

        public override void OnHideComplete()
        {
            base.OnHideComplete();

            actionToCallOnHide?.Invoke();
            actionToCallOnHide = null;
        }

        public override void Hide()
        {
            base.Hide();
        }

        #endregion

        #region PRIVATE_METHODS

        private void RefreshView()
        {
            IsEventResultRedayForShown();

            ResetViewInfos();

            var data = VIPLeaderboardManager.Instance.GetLeaderboardPlayerUIDatas();

            if (!isEventResult)
            {
                timerParent.gameObject.SetActive(true);

                RegisterLeaderBoardTimer();
                UpdateLeaderBoardTimer();
                closeButton.gameObject.SetActive(true);
            }
            else
            {
                SetLeaderBoardComplateView();
                for (int i = 0; i < onLeaderboardComplate.Length; i++)
                {
                    onLeaderboardComplate[i].SetActive(true);
                }
            }
            timerAndMessageParent.ForceUpdateRectTransforms();
            UIUtilityEvents.RaiseOnRefreshUIRects();
            SetView(data);
            ScrollToCurrentUser(data.Find(x => x.leaderboardPlayerType == LeaderboardPlayerType.UserPlayer).rank - 1);
        }

        private void IsEventResultRedayForShown()
        {
            var data = VIPLeaderboardManager.Instance.GetLeaderboardPlayerUIDatas();
            var myViewData = data.Find(x => x.leaderboardPlayerType == LeaderboardPlayerType.UserPlayer);
            isEventResult = VIPLeaderboardManager.Instance.IsEventResultRedayForShown();
            AnalyticsManager.Instance.LogGAEvent("VipLeagueEnded : " + data.Find(x => x.leaderboardPlayerType == LeaderboardPlayerType.UserPlayer).rank);
            AnalyticsManager.Instance.LogGAEvent("VipLeagueRoundsPlayed : " + PlayerPersistantData.GetVIPLeaderboardPlayerData().currentLevel);
        }

        private void ScrollToCurrentUser(int index)
        {
            Vector3 scrollPos = reusableVerticalScrollView.GetItemWorldPosition(index);
            leaderboardPlayersListScroll.ScrollToRect(scrollPos);
            reusableVerticalScrollView.RefreshVisibility();
        }

        private void ResetViewInfos()
        {
            myPlayerScoreCardInfo.gameObject.SetActive(false);
            timerParent.gameObject.SetActive(false);
            for (int i = 0; i < onLeaderboardComplate.Length; i++)
            {
                onLeaderboardComplate[i].SetActive(false);
            }
        }

        private void SetView(List<LeaderBoardPlayerScoreInfoUIData> leaderBoardPlayerScoreInfoUIDatas)
        {
            reusableVerticalScrollView.Initialize(leaderBoardPlayerScoreInfoUIDatas.Count, (gameObject, index) =>
            {
                VIPLeaderboardPlayerScoreInfo scoreInfo = gameObject.GetComponent<VIPLeaderboardPlayerScoreInfo>();
                scoreInfo.SetUI(leaderBoardPlayerScoreInfoUIDatas[index]);

                if (leaderBoardPlayerScoreInfoUIDatas[index].leaderboardPlayerType == LeaderboardPlayerType.UserPlayer)
                    scoreInfo.MainParent.gameObject.SetActive(false);
            });

            var myViewData = leaderBoardPlayerScoreInfoUIDatas.Find(x => x.leaderboardPlayerType == LeaderboardPlayerType.UserPlayer);
            Vector3 positionOfMyView = reusableVerticalScrollView.GetItemPosition(leaderBoardPlayerScoreInfoUIDatas.IndexOf(myViewData));
            followerScrollPos.anchoredPosition = positionOfMyView;
            myPlayerScoreCardInfo.SetUI(myViewData);
        }

        private void RegisterLeaderBoardTimer()
        {
            if (VIPLeaderboardManager.Instance.IsSystemInitialized && VIPLeaderboardManager.Instance.LeaderboardRunTimer != null)
                VIPLeaderboardManager.Instance.LeaderboardRunTimer.RegisterTimerTickEvent(UpdateLeaderBoardTimer);
        }

        private void UpdateLeaderBoardTimer()
        {
            leaderBoardTimerText.text = VIPLeaderboardManager.Instance.LeaderboardRunTimer.GetRemainingTimeSpan().ParseTimeSpan(2);
        }

        private void UnregisterLeaderBoardTimer()
        {
            if (VIPLeaderboardManager.Instance.IsSystemInitialized && VIPLeaderboardManager.Instance.LeaderboardRunTimer != null)
                VIPLeaderboardManager.Instance.LeaderboardRunTimer.UnregisterTimerTickEvent(UpdateLeaderBoardTimer);
        }

        private void LeaderboardManager_onLeaderboardEventRunTimerOver()
        {
            RefreshView();
        }

        private void PlayRewardClaimAnimation()
        {
            int playerRank = VIPLeaderboardManager.Instance.GetPlayerRank();

            var itemInfo = reusableVerticalScrollView.GetItemIfVisible(playerRank - 1);
            if (itemInfo == null)
                return;

            if (playerRank <= VIPLeaderboardManager.Max_Top_Rank)
            {
                MainSceneUIManager.Instance.GetView<CommonRewardFeedbackView>().ShowView(VIPLeaderboardManager.Instance.GetRankReward(playerRank).rewards, () =>
                {

                });
            }
        }

        [Button]
        private void SetLeaderBoardComplateView()
        {
            noRewardTextText.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(false);

            foreach (var item in rewardTheams)
            {
                item.Value.rewardImage.gameObject.SetActive(false);
                item.Value.rewardAmount.gameObject.SetActive(false);
            }

            var data = VIPLeaderboardManager.Instance.GetLeaderboardPlayerUIDatas();
            var myViewData = data.Find(x => x.leaderboardPlayerType == LeaderboardPlayerType.UserPlayer);

            if (myViewData.rank <= VIPLeaderboardManager.Max_Top_Rank)
            {
                BaseReward baseReward = VIPLeaderboardManager.Instance.GetRankReward(myViewData.rank).rewards[0];

                LeaderboardPlayerRewardInfoUITheme leaderboardPlayerRewardInfoUITheme;

                if (rewardTheams.ContainsKey(baseReward.GetCurrencyId()))
                {
                    leaderboardPlayerRewardInfoUITheme = rewardTheams[baseReward.GetCurrencyId()];
                }
                else
                {
                    leaderboardPlayerRewardInfoUITheme = rewardTheams[-1];
                }

                leaderboardPlayerRewardInfoUITheme.SetUI(baseReward);
                cliamButtonText.SetTerm("Claim");
            }
            else
            {
                cliamButtonText.SetTerm("Close");
                noRewardTextText.gameObject.SetActive(true);
            }
            playerRankText.text = $"#{myViewData.rank}";
        }

        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region COROUTINES
        #endregion

        #region UI_CALLBACKS

        public void OnCliamButtonClick()
        {
            if (isEventResult)
            {
                PlayRewardClaimAnimation();
                VIPLeaderboardManager.Instance.OnLeaderboardViewVisited();
                Hide();
            }
        }

        #endregion
    }

    public class LeaderBoardPlayerScoreInfoUIData
    {
        public int rank;
        public int score;
        public string name;
        public Sprite avtarSprite;
        public Sprite frameSprite;
        public LeaderboardPlayerType leaderboardPlayerType;

        public LeaderBoardPlayerScoreInfoUIData DeepCopy()
        {
            LeaderBoardPlayerScoreInfoUIData copy = new LeaderBoardPlayerScoreInfoUIData();
            copy.rank = rank;
            copy.score = score;
            copy.name = name;
            copy.leaderboardPlayerType = leaderboardPlayerType;
            return copy;
        }
    }
}