using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class ManagerLoader : MonoBehaviour
    {
        #region PUBLIC_VARS
        public float LoadingProgress { get; private set; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private List<ManagerInstanceLoader> managers = new List<ManagerInstanceLoader>();

        #endregion

        #region UNITY_CALLBACKS

        public void Awake()
        {
            StartCoroutine(LoadManager());
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        IEnumerator LoadManager()
        {
            float startTime = Time.realtimeSinceStartup;
            Debug.Log("Starting to load all managers in ManagerLoader");

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
            }

            float totalLoadTime = Time.realtimeSinceStartup - startTime;
            Debug.Log($"ManagerLoader: Total loading time for all managers: {totalLoadTime:F2} seconds");
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
