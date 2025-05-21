using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tag.HexaStack
{
    public class GameplayManager : SerializedManager<GameplayManager>
    {
        #region PUBLIC_VARS
        public LevelDataSO CurrentLevel { get => currentHandler.CurrentLevel; }
        public int LevelNo { get => currentHandler.LevelNo; }
        public int ReviveCountCoin { get => currentHandler.ReviveCountCoin; set => currentHandler.ReviveCountCoin = value; }
        public int ReviveCountAd { get => currentHandler.ReviveCountAd; set => currentHandler.ReviveCountAd = value; }
        public int AdTileWatchCount { get => currentHandler.AdTileWatchCountAd; set => currentHandler.AdTileWatchCountAd = value; }
        public UndoBoosterProgressData UndoBoosterData { get => currentHandler.UndoBoosterData; set => currentHandler.UndoBoosterData = value; }
        public BaseGameplayHandler CurrentHandler { get => currentHandler; }

        public SpriteRenderer bg;

        public static TestPrintData testPrintData = new TestPrintData();

        // Events for the AutoplaySimulator
        public event Action<LevelDataSO> OnLevelStarted;
        public event Action OnLevelCompleted;
        public event Action OnLevelFailed;
        #endregion

        #region PRIVATE_VARS

        [SerializeField] Dictionary<GameplayHandlerType, BaseGameplayHandler> gameplayHandlerMapping = new Dictionary<GameplayHandlerType, BaseGameplayHandler>();
        [SerializeField] private BaseGameplayHandler currentHandler;
        private List<Action<BaseCell, int>> onStackRemove = new List<Action<BaseCell, int>>();
        private List<Action<BaseCell, int>> onStackOrItemRemove = new List<Action<BaseCell, int>>();
        private List<Action<int, int>> onItemRemove = new List<Action<int, int>>();
        private List<Action<bool, BaseCell>> onStackItemRemoveOrAdd = new List<Action<bool, BaseCell>>();

        private bool asd;

        private int IsTutorialFinish
        {
            get =>

                PlayerPrefs.GetInt("IsTutorialFinish", 0);

            set =>

                PlayerPrefs.SetInt("IsTutorialFinish", value);

        }

        public bool Asd { get => asd; set => asd = value; }

        #endregion

        #region UNITY_CALLBACKS
        private void Start()
        {
            StartCoroutine(WaitForGamePlay());
        }
        public IEnumerator WaitForGamePlay()
        {
            while (AutoOpenPopupHandler.Instance == null)
                yield return null;
            if (IsTutorialFinish == 0)
            {
                OnMainGameLevelStart();
                IsTutorialFinish = 1;
            }
        }
        #endregion

        #region PUBLIC_FUNCTIONS

        public void OnMainGameLevelStart_Editor()
        {
#if UNITY_EDITOR
            currentHandler = gameplayHandlerMapping[GameplayHandlerType.Editor];
            LevelDataSO levelData = AssetDatabase.LoadAssetAtPath<LevelDataSO>("Assets/Hexa Stack/Asset/" + LevelEditorManager.levelTestingType + "/Level " + LevelEditorManager.lastLevelEditNo + "/LevelData " + LevelEditorManager.lastLevelEditNo + ".asset");
            currentHandler.OnLevelStart(levelData);
            // Invoke the OnLevelStarted event
            OnLevelStarted?.Invoke(levelData);
#endif
        }
        [Button]
        public void OnMainGameLevelStart()
        {
            currentHandler = gameplayHandlerMapping[GameplayHandlerType.Main];
            testPrintData = new TestPrintData();
            GlobalUIManager.Instance.GetView<InGameLoadingView>().ShowView(1f, () =>
            {
                LevelDataSO levelData = ResourceManager.Instance.GetLevelData(DataManager.PlayerData.playerGameplayLevel);
                StreakBonusManager.Instance.OnLevelStart(DataManager.LevelProgressData.levelNo == levelData.Level);
                currentHandler.OnLevelStart(levelData);
                testPrintData.OnStartLevel();
                OnLevelStarted?.Invoke(levelData);
            });
        }

        public void OnCurrentLevelRestartOrNext()
        {
            GlobalUIManager.Instance.GetView<InGameLoadingView>().ShowView(1f, () =>
            {
                StreakBonusManager.Instance.OnLevelStart(false);
                currentHandler.OnNextOrCurrentLevelRestart();
                OnLevelStarted?.Invoke(currentHandler.CurrentLevel);
            });
        }

        public void OnVIPLeageLevelStart(LevelDataSO levelDataSO)
        {
            currentHandler = gameplayHandlerMapping[GameplayHandlerType.VIPLeague];
            GlobalUIManager.Instance.GetView<InGameLoadingView>().ShowView(1f, () =>
            {
                StreakBonusManager.Instance.OnLevelStart(DataManager.LevelProgressData.levelNo == levelDataSO.Level);
                currentHandler.OnLevelStart(levelDataSO);
                OnLevelStarted?.Invoke(levelDataSO);
            });
        }

        [Button]
        public void OnLevelWin()
        {
            StreakBonusManager.Instance.OnWinLevel();
            currentHandler.OnLevelWin();
            testPrintData.OnLevelComplate(LevelComplateState.Win);
            OnLevelCompleted?.Invoke();
        }

        [Button]
        public void OnOutOfSpace()
        {
            currentHandler.OnOutOfSpace();
        }

        public void OnLevelFail()
        {
            currentHandler.OnLevelFail();
            testPrintData.OnLevelComplate(LevelComplateState.Fail);
            OnLevelFailed?.Invoke();
            StreakBonusManager.Instance.OnLossLevel();
        }

        public void OnLevelFailRetry()
        {
            currentHandler.OnLevelFailRetry();
        }

        public void OnExit()
        {
            currentHandler.OnExit();
            testPrintData.OnLevelComplate(LevelComplateState.Exit);
        }

        public void OnRetry()
        {
            currentHandler.OnRetry();
        }

        public void SaveAllDataOfLevel()
        {
            currentHandler.SaveAllDataOfLevel();
        }

        public void SaveDataForUndoBooster()
        {
            currentHandler.SaveDataForUndoBooster();
        }

        public void OnUndoBoosterUse()
        {
            currentHandler.OnUndoBoosterUse();
        }

        public void CancelUndoBooster()
        {
            currentHandler.CancelUndoBooster();
        }

        public void IncreaseLevelRunTime()
        {
            currentHandler.IncreaseLevelRunTime();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        public void InvokeOnStackRemove(BaseCell baseCell, int itemId)
        {
            foreach (var ev in onStackRemove)
            {
                ev?.Invoke(baseCell, itemId);
            }
        }

        public void AddListenerOnStackRemove(Action<BaseCell, int> action)
        {
            if (!onStackRemove.Contains(action))
                onStackRemove.Add(action);
        }

        public void RemoveListenerOnStackRemove(Action<BaseCell, int> action)
        {
            if (onStackRemove.Contains(action))
                onStackRemove.Remove(action);
        }

        public void InvokeOnStackOrItemRemove(BaseCell baseCell, int itemId)
        {
            foreach (var ev in onStackOrItemRemove)
            {
                ev?.Invoke(baseCell, itemId);
            }
        }

        public void AddListenerOnStackOrItemRemove(Action<BaseCell, int> action)
        {
            if (!onStackOrItemRemove.Contains(action))
                onStackOrItemRemove.Add(action);
        }

        public void RemoveListenerOnStackOrItemRemove(Action<BaseCell, int> action)
        {
            if (onStackOrItemRemove.Contains(action))
                onStackOrItemRemove.Remove(action);
        }

        public void InvokeOnItemRemove(int itemId, int itemCount)
        {
            foreach (var ev in onItemRemove)
            {
                ev?.Invoke(itemId, itemCount);
            }
        }

        public void AddListenerOnItemRemove(Action<int, int> action)
        {
            if (!onItemRemove.Contains(action))
                onItemRemove.Add(action);
        }

        public void RemoveListenerOnItemRemove(Action<int, int> action)
        {
            if (onItemRemove.Contains(action))
                onItemRemove.Remove(action);
        }

        public void InvokeOnStackItemRemoveOrAdd(BaseCell baseCell, bool isStart)
        {
            foreach (var ev in onStackItemRemoveOrAdd)
            {
                ev?.Invoke(isStart, baseCell);
            }
        }

        public void AddListenerOnStackItemRemoveOrAdd(Action<bool, BaseCell> action)
        {
            if (!onStackItemRemoveOrAdd.Contains(action))
                onStackItemRemoveOrAdd.Add(action);
        }

        public void RemoveListenerStackItemRemoveOrAdd(Action<bool, BaseCell> action)
        {
            if (onStackItemRemoveOrAdd.Contains(action))
                onStackItemRemoveOrAdd.Remove(action);
        }

        #endregion

        #region UI_CALLBACKS       

        #endregion

        #region PUBLIC_FUNCTIONS

        #endregion
    }

    public class TestPrintData
    {
        public int levelNo;
        public int target;
        public string timeToComplateLevel;
        public int noOfMoves;
        public int boosterUseHammer;
        public int boosterUseStackSwap;
        public int boosterUseSpawn;
        public int tileAdWatch;
        public int outOfSpaceCount;
        public LevelComplateState levelComplateState;

        public DateTime startTime;
        public DateTime endTime;

        public void OnStartLevel()
        {
            LevelDataSO levelData = GameplayManager.Instance.CurrentLevel;
            levelNo = levelData.Level;
            target = levelData.LevelGoals[0].GoalCount;

            startTime = TimeManager.Now;
        }

        public void AddMove()
        {
            noOfMoves++;
        }

        public void OnBoosterUse(int id)
        {
            switch (id)
            {
                case 3:
                    boosterUseHammer++;
                    break;
                case 4:
                    boosterUseSpawn++;
                    break;
                case 5:
                    boosterUseStackSwap++;
                    break;
                default:
                    break;
            }
        }

        public void OnTileAdWatch()
        {
            tileAdWatch++;
        }

        public void AddOutOfSpaceCoount()
        {
            outOfSpaceCount++;
        }

        public void OnLevelComplate(LevelComplateState levelComplateState)
        {
            endTime = TimeManager.Now;
            this.levelComplateState = levelComplateState;

            double time = endTime.Subtract(startTime).TotalSeconds;

            string printString = $"{levelNo}" +
                $" | {target}" +
                $" | {target - GameplayManager.Instance.CurrentLevel.LevelGoals[0].CurrentCount}" +
                $" | {time}" +
                $" | {noOfMoves}" +
                $" | {tileAdWatch}" +
                $" | {outOfSpaceCount}" +
                $" | {this.levelComplateState}" +
                $" | {boosterUseHammer}" +
                $" | {boosterUseStackSwap}" +
                $" | {boosterUseSpawn}" +
                $" | {0}";

            TestDataManager.Print(printString);
        }
    }

    public enum LevelComplateState
    {
        Win,
        Fail,
        Exit
    }
}
