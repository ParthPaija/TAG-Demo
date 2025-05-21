using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class GameplayBottomView : BaseView
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] UIAnimationHandler uIAnimationHandler;
        [SerializeField] private BoosterItemView[] boosterItemViews;
        [SerializeField] private UndoBoosterUseConditions undoBoosteCondition;
        [SerializeField] private StreakBonusGameplayButton streakBonusGameplayButton;
        [SerializeField] private Text spawnerModeText;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetSpawnerModeText(string spawnerModeString)
        {
            spawnerModeText.text = spawnerModeString;
        }

        public override void Show(Action action = null, bool isForceShow = false)
        {
            base.Show(action, isForceShow);
            SetView();
            streakBonusGameplayButton.Init();
        }

        public void ShowAnimation()
        {
            uIAnimationHandler.ShowAnimation(() => { });
        }

        public void HideAnimation()
        {
            uIAnimationHandler.HideAnimation(() => { });
        }

        public void CheakUndoBoosterCondition()
        {
            if (undoBoosteCondition != null)
                undoBoosteCondition.SetGrayState(GameplayManager.Instance.UndoBoosterData.canUseUndoBooster);
        }

        public void SetView()
        {
            for (int i = 0; i < boosterItemViews.Length; i++)
            {
                boosterItemViews[i].SetView();
            }
        }

        public Vector3 GetBoosterPos(int boosterId)
        {
            for (int i = 0; i < boosterItemViews.Length; i++)
            {
                if (boosterItemViews[i].BoosterId == boosterId)
                {
                    return boosterItemViews[i].transform.position;
                }
            }
            return Vector3.zero;
        }

        public Transform GetPropellerPos()
        {
            return streakBonusGameplayButton.transform;
        }
        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
