using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class LevelManager : SerializedManager<LevelManager>
    {
        #region PUBLIC_VARS
        public Level LoadedLevel { get => loadedLevel; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Transform levelHolder;
        [SerializeField] private LevelDataSO currentLevel;
        private Level loadedLevel;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void StartLevel(LevelDataSO levelData)
        {
            //if (LevelEditorManager.Instance != null)
            //{
            //    StartLevel_Editor(levelData);
            //    return;
            //}
            currentLevel = levelData;
            LoadLevel();
            
        }

        public void UnloadLevel()
        {
            if (loadedLevel != null)
            {
                ItemStackSpawnerManager.Instance.ClearStack();
                Destroy(loadedLevel.gameObject);
            }
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void LoadLevel()
        {
            UnloadLevel();
            loadedLevel = Instantiate(currentLevel.LevelPrefab, levelHolder);
            loadedLevel.Init();
            ItemStackSpawnerManager.Instance.OnGameStart();
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion

        public void StartLevel_Editor(LevelDataSO levelData)
        {
            currentLevel = levelData;
            FindObjectOfType<Level>().transform.SetParent(levelHolder);
            FindObjectOfType<Level>().transform.localPosition = Vector3.zero;
            FindObjectOfType<Level>().Init();
            loadedLevel = FindObjectOfType<Level>();
            ItemStackSpawnerManager.Instance.OnGameStart();
            //LoadLevel();
        }
    }
}
