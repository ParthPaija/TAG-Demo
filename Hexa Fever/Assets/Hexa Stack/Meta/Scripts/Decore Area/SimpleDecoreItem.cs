using System;
using System.Collections;
using Tag.CoreGame;
using Tag.MetaGame.Animation;
using Tag.MetaGame.TaskSystem;
using UnityEngine;

namespace Tag.MetaGame
{
    public class SimpleDecoreItem : BaseDecoreItem
    {
        #region PUBLIC_VARS

        public BaseAnimatorPlayBehaviour animatorPlayBehaviour;

        #endregion

        #region PRIVATE_VARS

        private DecoreItemPlayerData _decoreItemPlayerData;
        private Action _saveDataAction;
        private TaskManager TaskManager { get { return TaskManager.Instance; } }
        private AreaManager AreaManager { get { return AreaManager.Instance; } }

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void LoadData(string data, Action saveDataAction)
        {
            _saveDataAction = saveDataAction;
            _decoreItemPlayerData = JsonUtility.FromJson<DecoreItemPlayerData>(data);
        }

        public override void LoadItem()
        {
            //animatorPlayBehaviour.SetVisibility(_decoreItemPlayerData.isUnlocked);
            if (_decoreItemPlayerData.isUnlocked)
            {
                animatorPlayBehaviour.SetTaskStatus(true);
                animatorPlayBehaviour.PlayOnAnimation();
                animatorPlayBehaviour.PlayIdelAnimation();
            }
            else
            {
                animatorPlayBehaviour.SetTaskStatus(false);
            }
        }

        public override void StartDecor()
        {
            _decoreItemPlayerData = new DecoreItemPlayerData(id)
            {
                isUnlocked = true
            };
            SetMyPlayerData();
            PlayUnlockAnimation();
        }

        public override string GetData()
        {
            if (_decoreItemPlayerData != null)
            {
                return JsonUtility.ToJson(_decoreItemPlayerData);
            }
            else
            {
                return JsonUtility.ToJson(new DecoreItemPlayerData(id));
            }
        }

        public void PlayUnlockAnimation()
        {
            //BackHandler.SetCanBack(false);
            //AutoPopupHandler.SetCanShowAutoOpenPopup(false);
            StartCoroutine(DecoreAnimation(delegate
            {
                animatorPlayBehaviour.SetTaskStatus(true);
                TaskManager.UpdateCompleteTaskUI();
                //BackHandler.SetCanBack(true);
                //AutoPopupHandler.SetCanShowAutoOpenPopup(true);
            }));
            TaskManager.UpdateCompleteTaskPlayerData();
        }

        public override void PlayRevealAnimation()
        {
            animatorPlayBehaviour.OnPlayAnimation();
        }

        public override void PlayCompleteParticle()
        {
            animatorPlayBehaviour.PlayParticles();
        }

        public override void HideItem()
        {
            //animatorPlayBehaviour.SetVisibility(false);
            animatorPlayBehaviour.SetTaskStatus(false);
            animatorPlayBehaviour.PlayOffAnimation();
        }

        public override float GetAnimationTime()
        {
            return animatorPlayBehaviour.GetAnimationTime();
        }

        public override float GetParticleTime()
        {
            return animatorPlayBehaviour.GetParticleTime();
        }
        public override bool IsItemDecore()
        {
            return _decoreItemPlayerData.isUnlocked;
        }
        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetMyPlayerData()
        {
            AreaManager.SetTaskData(id, JsonUtility.ToJson(_decoreItemPlayerData));
            _saveDataAction.Invoke();
        }

        #endregion

        #region CO-ROUTINES

        private IEnumerator DecoreAnimation(Action action = null)
        {
            animatorPlayBehaviour.OnPlayAnimation();
            yield return new WaitForSeconds(animatorPlayBehaviour.GetAnimationTime());
            animatorPlayBehaviour.PlayParticles();
            yield return new WaitForSeconds(animatorPlayBehaviour.GetParticleTime());
            action?.Invoke();
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}