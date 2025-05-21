using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    public class CommonOfferManager : SerializedManager<CommonOfferManager>
    {
        #region PUBLIC_VARS
        public List<CommonOfferRemoteConfigData> CommonOfferRemoteConfigDatas { get => commonOfferRemoteConfigDatas; }
        public List<CommonOfferData> OfferData { get => offerData; }
        public CommonOfferPlayerPersistantData PlayerPersistantOfferDataData { get => playerPersistantData; }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private CommonOfferConfigDataSO offerConfigDataSO;
        [SerializeField] private CommonOfferPlayerPersistantData playerPersistantData;
        [SerializeField] private List<CommonOfferData> offerData;
        [SerializeField] private List<CommonOfferRemoteConfigData> commonOfferRemoteConfigDatas;
        [SerializeField] private Dictionary<int, CommonOfferView> offersView = new Dictionary<int, CommonOfferView>();
        [SerializeField] private Dictionary<int, CommonOfferButton> offersButton = new Dictionary<int, CommonOfferButton>();

        #endregion

        #region UNITY_CALLBACKS

        private void Start()
        {
            commonOfferRemoteConfigDatas = offerConfigDataSO.GetValue<List<CommonOfferRemoteConfigData>>();
            Init();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void Init()
        {
            LoadPlayerData();
            foreach (var item in offersView)
            {
                item.Value.Init();
            }
            for (int i = 0; i < offerData.Count; i++)
            {
                offerData[i].Init();
            }
            SetButton();
        }

        public CommonOfferRemoteConfigData GetOfferRemoteConfigData(int offerId)
        {
            return commonOfferRemoteConfigDatas.Find(x => x.offerId == offerId);
        }

        public CommonOfferData GetOfferData(int offerId)
        {
            return offerData.Find(x => x.offerId == offerId);
        }

        public void ShowOfferView(int offerId)
        {
            if (offersView.ContainsKey(offerId))
                offersView[offerId].ShowView(GetOfferData(offerId).iapShopBundleData);
        }

        public void ShowOfferWithHideAction(int offerId, Action actionToCallOnHide)
        {
            if (offersView.ContainsKey(offerId))
                offersView[offerId].ShowWithHideAction(actionToCallOnHide,GetOfferData(offerId).iapShopBundleData);
        }

        public void OnOfferPurchase(int offerId)
        {
            playerPersistantData.offerPucrchaseData.Add(offerId);
            SavePlayerData();
            SetButton();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void LoadPlayerData()
        {
            playerPersistantData = PlayerPersistantData.GetCommonOfferData();

            if (playerPersistantData == null)
            {
                playerPersistantData = new CommonOfferPlayerPersistantData();
                playerPersistantData.offerPucrchaseData = new List<int>();
            }
            SavePlayerData();
        }

        private void SavePlayerData()
        {
            PlayerPersistantData.SetCommonOfferData(playerPersistantData);
        }

        private void SetButton()
        {
            foreach (var item in offersButton)
            {
                if (item.Value != null)
                {
                    Debug.Log("Offer Cheak Button :- " + item.Value.name);
                    if (GetOfferData(item.Key).IsActive())
                    {
                        Debug.Log("Offer Active Button :- " + item.Value.name);
                        item.Value.Show();
                    }
                    else
                    {
                        Debug.Log("Offer Active Nathi Button :- " + item.Value.name);
                        item.Value.Hide();
                    }
                }
            }
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

        #endregion
    }

    public class CommonOfferData
    {
        public int offerId;
        public string offerName;
        public IapShopBundleData iapShopBundleData;
        public List<BaseCommonOfferUnlockConditions> commonOfferUnlockConditions;

        public void Init()
        {
            for (int i = 0; i < commonOfferUnlockConditions.Count; i++)
            {
                commonOfferUnlockConditions[i].Init(offerId);
            }
        }

        public bool IsActive()
        {
            for (int i = 0; i < commonOfferUnlockConditions.Count; i++)
            {
                if (commonOfferUnlockConditions[i].IsActive())
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class BaseCommonOfferUnlockConditions
    {
        private int offerId;

        public virtual void Init(int offerId)
        {
            this.offerId = offerId;
        }

        public virtual bool IsActive()
        {
            CommonOfferRemoteConfigData configData = CommonOfferManager.Instance.GetOfferRemoteConfigData(offerId);
            if (!configData.isActive)
                return false;
            if (DataManager.PlayerData.playerGameplayLevel < configData.openAt)
                return false;
            return true;
        }
    }

    public class SpecificeOfferNotActiveUnlockConditions : BaseCommonOfferUnlockConditions
    {
        public int specificOfferId;

        public override bool IsActive()
        {
            CommonOfferRemoteConfigData configData = CommonOfferManager.Instance.GetOfferRemoteConfigData(specificOfferId);
            if (configData.isActive)
                return false;
            return base.IsActive();
        }
    }

    public class SpecificeOfferPurchaseUnlockConditions : BaseCommonOfferUnlockConditions
    {
        public int specificOfferId;

        public override bool IsActive()
        {
            if (!CommonOfferManager.Instance.PlayerPersistantOfferDataData.IsOfferPucrhased(specificOfferId))
                return false;
            return base.IsActive();
        }
    }

    public class SpecificeOfferNotPurchaseUnlockConditions : BaseCommonOfferUnlockConditions
    {
        public int specificOfferId;

        public override bool IsActive()
        {
            if (CommonOfferManager.Instance.PlayerPersistantOfferDataData.IsOfferPucrhased(specificOfferId))
                return false;
            return base.IsActive();
        }
    }

    public class CommonOfferPlayerPersistantData
    {
        [JsonProperty("opd")] public List<int> offerPucrchaseData;

        public bool IsOfferPucrhased(int offerPucrchaseId)
        {
            return offerPucrchaseData != null && offerPucrchaseData.Contains(offerPucrchaseId);
        }
    }

    public class CommonOfferRemoteConfigData
    {
        public int offerId;
        public bool isActive;
        public int openAt;
    }
}
