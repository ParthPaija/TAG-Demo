using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class MainSceneLoader : SerializedManager<MainSceneLoader>
    {
        #region PUBLIC_VARIABLES

        public List<ManagerInstanceLoader> managers;

        #endregion

        #region PRIVATE_VARIABLES

        public static SceneTransitionData SceneTransitionData => mySceneTransistionData;
        private static SceneTransitionData mySceneTransistionData;

        #endregion

        #region PROPERTIES
        public float LoadingProgress { get; private set; }

        #endregion

        #region UNITY_CALLBACKS

        public override void Awake()
        {
            base.Awake();
            StartCoroutine(LoadManager());
        }

        #endregion

        #region PUBLIC_METHODS

        public static void SetSceneTransitionData(SceneTransitionData sceneTransitionData)
        {
            mySceneTransistionData = sceneTransitionData;
        }

        #endregion

        #region PRIVATE_METHODS

        private void OnMainSceneLoadingDone()
        {
            OnLoadingDone();
            //MainSceneUIManager.Instance.GetView<MainView>().Show();
            MainSceneUIManager.Instance.GetView<BottombarView>().InitView();
            MainSceneUIManager.Instance.GetView<BottombarView>().Show();
            AdManager.Instance.ShowBannerAd();
            AutoOpenPopupHandler.Instance.OnCheckForAutoOpenPopUps();
            SoundHandler.Instance.PlayMetaMusic();
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region COROUTINES

        IEnumerator LoadManager()
        {
            LoadingProgress = 0f;
            float startTime = Time.realtimeSinceStartup;

            for (int i = 0; i < managers.Count; i++)
            {
                float managerStartTime = Time.realtimeSinceStartup;
                Debug.Log($"Starting to load manager: {managers[i].name}");
                
                managers[i].gameObject.SetActive(true);
                while (!managers[i].loaded)
                {
                    yield return null;
                }
                
                float managerLoadTime = Time.realtimeSinceStartup - managerStartTime;
                Debug.Log($"Manager {managers[i].name} loaded in {managerLoadTime:F2} seconds");
                
                LoadingProgress = ((float)(i + 1)) / managers.Count;
            }
            
            yield return null;

            LoadingProgress = 1f;
            float totalLoadTime = Time.realtimeSinceStartup - startTime;
            Debug.Log($"Total loading time for all managers: {totalLoadTime:F2} seconds");

            yield return new WaitForSeconds(0.1f);
            OnMainSceneLoadingDone();
        }

        #endregion

        #region UI_CALLBACKS

        #endregion
    }

    public class SceneTransitionData
    {
        public GameLevelTriggerType gameLevelTriggerType;
        public SceneType loadingFromScene;
        public Level loadingFromLevel;

        public SceneTransitionData() { }
        public SceneTransitionData(SceneType sceneType)
        {
            loadingFromScene = sceneType;
        }
        public SceneTransitionData(SceneType sceneType, Level level)
        {
            loadingFromScene = sceneType;
            loadingFromLevel = level;
        }
    }

    public enum GameLevelTriggerType
    {
        NONE,
    }

    public enum GameLevelResultType
    {
        NONE,
        LEVEL_WIN,
        LEVEL_LOSE,
        LEVEL_ESCAPE
    }

    public enum SceneType
    {
        LOADING,
        MainScene,
    }
}