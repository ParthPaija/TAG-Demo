using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tag.HexaStack.Editor;
using Tag.MetaGame.TaskSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class MainView : BaseView
    {
        #region PUBLIC_VARS

        public VIPLeaderboardUIButton LeaderboardUIButton { get => leaderboardUIButton; }
        public DailyAdPassViewButton DailyAdPassViewButton { get => dailyAdPassViewButton; }
        public PlayerProfileButton PlayerProfileButton { get => playerProfileButton; }

        #endregion

        #region PRIVATE_VARS

        [Header("Debug Build")]
        [SerializeField] private InputField levelNo;
        [SerializeField] private Dropdown levelTestngTyeDD;

        [Space(10)]
        [SerializeField] private Text metaButtonText;
        [SerializeField] private RectFillBar fillBar;
        [SerializeField] private Button newAreaButton;
        [SerializeField] private Dictionary<LevelType, LevelButtonView> levelButton = new Dictionary<LevelType, LevelButtonView>();
        [SerializeField] private VIPLeageLevelButtonView leageLevelButtonView;
        [SerializeField] private VIPLeaderboardUIButton leaderboardUIButton;
        [SerializeField] private DailyAdPassViewButton dailyAdPassViewButton;
        [SerializeField] private PlayerProfileButton playerProfileButton;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void Show(Action action = null, bool isForceShow = false)
        {
            base.Show();
            SetUI();
            MainSceneUIManager.Instance.OnMainView();

            levelTestngTyeDD.ClearOptions();
            levelTestngTyeDD.AddOptions(Enum.GetNames(typeof(LevelTestingType)).ToList());
            levelTestngTyeDD.SetValueWithoutNotify((int)ResourceManager.Instance.CurrentTestingType);

            levelTestngTyeDD.gameObject.SetActive(!Constant.IsProductionBuid);
            levelNo.gameObject.SetActive(!Constant.IsProductionBuid);
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        public void OnLeveltestingTypeChanage()
        {
            ResourceManager.Instance.ChnageLevelTestingType((LevelTestingType)levelTestngTyeDD.value);
        }

        public void SetUI()
        {
            foreach (var item in levelButton)
            {
                item.Value.gameObject.SetActive(false);
            }
            levelNo.text = DataManager.PlayerData.playerGameplayLevel.ToString();

            if (VIPLeagueManager.Instance.IsVIPLeageUnlocked())
            {
                StartCoroutine(WaitForInit(() =>
                {
                    LevelDataSO levelDataSO = VIPLeagueManager.Instance.GetLevelData();
                    leageLevelButtonView.SetView(levelDataSO, OnClick_LeageLevel);
                }));

            }
            else
            {
                LevelDataSO levelDataSO = ResourceManager.Instance.GetLevelData(DataManager.PlayerData.playerGameplayLevel);
                if (levelButton.ContainsKey(levelDataSO.LevelType))
                {
                    levelButton[levelDataSO.LevelType].SetView(OnClick_Level);
                }
                else
                {
                    levelButton[LevelType.Normal].SetView(OnClick_Level);
                }
            }

            int completedTaskCount = TaskManager.Instance.CompleteTaskCount;
            int totalTaskCount = TaskManager.Instance.TotalTaskCount;
            fillBar.Fill((float)completedTaskCount / totalTaskCount);
            metaButtonText.text = completedTaskCount + "/" + totalTaskCount;
            newAreaButton.gameObject.SetActive(TaskManager.Instance.CanOpenNewAreaView());
        }

        #endregion

        #region CO-ROUTINES

        public Coroutine WaitAndCallAction(Action action, DateTime endTime)
        {
            return StartCoroutine(WaitForActionCO(action, endTime));
        }

        private IEnumerator WaitForActionCO(Action action, DateTime endTime)
        {
            WaitForSeconds wait = new WaitForSeconds(1f);
            while (DateTime.Now < endTime)
            {
                yield return wait;
            }
            action?.Invoke();
        }

        IEnumerator WaitForInit(Action actionToCall)
        {
            while (PlayerPersistantData.GetVIPLeaderboardPlayerData() == null)
            {
                yield return null;
            }
            actionToCall?.Invoke();
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void OnClick_LeageLevel(LevelDataSO levelDataSO)
        {
            if (!VIPLeaderboardManager.Instance.IsCurrentLeaderboardEventActive() && VIPLeaderboardManager.Instance.IsEventResultRedayForShown())
            {
                MainSceneUIManager.Instance.GetView<VIPLeaderboardView>().Show();
                return;
            }

            var currency = DataManager.Instance.GetCurrency(CurrencyConstant.ENERGY);
            if (currency.Value < 1)
            {
                MainSceneUIManager.Instance.GetView<NotEnoughLifeView>().ShowView(() =>
                {
                    GameplayManager.Instance.OnVIPLeageLevelStart(levelDataSO);
                }, null);
                return;
            }
            if (!string.IsNullOrEmpty(levelNo.text) && !string.IsNullOrWhiteSpace(levelNo.text))
            {
                var playerData = DataManager.PlayerData;
                playerData.playerGameplayLevel = int.Parse(levelNo.text);
                DataManager.Instance.SaveData(playerData);
            }
            GameplayManager.Instance.OnVIPLeageLevelStart(levelDataSO);
        }

        public void OnClick_Level()
        {
            var currency = DataManager.Instance.GetCurrency(CurrencyConstant.ENERGY);
            if (currency.Value < 1)
            {
                MainSceneUIManager.Instance.GetView<NotEnoughLifeView>().ShowView(() =>
                {
                    if (!string.IsNullOrEmpty(levelNo.text) && !string.IsNullOrWhiteSpace(levelNo.text))
                    {
                        var playerData = DataManager.PlayerData;
                        playerData.playerGameplayLevel = int.Parse(levelNo.text);
                        DataManager.Instance.SaveData(playerData);
                    }

                    GameplayManager.Instance.OnMainGameLevelStart();
                }, null);
                return;
            }
            if (!string.IsNullOrEmpty(levelNo.text) && !string.IsNullOrWhiteSpace(levelNo.text))
            {
                var playerData = DataManager.PlayerData;
                playerData.playerGameplayLevel = int.Parse(levelNo.text);
                DataManager.Instance.SaveData(playerData);
            }
            GameplayManager.Instance.OnMainGameLevelStart();
        }

        public void OnSetting()
        {
            MainSceneUIManager.Instance.GetView<SettingView>().Show();
        }

        public void OnMetaClick()
        {
            MainSceneUIManager.Instance.GetView<TodoTaskPopup>().Show();
            Hide();
            MainSceneUIManager.Instance.GetView<BottombarView>().Hide();
        }

        public void OnNewAreaClick()
        {
            if (TaskManager.Instance.IsAllAreaCompleted())
            {
                MainSceneUIManager.Instance.GetView<NewAreaComingSoonPopup>().Show();
            }
            else
            {
                MainSceneUIManager.Instance.GetView<AreaUnlockView>().Show();
            }
        }

        #endregion
    }
}
