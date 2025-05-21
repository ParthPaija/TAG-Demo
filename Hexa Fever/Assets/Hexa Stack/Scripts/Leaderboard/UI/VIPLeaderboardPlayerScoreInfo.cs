using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class VIPLeaderboardPlayerScoreInfo : SerializedMonoBehaviour
    {
        #region PUBLIC_VARIABLES

        public RectTransform MainParent => mainParent;
        public RectTransform GiftBoxImage => mainParent;

        #endregion

        #region PRIVATE_VARIABLES

        [SerializeField] private RectTransform mainParent;
        [SerializeField] private RectTransform topRankParent;
        [SerializeField] private RectTransform belowRankParent;

        [SerializeField] private Text rankText;
        [SerializeField] private Image rankImage;

        [Space]
        [SerializeField] private Text playerName;
        [SerializeField] private Image avtarImage;
        [SerializeField] private Image frameImage;
        [SerializeField] private Text playerScore;

        [Space]
        [SerializeField] private Image mainBGImage;
        [SerializeField] private Image mainScoreBGImage;
        [SerializeField] private List<Outline> themeTexts;

        [Space(20), Header("Reward")]
        [SerializeField] private Dictionary<int, LeaderboardPlayerRewardInfoUITheme> rewardTheams = new Dictionary<int, LeaderboardPlayerRewardInfoUITheme>();

        private VIPLeaderboardView LeaderboardView => MainSceneUIManager.Instance.GetView<VIPLeaderboardView>();
        [SerializeField] private LeaderBoardPlayerScoreInfoUIData leaderBoardPlayerScoreInfoUIData;

        protected Coroutine rankSetCoroutine;
        private const string Rank_Format = "{0}.";

        #endregion

        #region PROPERTIES

        #endregion

        #region UNITY_CALLBACKS

        private void OnDisable()
        {
            rankSetCoroutine = null;
        }

        #endregion

        #region PUBLIC_METHODS

        public void SetUI(LeaderBoardPlayerScoreInfoUIData leaderBoardPlayerScoreInfoUIData)
        {
            ResetAllObjects();

            this.leaderBoardPlayerScoreInfoUIData = leaderBoardPlayerScoreInfoUIData;

            if (leaderBoardPlayerScoreInfoUIData.rank <= 3)
            {
                topRankParent.gameObject.SetActive(true);
                rankImage.sprite = LeaderboardView.RankImages[leaderBoardPlayerScoreInfoUIData.rank - 1];
            }
            else
            {
                belowRankParent.gameObject.SetActive(true);
                rankText.text = string.Format(Rank_Format, leaderBoardPlayerScoreInfoUIData.rank);
            }

            if (leaderBoardPlayerScoreInfoUIData.rank <= VIPLeaderboardManager.Max_Top_Rank)
            {
                BaseReward baseReward = VIPLeaderboardManager.Instance.GetRankReward(leaderBoardPlayerScoreInfoUIData.rank).rewards[0];
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
            }

            playerName.text = leaderBoardPlayerScoreInfoUIData.name;
            avtarImage.sprite = leaderBoardPlayerScoreInfoUIData.avtarSprite;
            frameImage.sprite = leaderBoardPlayerScoreInfoUIData.frameSprite;
            playerScore.text = leaderBoardPlayerScoreInfoUIData.score + "";

            bool isActiveUI = leaderBoardPlayerScoreInfoUIData.leaderboardPlayerType == LeaderboardPlayerType.UserPlayer;

            mainBGImage.sprite = (isActiveUI ? LeaderboardView.ActiveTheme : LeaderboardView.InactiveTheme).mainBGSprite;
            mainScoreBGImage.sprite = (isActiveUI ? LeaderboardView.ActiveTheme : LeaderboardView.InactiveTheme).mainScoreBGSprite;

            themeTexts.ForEach(x => x.effectColor = (isActiveUI ? LeaderboardView.ActiveTheme : LeaderboardView.InactiveTheme).themeColor);

            gameObject.SetActive(true);
        }

        public void ResetView()
        {
            ResetAllObjects();
            gameObject.SetActive(false);
        }

        public void AnimateRank(int from, int to, float animTime = 0.65f)
        {
            if (rankSetCoroutine == null)
                rankSetCoroutine = StartCoroutine(DoAnimateRankValueChange(animTime, from, to, rankText, Rank_Format));
        }

        #endregion

        #region PRIVATE_METHODS

        private void ResetAllObjects()
        {
            topRankParent.gameObject.SetActive(false);
            belowRankParent.gameObject.SetActive(false);
            mainParent.gameObject.SetActive(true);

            foreach (var item in rewardTheams)
            {
                item.Value.rewardImage.gameObject.SetActive(false);
                item.Value.rewardAmount.gameObject.SetActive(false);
            }
        }

        private void SetRankView(int rank)
        {
            topRankParent.gameObject.SetActive(false);
            belowRankParent.gameObject.SetActive(false);

            if (rank <= VIPLeaderboardManager.Max_Top_Rank)
            {
                topRankParent.gameObject.SetActive(true);
                rankImage.sprite = LeaderboardView.RankImages[rank - 1];
            }
            else
            {
                belowRankParent.gameObject.SetActive(true);
                rankText.text = string.Format(Rank_Format, rank);
            }
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region COROUTINES

        private IEnumerator DoAnimateRankValueChange(float time, int startValue, int targetValue, Text textComponent, string format = "{0}")
        {
            float i = 0;
            float rate = 1 / time;

            while (i < 1)
            {
                i += Time.deltaTime * rate;

                SetRankView((int)Mathf.Lerp(startValue, targetValue, i));
                yield return null;
            }
            SetRankView(targetValue);

            rankSetCoroutine = null;
        }

        #endregion

        #region UI_CALLBACKS

        #endregion
    }

    public class LeaderboardPlayerRewardInfoUITheme
    {
        public Image rewardImage;
        public Text rewardAmount;

        public virtual void SetUI(BaseReward baseReward)
        {
            rewardImage.gameObject.SetActive(true);
            rewardAmount.gameObject.SetActive(true);
            rewardImage.sprite = baseReward.GetRewardImageSprite();
            rewardAmount.text = "x" + baseReward.GetAmountStringForUI();
        }
    }

    public class LeaderboardPlayerCoinRewardInfoUITheme : LeaderboardPlayerRewardInfoUITheme
    {
        public override void SetUI(BaseReward baseReward)
        {
            rewardImage.gameObject.SetActive(true);
            rewardAmount.gameObject.SetActive(true);
            rewardAmount.text = "x" + baseReward.GetAmountStringForUI();
        }
    }

    public class LeaderboardPlayerScoreInfoUITheme
    {
        public Color themeColor;
        public Material textMaterial;

        public Sprite mainBGSprite;
        public Sprite mainScoreBGSprite;
    }
}