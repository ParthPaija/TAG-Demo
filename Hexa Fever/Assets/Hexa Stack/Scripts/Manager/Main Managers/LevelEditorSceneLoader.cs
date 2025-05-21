using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class LevelEditorSceneLoader : SerializedManager<MainSceneLoader>
    {
        #region PUBLIC_VARIABLES

        public List<ManagerInstanceLoader> managers;

        #endregion

        #region PRIVATE_VARIABLES

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

        #endregion

        #region PRIVATE_METHODS

        private void OnMainSceneLoadingDone()
        {
            OnLoadingDone();
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region COROUTINES

        IEnumerator LoadManager()
        {
            LoadingProgress = 0f;
            yield return null;

            for (int i = 0; i < managers.Count; i++)
            {
                managers[i].gameObject.SetActive(true);
                while (!managers[i].loaded)
                {
                    yield return 0;
                }

                LoadingProgress = ((float)(i + 1)) / managers.Count;
                yield return new WaitForSeconds(0.5f);
            }

            yield return null;

            LoadingProgress = 1f;
            yield return new WaitForSeconds(0.5f);

            OnMainSceneLoadingDone();
        }

        #endregion

        #region UI_CALLBACKS

        #endregion
    }
}
