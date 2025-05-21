using System;
using System.Collections.Generic;
using Tag.CommonPurchaseSystem;
using Tag.HexaStack;
using UnityEngine;

namespace Tag.MetaGame
{
    public class OptionSelectionView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        private BaseDecoreOptionView _selectedOptionView;
        private Action<int> _onSuccessAction;
        private Action<int> _onOptionChangeAction;
        private Action _onCloseAction;

        private OptionData[] _optionDatas;
        private BaseDecoreOptionView[] _baseOptionViews;

        [SerializeField] private Transform _holderTransform;
        [SerializeField] private GameObject _closeButtonGO;
        [SerializeField] private BaseDecoreOptionView[] _baseOptionViewPrefabs;

        private AreaEditMode AreaEditMode { get { return AreaEditMode.Instance; } }
        //private SoundHandler SoundHandler { get { return FindObjectFromLibrary<SoundHandler>(); } }
        //private BackHandler BackHandler { get { return FindObjectFromLibrary<BackHandler>(); } }

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetView(OptionData[] optionDatas, DecoreItemWithOptionPlayerData optionPlayerData, bool canClose)
        {
            DestroyOldViews();
            AreaEditMode.HideIcons();
            _optionDatas = optionDatas;
            _closeButtonGO.SetActive(canClose);
            GenerateOption(optionPlayerData.ownOptions);
            _selectedOptionView = _baseOptionViews[optionPlayerData.selectedOption];
            gameObject.SetActive(true);
            _selectedOptionView.SelectOption();
            //BackHandler.Add(this);
        }

        public void RegisterActions(Action<int> onSuccessAction, Action onCloseAction, Action<int> onOptionChangeAction)
        {
            _onSuccessAction = onSuccessAction;
            _onCloseAction = onCloseAction;
            _onOptionChangeAction = onOptionChangeAction;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void GenerateOption(List<int> ownOptions)
        {
            _baseOptionViews = new BaseDecoreOptionView[_optionDatas.Length];
            for (int i = 0; i < _optionDatas.Length; i++)
            {
                PurchaseType purchaseType = PurchaseType.None;
                if (ownOptions.Contains(i))
                {
                    purchaseType = PurchaseType.Free;
                }
                else
                {
                    purchaseType = _optionDatas[i].basePurchaseDataSO.PurchaseType;
                    ShowCurrencyViewIfNeeded();
                }
                BaseDecoreOptionView baseOptionView = GenerateOptionViewByType(purchaseType);
                baseOptionView.SetView(i, _optionDatas[i], OnChangeSelection);
                _baseOptionViews[i] = baseOptionView;

                void ShowCurrencyViewIfNeeded()
                {
                    if (_optionDatas[i].basePurchaseDataSO.PurchaseType == PurchaseType.Currency)
                    {
                        CurrencyPurchaseDataSO basePurchaseDataSO = (CurrencyPurchaseDataSO)_optionDatas[i].basePurchaseDataSO;
                        //FindObjectFromLibraryById<CurrencyView>(basePurchaseDataSO.currencyData.rewardId).ShowElement();
                    }
                }
            }
        }

        private BaseDecoreOptionView GenerateOptionViewByType(PurchaseType purchaseType)
        {
            for (int i = 0; i < _baseOptionViewPrefabs.Length; i++)
            {
                if (_baseOptionViewPrefabs[i].PrefabType == purchaseType)
                {
                    return Instantiate(_baseOptionViewPrefabs[i], _holderTransform);
                }
            }
            throw new Exception("Purchase type is invalid!! " + purchaseType);
        }

        private void OnChangeSelection(int selectedIndex)
        {
            if (_selectedOptionView != _baseOptionViews[selectedIndex])
            {
                _selectedOptionView.DeselectOption();
                _selectedOptionView = _baseOptionViews[selectedIndex];
                _selectedOptionView.SelectOption();
                _onOptionChangeAction.Invoke(selectedIndex);
            }
        }

        private void ResetView()
        {
            _optionDatas = new OptionData[0];
            _onSuccessAction = null;
            _selectedOptionView = null;
        }
        private void DestroyOldViews()
        {
            if (_baseOptionViews == null) return;
            for (int i = 0; i < _baseOptionViews.Length; i++)
            {
                MonoBehaviour.Destroy(_baseOptionViews[i].gameObject);
            }
            _baseOptionViews = new BaseDecoreOptionView[0];
        }
        private void SuccessAction()
        {
            _onSuccessAction.Invoke(_selectedOptionView.Index);
            gameObject.SetActive(false);
            ResetView();
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS
        public void OnBack()
        {

        }
        #endregion

        #region UI_CALLBACKS       

        public void OnSaveButtonClick()
        {
            Debug.Log("Save Button Clicked");
            //SoundHandler.PlaySound(SoundType.ButtonClick);
            _selectedOptionView.BuyOption(SuccessAction);
        }

        public void OnCloseButtonClick()
        {
            //SoundHandler.PlaySound(SoundType.ButtonClick);
            gameObject.SetActive(false);
            _onCloseAction.Invoke();
            AreaEditMode.ShowIcons();
            ResetView();
            //BackHandler.Remove(this);
        }



        #endregion
    }
}
