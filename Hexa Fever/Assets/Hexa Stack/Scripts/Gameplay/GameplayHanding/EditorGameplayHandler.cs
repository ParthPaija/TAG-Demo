#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
#endif

namespace Tag.HexaStack
{
#if UNITY_EDITOR
    public class EditorGameplayHandler : MainGameplayHandler
    {
        #region PUBLIC_VARS
        public int playAttemptsCount = 0; // Track how many times the level has been played
        public int maxPlayAttempts = 5; // Maximum number of attempts allowed
        public bool autoReplay = true; // Whether to automatically replay until max attempts
        #endregion

        #region PRIVATE_VARS
        private bool isReplaying = false;
        private const string PLAY_ATTEMPTS_KEY = "HexaSort_PlayAttemptsCount";
        private const string MAX_ATTEMPTS_KEY = "HexaSort_MaxPlayAttempts";
        private const string AUTO_REPLAY_KEY = "HexaSort_AutoReplay";
        #endregion

        #region UNITY_CALLBACKS
        private void Awake()
        {
            // Load saved values
            LoadSettings();
        }
        #endregion

        #region PUBLIC_FUNCTIONS

        public override void OnLevelStart(LevelDataSO levelDataSO)
        {
            currentLevel = levelDataSO;
            currentLevel.OnLevelStart();
            //ResetUndoBoosterData();
            GameRuleManager.Instance.OnLevelComplate();

            reviveCountCoin = 0;

            LevelProgressData levelProgressData = DataManager.LevelProgressData;

            levelProgressData.ReSetData();

            levelRunTime = levelProgressData.currentRunningTime;
            DataManager.Instance.SaveLevelProgressData(levelProgressData);

            SetGoal();

            reviveCountCoin = levelProgressData.currentReviveCountCoin;
            reviveCountAd = levelProgressData.currentReviveCountAd;
            
            // Increment play attempts when starting
            playAttemptsCount++;
            SavePlayAttempts();
            Debug.Log($"Level attempt #{playAttemptsCount} of {maxPlayAttempts}");
        }

        public override void OnLevelWin()
        {
            GameRuleManager.Instance.OnLevelComplate();
            Debug.Log($"Level completed! Attempt #{playAttemptsCount}");
            
            // Check if we should restart the level or exit
            if (playAttemptsCount < maxPlayAttempts)
            {
                Debug.Log("Restarting level after win...");
                //RestartCurrentLevel();
                SceneManager.LoadScene("LevelEditor");
            }
            else
            {
                Debug.Log($"Reached max attempts ({maxPlayAttempts}). Exiting play mode.");
                ResetPlayAttempts();
                
                #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
                #endif
            }
        }

        public override void OnOutOfSpace()
        {
            MainSceneUIManager.Instance.GetView<OutOfSpaceView>().Show();
        }

        public override void OnLevelFail()
        {
            Debug.Log($"Level failed! Attempt #{playAttemptsCount}");
            
            // Check if we should restart the level or exit
            if (playAttemptsCount < maxPlayAttempts)
            {
                Debug.Log("Restarting level after fail...");
                //RestartCurrentLevel();
                SceneManager.LoadScene("LevelEditor");
            }
            else
            {
                Debug.Log($"Reached max attempts ({maxPlayAttempts}). Exiting play mode.");
                ResetPlayAttempts();
                
                #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
                #endif
            }
        }

        public override void SaveAllDataOfLevel()
        {
            // Save our settings too
            SaveSettings();
        }

        public override void OnUndoBoosterUse()
        {
        }

        public override void CancelUndoBooster()
        {
        }

        public override void Init(GameplayManager gameplayManager)
        {
            base.Init(gameplayManager);
            LoadSettings();
        }

        public override void OnLevelFailRetry()
        {
            base.OnLevelFailRetry();
        }

        public void SetMaxPlayAttempts(int attempts)
        {
            maxPlayAttempts = Mathf.Max(1, attempts);
            PlayerPrefs.SetInt(MAX_ATTEMPTS_KEY, maxPlayAttempts);
            PlayerPrefs.Save();
            Debug.Log($"Max play attempts set to {maxPlayAttempts}");
        }

        public void ResetPlayAttempts()
        {
            playAttemptsCount = 0;
            SavePlayAttempts();
            Debug.Log("Play attempts reset to 0");
        }

        public void ToggleAutoReplay()
        {
            autoReplay = !autoReplay;
            PlayerPrefs.SetInt(AUTO_REPLAY_KEY, autoReplay ? 1 : 0);
            PlayerPrefs.Save();
            Debug.Log($"Auto replay {(autoReplay ? "enabled" : "disabled")}");
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetGoal()
        {
            GameplayGoalHandler.Instance.OnLevelStart(currentLevel);
            LevelManager.Instance.StartLevel(currentLevel);

            //GoalStripUIManager.Instance.ShowGoalStripView(currentLevel.LevelType, () =>
            //{
            //});
        }
        
        private void SavePlayAttempts()
        {
            PlayerPrefs.SetInt(PLAY_ATTEMPTS_KEY, playAttemptsCount);
            PlayerPrefs.Save();
        }
        
        private void LoadSettings()
        {
            // Load previous count if exists
            if (PlayerPrefs.HasKey(PLAY_ATTEMPTS_KEY))
            {
                playAttemptsCount = PlayerPrefs.GetInt(PLAY_ATTEMPTS_KEY, 0);
            }
            
            // Load max attempts
            if (PlayerPrefs.HasKey(MAX_ATTEMPTS_KEY))
            {
                maxPlayAttempts = PlayerPrefs.GetInt(MAX_ATTEMPTS_KEY, 5);
            }
            
            // Load auto replay setting
            if (PlayerPrefs.HasKey(AUTO_REPLAY_KEY))
            {
                autoReplay = PlayerPrefs.GetInt(AUTO_REPLAY_KEY, 1) == 1;
            }
        }
        
        private void SaveSettings()
        {
            SavePlayAttempts();
            PlayerPrefs.SetInt(MAX_ATTEMPTS_KEY, maxPlayAttempts);
            PlayerPrefs.SetInt(AUTO_REPLAY_KEY, autoReplay ? 1 : 0);
            PlayerPrefs.Save();
        }
        
        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        #endregion
    }
#endif
}
