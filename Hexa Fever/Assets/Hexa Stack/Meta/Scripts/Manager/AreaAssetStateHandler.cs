using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Tag.AssetManagement;
using Tag.HexaStack;
using UnityEngine;

namespace Tag.MetaGame
{
    public class AreaAssetStateHandler : Manager<AreaAssetStateHandler>
    {
        #region PUBLIC_VARS

        public int TotalAreaNo { get; private set; }

        #endregion

        #region PRIVATE_VARS

        [ShowInInspector] private readonly Dictionary<string, AssetState> _assetStates = new Dictionary<string, AssetState>();
        private IAssetManager AssetManager { get { return AssetManagerAddressable.Instance; } }
        private InternetManager InternetManager { get { return InternetManager.Instance; } }

        private Action _onInitDone;

        #endregion

        #region UNITY_CALLBACKS
        public override void Awake()
        {
            base.Awake();
            Init(OnLoadingDone);
        }
        #endregion

        #region PUBLIC_FUNCTIONS
        public void Init(Action onInitDone)
        {
            _onInitDone = onInitDone;
            CheckAllAssetStatus();
        }
        public AssetState GetAssetState(string assetKey)
        {
            //Debug.LogError("GetAssetState " + assetKey);
            if (_assetStates.ContainsKey(assetKey))
                return _assetStates[assetKey];
            return AssetState.NONE;
        }
        public void SetAssetState(string assetKey, AssetState assetState)
        {
            if (_assetStates.ContainsKey(assetKey))
                _assetStates[assetKey] = assetState;
            else
                _assetStates.Add(assetKey, assetState);
        }
        #endregion

        #region PRIVATE_FUNCTIONS
        private void CheckAllAssetStatus()
        {
            InternetManager.CheckNetConnection((IsConnectionAvailable) =>
            {
                if (IsConnectionAvailable)
                {
                    StartCoroutine(CheckAllAssetStatusCO());
                }
                else
                {
                    DownloadFail();
                }
            });
        }

        private void DownloadFail()
        {
            Debug.Log("donwloadFail");
            StopAllCoroutines();
            InternetManager.Instance.CheckNetConnection(Redownload);
            //   AssetDownloadFailedPopup.ShowPopup(Redownload);
        }

        private void Redownload()
        {
            Debug.Log("Redownload");
            StartCoroutine(CheckAllAssetStatusCO());
        }
        #endregion

        #region CO-ROUTINES

        private IEnumerator CheckAllAssetStatusCO()
        {
            TotalAreaNo = 0;
            AssetAddress assetAddress;
            while (true)
            {
                assetAddress = AssetAddress.GenerateAreaAddress(AreaUtility.AreaNoToAreaId(TotalAreaNo + 1));
                yield return CheckAssetStatus(assetAddress);

                assetAddress = AssetAddress.GenerateAreaExploreSpriteDataAddress(AreaUtility.AreaNoToAreaId(TotalAreaNo + 1));
                yield return CheckAssetStatus(assetAddress);
                TotalAreaNo++;
            }
            //            _onInitDone?.Invoke();
        }

        private IEnumerator CheckAssetStatus(AssetAddress assetAddress)
        {
            AssetOperationHandle validAddressHandle = AssetManager.IsValidAddress(assetAddress);

            while (!validAddressHandle.IsDone)
                yield return null;
            //Debug.LogError("valid Address " + assetAddress.assetKey + validAddressHandle.IsValidAddress);
            if (!validAddressHandle.status)
            {
                DownloadFail();
                yield break;
            }

            if (!validAddressHandle.IsValidAddress)
            {
                _onInitDone?.Invoke();
                //                _assetStates.Add(assetAddress.assetKey, AssetState.INVALID_ADDRESS);
                StopAllCoroutines();
                yield break;
            }

            AssetOperationHandle assetOperationHandle = AssetManager.IsAssetDownloaded(assetAddress);
            while (!assetOperationHandle.IsDone)
                yield return null;

            //Debug.LogError("DownloadState " + assetAddress.assetKey + " " + assetOperationHandle.IsDownloaded);
            if (!assetOperationHandle.status)
            {
                DownloadFail();
                yield break;
            }

            if (assetOperationHandle.IsDownloaded)
                _assetStates.Add(assetAddress.assetKey, AssetState.DOWNLOADED);
            else
                _assetStates.Add(assetAddress.assetKey, AssetState.NOT_DOWNLOAD);
        }
        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
    public enum AssetState
    {
        NONE = 1,
        NOT_DOWNLOAD = 2,
        DOWNLOADED = 3,
        INVALID_ADDRESS = 4
    }
}
