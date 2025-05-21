using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Tag.AssetManagement;
using Tag.CoreGame;
using Tag.HexaStack;
using Tag.MetaGame.TaskSystem;
using UnityEngine;

namespace Tag.MetaGame
{
    public class AreaManager : Manager<AreaManager>
    {
        #region PUBLIC_VARS
        public string CurrentAreaId { get { return _currentArea.id; } }
        public string defaultData;
        #endregion

        #region PRIVATE_VARS
        [SerializeField] private Transform _currentAreaParentTransform;
        [SerializeField] private Transform _visitedAreaParentTransform;
        [SerializeField] private Area _currentArea;
        [SerializeField] private Area _currentVisitArea;
        [SerializeField] private LoadOperationHandle<GameObject> _currentAreaLoadOperationHandle;
        [SerializeField] private LoadOperationHandle<GameObject> _visitAreaLoadOperationHandle;
        [SerializeField] private ParticleSystem _newAreaUnlockParticles;


        private string ALL_AREA_PROGRESS_DATA_KEY = "ALL_AREA_PROGRESS_DATA_KEY";
        private AllAreaPlayerData _areaPlayerData;

        private bool _isValidAddress;
        private Action<int> _onNewAreaUnlock;
        private AreasView AreasView { get { return MainSceneUIManager.Instance.GetView<AreasView>(); } }
        private AssetUpdateAvailablePopup AssetUpdateAvailablePopup { get { return GlobalUIManager.Instance.GetView<AssetUpdateAvailablePopup>(); } }
        private TaskManager TaskManager { get { return TaskManager.Instance; } }
        private InGameLoadingView InGameLoadingView { get { return GlobalUIManager.Instance.GetView<InGameLoadingView>(); } }
        private ILoader Loader { get { return AssetManagerAddressable.Instance.loaderAddressable; } }
        private AreaAssetStateHandler AreaAssetStateHandler { get { return AreaAssetStateHandler.Instance; } }

        public bool isSyncWithCloud;
        public bool Sync { get { return isSyncWithCloud; } }
        #endregion

        #region UNITY_CALLBACKS
        public override void Awake()
        {
            base.Awake();
            InitData(OnLoadingDone);
        }
        #endregion

        #region PUBLIC_FUNCTIONS

        public void InitData(Action onInit)
        {
            LoadData();
            string areaId = _areaPlayerData._currentAreaIdPrefs;

            if (AreaAssetStateHandler.GetAssetState(areaId) == AssetState.DOWNLOADED)
            {
                LoadArea(areaId, onInit);
            }
            else if (AreaAssetStateHandler.GetAssetState(areaId) == AssetState.NOT_DOWNLOAD)
            {
                DownloadArea(areaId, delegate { LoadArea(areaId, onInit); });
            }
            else
            {
                throw new Exception($"Not Valid AreaId {areaId}");
            }
        }


        public void VisitArea(string areaId, Action exitAction)
        {
            if (_visitAreaLoadOperationHandle != null)
            {
                return;
            }

            _visitAreaLoadOperationHandle = Loader.LoadAsset<GameObject>(AssetAddress.GenerateAreaAddress(areaId));
            StartCoroutine(LoadProgress(_visitAreaLoadOperationHandle, OnLoadArea));
            void OnLoadArea()
            {
                AreasView.HideView();
                InGameLoadingView.Hide();
                if (_visitAreaLoadOperationHandle.data.TryGetComponent(out Area area))
                {
                    _currentArea.gameObject.SetActive(false);
                    _currentVisitArea = Instantiate(area, _visitedAreaParentTransform);
                    _currentVisitArea.Init(_areaPlayerData._allAreaProgressDict[area.id].data);
                    _currentVisitArea.PlayRevealSequence(exitAction);
                    //  TransitionView.RegisterAfterHideFXAction(() => _currentVisitArea.PlayRevealSequence(exitAction));
                }
                else
                {
                    throw new Exception("area id is not valid!! : ");
                }
            }
        }

        public void OnAreaComplete(Action exitAction)
        {
            SaveData();
            _currentArea.AreaCompleteRevealSequence(exitAction);
        }

        public void StartTask(string taskId)
        {
            _currentArea.StartTask(taskId);
        }

        public void UnloadVisitedArea()
        {
            if (_currentVisitArea != null)
            {
                _currentArea.gameObject.SetActive(true);
                Destroy(_currentVisitArea.gameObject);
                Loader.Unload(_visitAreaLoadOperationHandle);
                _visitAreaLoadOperationHandle = null;
                _currentVisitArea = null;
            }
        }

        public void SetTaskData(string taskId, string taskData)
        {
            if (_currentVisitArea != null)
            {
                _currentVisitArea.SetDecoreItemData(taskId, taskData);
            }
            else
            {
                _currentArea.SetDecoreItemData(taskId, taskData);
            }
        }

        public void SetAreaData(string areaId, string areaData)
        {
            _areaPlayerData._allAreaProgressDict[areaId].data = areaData;
        }

        public void SaveData()
        {
            PlayerPrefs.SetString(ALL_AREA_PROGRESS_DATA_KEY, JsonConvert.SerializeObject(_areaPlayerData));
        }

        public void UnlockNewArea(LoadOperationHandle<GameObject> loadOperationHandle)
        {
            if (loadOperationHandle.data.TryGetComponent(out Area area))
            {
                AnalyticsManager.Instance.LogGAEvent("MetaAreaStart : " + _areaPlayerData._currentAreaIdPrefs + " : " + DataManager.PlayerData.playerGameplayLevel);
                TaskManager.UnlockNewArea(area.taskAreaData);
                _areaPlayerData._currentAreaIdPrefs = area.id;
                _areaPlayerData._allAreaProgressDict.Add(area.id, new AreaPlayerData(area.GetData()));
                SaveData();
                Destroy(_currentArea.gameObject);
                Loader.Unload(_currentAreaLoadOperationHandle);
                _currentAreaLoadOperationHandle = loadOperationHandle;
                _currentArea = Instantiate(area, _currentAreaParentTransform);
                _newAreaUnlockParticles.Play();
                _currentArea.Init(_areaPlayerData._allAreaProgressDict[_currentArea.id].data);
                _currentArea.LoadArea();
                AreasView.UpdateView();
                _onNewAreaUnlock?.Invoke(AreaUtility.AreaIdToAreaNo(area.id));
            }
        }

        public void RegisterIconActions(Action showIconAction, Action hideIconAction)
        {
            if (_currentVisitArea != null)
            {
                _currentVisitArea.RegisterIconAction(showIconAction, hideIconAction);
            }
            else
            {
                _currentArea.RegisterIconAction(showIconAction, hideIconAction);
            }
        }

        public void RaiseShowIconsAction()
        {
            if (_currentVisitArea != null)
            {
                _currentVisitArea.RaiseOnDecoreIconsShow();
            }
            else
            {
                _currentArea.RaiseOnDecoreIconsShow();
            }
        }

        public void RaiseHideIconsAction()
        {
            if (_currentVisitArea != null)
            {
                _currentVisitArea.RaiseOnDecoreIconsHide();
            }
            else
            {
                _currentArea.RaiseOnDecoreIconsHide();
            }
        }

        public bool IsAreaLocked(string areaId)
        {
            return !_areaPlayerData._allAreaProgressDict.ContainsKey(areaId);
        }

        public AreaState GetAreaState(string areaId)
        {
            return _areaPlayerData._allAreaProgressDict[areaId].areaState;
        }

        public string GetLastCompleteAreaId()
        {
            string areaId = "A1";
            foreach (KeyValuePair<string, AreaPlayerData> keyValuePair in _areaPlayerData._allAreaProgressDict)
            {
                if (keyValuePair.Value.areaState == AreaState.Complete)
                {
                    areaId = keyValuePair.Key;
                }
            }
            return areaId;
        }

        public void CompleteCurrentArea()
        {
            AnalyticsManager.Instance.LogGAEvent("MetaAreaComplete : " + _areaPlayerData._currentAreaIdPrefs + " : " + DataManager.PlayerData.playerGameplayLevel);
            UpdateAreaState(_areaPlayerData._currentAreaIdPrefs, AreaState.Complete);
        }
        public List<string> GetCurrentAreaCompleteTask()
        {
            return _currentArea.GetCompleteTaskIds();
        }
        public void RegisterOnNewAreaUnlockAction(Action<int> onAreaComplete)
        {
            _onNewAreaUnlock = onAreaComplete;
        }
        public void DeregisterOnNewAreaUnlockAction(Action<int> action)
        {
            _onNewAreaUnlock -= action;
        }
        #endregion

        #region PRIVATE_FUNCTIONS

        private void LoadArea(string areaId, Action onInit)
        {
            _currentAreaLoadOperationHandle = Loader.LoadAsset<GameObject>(AssetAddress.GenerateAreaAddress(areaId));
            StartCoroutine(LoadProgress(_currentAreaLoadOperationHandle, OnLoadArea));
            void OnLoadArea()
            {
                if (_currentAreaLoadOperationHandle.data.TryGetComponent(out Area area))
                {
                    _currentArea = Instantiate(area, _currentAreaParentTransform);
                    onInit?.Invoke();
                    //LoadData();
                    _currentArea.Init(_areaPlayerData._allAreaProgressDict[_currentArea.id].data);
                    TaskManager.InitData(_currentArea.taskAreaData);
                    _currentArea.LoadArea();
                }
            }
        }
        private void DownloadArea(string areaId, Action onDownload)
        {
            AssetUpdateAvailablePopup.ShowPopup(areaId, onDownload);
        }
        private void UpdateAreaState(string areaId, AreaState areaState)
        {
            _areaPlayerData._allAreaProgressDict[areaId].areaState = areaState;
            SaveData();
        }

        private void LoadData()
        {
            string data = PlayerPrefs.GetString(ALL_AREA_PROGRESS_DATA_KEY);
            if (data.Equals(""))
            {
                _areaPlayerData = new AllAreaPlayerData();
                _areaPlayerData._allAreaProgressDict = SerializeUtility.DeserializeObject<Dictionary<string, AreaPlayerData>>(defaultData);
                _areaPlayerData._currentAreaIdPrefs = "A1";
                SaveData();
                AnalyticsManager.Instance.LogGAEvent("MetaAreaStart : " + _areaPlayerData._currentAreaIdPrefs + " : " + DataManager.PlayerData.playerGameplayLevel);
            }
            else
            {
                _areaPlayerData = JsonConvert.DeserializeObject<AllAreaPlayerData>(data);
            }
        }
        #endregion

        #region CO-ROUTINES
        private IEnumerator<WaitForSeconds> LoadProgress(LoadOperationHandle<GameObject> loadOperationHandle, Action onLoad)
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(1);
            while (!loadOperationHandle.IsDone)
            {
                yield return waitForSeconds;
            }
            onLoad();
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion

        public void SetAreaPlayerData(string areaId, string data)
        {
            _areaPlayerData._currentAreaIdPrefs = areaId;
            _areaPlayerData._allAreaProgressDict = JsonConvert.DeserializeObject<Dictionary<string, AreaPlayerData>>(data);
            SaveData();
        }

#if UNITY_EDITOR
        [ContextMenu("DebugPrefs")]
        public void DebugPrefs()
        {
            Debug.LogError(JsonConvert.SerializeObject(_areaPlayerData._allAreaProgressDict));
        }
#endif


    }
    [Serializable]
    public class AllAreaPlayerData
    {
        public string _currentAreaIdPrefs;
        public Dictionary<string, AreaPlayerData> _allAreaProgressDict;
    }
}
