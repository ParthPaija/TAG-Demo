using System;
using Tag.HexaStack;
using Tag.MetaGame.TaskSystem;
using UnityEngine;

namespace Tag.MetaGame
{
    public class DecoreItemWithOption : BaseDecoreItem
    {
        #region PUBLIC_VARS

        public DecoreIcon _decoreIcon;

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private DecoreOptionDataSO _decoreOptionDataSO;
        [SerializeField] private DecoreOptionObject[] _decoreOptionObjects;

        private DecoreOptionObject _currentSelectedOption;
        private DecoreItemWithOptionPlayerData _decoreItemWithOptionPlayerData;
        private Action _saveDataAction;

        private TaskManager TaskManager { get { return TaskManager.Instance; } }
        private AreaEditMode AreaEditMode { get { return AreaEditMode.Instance; } }
        private AreaManager AreaManager { get { return AreaManager.Instance; } }
        private OptionSelectionView OptionSelectionView { get { return MainSceneUIManager.Instance.GetView<OptionSelectionView>(); } }

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void LoadData(string data, Action saveDataAction)
        {
            _saveDataAction = saveDataAction;
            _decoreItemWithOptionPlayerData = JsonUtility.FromJson<DecoreItemWithOptionPlayerData>(data);
            if (_decoreItemWithOptionPlayerData.isUnlocked)
            {
                _currentSelectedOption = _decoreOptionObjects[_decoreItemWithOptionPlayerData.selectedOption];
                _decoreIcon.RegisterOnClickAction(delegate { ShowOptionSelectionView(); });
            }
        }

        public override void LoadItem()
        {
            if (_decoreItemWithOptionPlayerData.isUnlocked)
            {
                ShowAllObjects();
            }
        }

        public override void StartDecor()
        {
            _decoreItemWithOptionPlayerData = new DecoreItemWithOptionPlayerData(id)
            {
                isUnlocked = true
            };
            SetMyPlayerData();
            PlayUnlockAnimation();
            _decoreIcon.RegisterOnClickAction(delegate { ShowOptionSelectionView(); });
        }

        public override string GetData()
        {
            if (_decoreItemWithOptionPlayerData != null)
            {
                return JsonUtility.ToJson(_decoreItemWithOptionPlayerData);
            }
            else
            {
                return JsonUtility.ToJson(new DecoreItemWithOptionPlayerData(id));
            }
        }

        public override void PlayRevealAnimation()
        {
            _currentSelectedOption.animatorPlayBehaviour.OnPlayAnimation();
        }

        public override void PlayCompleteParticle()
        {
            _currentSelectedOption.animatorPlayBehaviour.PlayParticles();
        }

        public override void HideItem()
        {
            //_currentSelectedOption.animatorPlayBehaviour.SetVisibility(false);
            _currentSelectedOption.animatorPlayBehaviour.SetTaskStatus(false);
            _currentSelectedOption.animatorPlayBehaviour.PlayOffAnimation();
        }

        public override float GetAnimationTime()
        {
            return _currentSelectedOption.animatorPlayBehaviour.GetAnimationTime();
        }

        public override float GetParticleTime()
        {
            return _currentSelectedOption.animatorPlayBehaviour.GetParticleTime();
        }

        public override bool IsItemDecore()
        {
            return _decoreItemWithOptionPlayerData.isUnlocked;
        }
        #endregion

        #region PRIVATE_FUNCTIONS

        private void TaskComplete(int index)
        {
            _decoreOptionObjects[index].PlayAnimation(delegate
             {
                 AreaEditMode.AreaEditModeOff();
                 TaskManager.UpdateCompleteTaskUI();
             });
            TaskManager.UpdateCompleteTaskPlayerData();
            SaveData(index);
            _saveDataAction.Invoke();
        }

        private void SaveOptionSelection(int index)
        {
            _decoreOptionObjects[index].PlayAnimation(delegate
             {
                 AreaEditMode.ShowIcons();
             });
            SaveData(index);
            _saveDataAction.Invoke();
        }

        private void SaveData(int index)
        {
            _decoreItemWithOptionPlayerData.selectedOption = index;
            _decoreItemWithOptionPlayerData.OwnSelectedOption();
            SetMyPlayerData();
        }

        private void RevertDecore()
        {
            ChangeOption(_decoreItemWithOptionPlayerData.selectedOption);
        }

        private void ChangeOption(int index)
        {
            if (_currentSelectedOption != _decoreOptionObjects[index])
            {
                //_currentSelectedOption.animatorPlayBehaviour.SetVisibility(false);
                _currentSelectedOption.animatorPlayBehaviour.SetTaskStatus(false);
                _currentSelectedOption.animatorPlayBehaviour.PlayOffAnimation();
                _currentSelectedOption = _decoreOptionObjects[index];
                //_decoreOptionObjects[index].animatorPlayBehaviour.SetVisibility(true);
                _decoreOptionObjects[index].animatorPlayBehaviour.SetTaskStatus(true);
                _decoreOptionObjects[index].animatorPlayBehaviour.PlayOnAnimation();
            }
        }

        private void ShowOptionSelectionView()
        {
            OptionSelectionView optionSelectionView = OptionSelectionView;
            optionSelectionView.RegisterActions(SaveOptionSelection, RevertDecore, ChangeOption);
            optionSelectionView.SetView(_decoreOptionDataSO.optionDatas, _decoreItemWithOptionPlayerData, true);
        }

        private void PlayUnlockAnimation()
        {
            _currentSelectedOption = _decoreOptionObjects[_decoreItemWithOptionPlayerData.selectedOption];
            //_currentSelectedOption.animatorPlayBehaviour.SetVisibility(true);
            _currentSelectedOption.animatorPlayBehaviour.SetTaskStatus(true);
            _currentSelectedOption.animatorPlayBehaviour.PlayOnAnimation();
            OptionSelectionView optionSelectionView = OptionSelectionView;
            optionSelectionView.RegisterActions(TaskComplete, RevertDecore, ChangeOption);
            optionSelectionView.SetView(_decoreOptionDataSO.optionDatas, _decoreItemWithOptionPlayerData, false);
        }

        private void ShowAllObjects()
        {
            //_decoreOptionObjects[_decoreItemWithOptionPlayerData.selectedOption].animatorPlayBehaviour.SetVisibility(true);
            _decoreOptionObjects[_decoreItemWithOptionPlayerData.selectedOption].animatorPlayBehaviour.SetTaskStatus(true);
            _decoreOptionObjects[_decoreItemWithOptionPlayerData.selectedOption].animatorPlayBehaviour.PlayOnAnimation();
            _decoreOptionObjects[_decoreItemWithOptionPlayerData.selectedOption].animatorPlayBehaviour.PlayIdelAnimation();
        }

        private void SetMyPlayerData()
        {
            AreaManager.SetTaskData(id, JsonUtility.ToJson(_decoreItemWithOptionPlayerData));
        }


        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}