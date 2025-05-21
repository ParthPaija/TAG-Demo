using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class UndoBoosterUseConditions : BaseBoosterUseConditions
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Image[] images;
        [SerializeField] private Outline[] outlinesBoosterCount;
        [SerializeField] private Outline[] outlinesCoinTextCount;

        [SerializeField] private Color boosterCountTextDefaultColor;
        [SerializeField] private Color coinCountTextDefaultColor;
        [SerializeField] private Color textGrayColor;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public override bool CanUseBooster()
        {
            if (!GameplayManager.Instance.UndoBoosterData.canUseUndoBooster)
            {
                GlobalUIManager.Instance.GetView<ToastMessageView>().ShowMessage("No moves to Undo!", transform.position.With(null, null, 0));
                return false;
            }
            return base.CanUseBooster();
        }

        public void SetGrayState(bool isUseable)
        {
            for (int i = 0; i < images.Length; i++)
            {
                images[i].material = isUseable ? null : ResourceManager.Instance.grayScale;
            }

            for (int i = 0; i < outlinesBoosterCount.Length; i++)
            {
                outlinesBoosterCount[i].effectColor = isUseable ? boosterCountTextDefaultColor : textGrayColor;
            }

            for (int i = 0; i < outlinesCoinTextCount.Length; i++)
            {
                outlinesCoinTextCount[i].effectColor = isUseable ? coinCountTextDefaultColor : textGrayColor;
            }

            //boosterCountText.fontMaterial = isUseable ? boosterCountTextDefaultMat : textGrayMat;
            //coinCountText.fontMaterial = isUseable ? coinCountTextDefaultMat : textGrayMat;
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
