using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class DailyDealsManager : SerializedManager<DailyDealsManager>
    {
        #region PUBLIC_VARS

        [SerializeField] private DailyDealsDataSO dailyDealsData;
        [SerializeField] private List<DailyDealSlotConfig> _currentDeals = new List<DailyDealSlotConfig>();
        [SerializeField] private DailyDealsPlayerData _playerData = new DailyDealsPlayerData();
        public DailyDealsPlayerData DailyDealsPlayerData => _playerData;
        #endregion

        #region PRIVATE_VARS

        private const string DAILY_DEALS_DATA_KEY = "DailyDealsData";
        private bool isInit = false;
        private Coroutine _timerCoroutine;
        #endregion

        #region UNITY_CALLBACKS

        public override void Awake()
        {
            base.Awake();
            OnLoadingDone();
            Init();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public bool IsFreeDealsCliamAvailable()
        {
            if (!IsDailyDealsActive()) return false;

            for (int i = 0; i < _currentDeals.Count; i++)
            {
                if (_currentDeals[i].dealPurchaseData.dealType == DealType.Free)
                {
                    if(!_playerData.slots.Find(x => x.id == _currentDeals[i].prefsId).isClaimed)
                        return true;
                }
            }
            return false;
        }

        public void Init()
        {
            if (isInit)
                return;

            if (IsDailyDealsActive())
            {
                LoadDeals();
                CheckForRefresh();
            }
            isInit = true;
        }

        public List<DailyDealSlotConfig> GetCurrentDeals()
        {
            return _currentDeals;
        }

        public bool ClaimDeal(int dealIndex)
        {
            if (dealIndex < 0 || dealIndex >= _currentDeals.Count)
                return false;

            DailyDealSlotConfig deal = _currentDeals[dealIndex];
            return false;
        }

        public TimeSpan GetTimeUntilRefresh()
        {
            return GetLastRefreshTime().AddDays(1).Subtract(TimeManager.Now);
        }

        public void OnPurchase(int index)
        {
            _playerData.slots[index].isClaimed = true;
            SaveDeals();
        }

        public bool IsDailyDealsActive()
        {
            return dailyDealsData.dailyDealsRemoteConfig.isActive;
        }
        #endregion

        #region PRIVATE_FUNCTIONS

        private void LoadDeals()
        {
            string dealsJson = PlayerPrefs.GetString(DAILY_DEALS_DATA_KEY, "");
            if (string.IsNullOrEmpty(dealsJson))
            {
                GenerateNewDeals();
            }
            else
            {
                _playerData = JsonUtility.FromJson<DailyDealsPlayerData>(dealsJson);
                LoadCurrentDealData();
            }
        }

        private void SaveDeals()
        {
            PlayerPrefs.SetString(DAILY_DEALS_DATA_KEY, JsonUtility.ToJson(_playerData));
            PlayerPrefs.Save();
            Debug.LogError("SaveDeals: " + JsonUtility.ToJson(_playerData));
        }

        private DateTime GetLastRefreshTime()
        {
            return _playerData.lastRefreshTime;
        }

        private void CheckForRefresh()
        {
            int dayDifference = TimeManager.Now.Subtract(GetLastRefreshTime()).Days;
            if (dayDifference >= 1)
            {
                AnalyticsManager.Instance.LogGAEvent("DailyDealsRefresh");
                GenerateNewDeals();
            }
            StartTimer();
        }

        private void StartTimer()
        {
            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            _timerCoroutine = null;
            _timerCoroutine = StartCoroutine(RefreshCheckRoutine());
        }
        private void GenerateNewDeals()
        {
            Debug.LogError("GenerateNewDeals");
            _currentDeals.Clear();
            _currentDeals.AddRange(dailyDealsData.GetRandomDealsFromEachSlot(_playerData));

            Debug.LogError("Current Deals: " + _currentDeals.Count);
            _playerData = new DailyDealsPlayerData();
            _playerData.lastRefreshTime = TimeManager.Now.Date;

            for (int i = 0; i < _currentDeals.Count; i++)
            {
                _playerData.slots.Add(new DailyDealSlotPlayerData(_currentDeals[i].prefsId));
            }
            SaveDeals();
        }

        private void LoadCurrentDealData()
        {
            _currentDeals = new List<DailyDealSlotConfig>();
            _currentDeals.AddRange(dailyDealsData.GetDealsForSlot(_playerData));
        }

        private void SetDealsUi()
        {
            DailyDealsView dailyDealsView = MainSceneUIManager.Instance.GetView<ShopView>().dailyDealsView;
            if (dailyDealsView.gameObject.activeInHierarchy)
            {
                dailyDealsView.Init();
            }
        }

        #endregion

        #region CO-ROUTINES

        private IEnumerator RefreshCheckRoutine()
        {
            DailyDealsView dailyDealsView = MainSceneUIManager.Instance.GetView<ShopView>().dailyDealsView;
            TimeSpan timeSpan = GetLastRefreshTime().AddDays(1).Subtract(TimeManager.Now);
            while (timeSpan.TotalSeconds > 0)
            {
                timeSpan = timeSpan.Subtract(new TimeSpan(0, 0, 1));
                dailyDealsView.UpdateTimerDisplay(timeSpan);
                yield return new WaitForSeconds(1f);
            }
            GenerateNewDeals();
            SetDealsUi();
            StartTimer();
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        #endregion
    }
}

[System.Serializable]
public class DailyDealsPlayerData
{
    public List<DailyDealSlotPlayerData> slots = new List<DailyDealSlotPlayerData>();
    public string lastRefreshTimeString;

    [JsonIgnore]
    public DateTime lastRefreshTime
    {
        get
        {
            if (string.IsNullOrEmpty(lastRefreshTimeString))
                return DateTime.MinValue;
            return DateTime.Parse(lastRefreshTimeString);
        }
        set
        {
            lastRefreshTimeString = value.ToString("o"); // ISO 8601 format
        }
    }
}
[System.Serializable]
public class DailyDealSlotPlayerData
{
    public int id;
    public bool isClaimed;

    public DailyDealSlotPlayerData(int id, bool isClaimed = false)
    {
        this.id = id;
        this.isClaimed = false;
    }
}