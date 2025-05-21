using System;

namespace Tag.HexaStack
{
    public class ReSpwanerBooster : BaseBooster
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void OnActive(Action onUse)
        {
            GameplayManager.Instance.CancelUndoBooster();
            InputManager.StopInteraction = true;
            base.OnActive(onUse);
            ItemStackSpawnerManager.Instance.SpwnItemForBooster(false);
            OnUse();
            GameplayManager.Instance.SaveAllDataOfLevel();
        }

        public override void OnUse()
        {
            base.OnUse();
            InputManager.StopInteraction = false;
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
