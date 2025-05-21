using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class SetRewardChild : MonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private RectTransform parent;
        [SerializeField] private RectTransform selfRectTransfrom;
        [SerializeField] private GameObject[] gameObjects;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void Set()
        {
            bool isActive = false;
            for (int i = 0; i < gameObjects.Length; i++)
            {
                if (gameObjects[i].activeSelf)
                    isActive = true;
            }
            gameObject.SetActive(isActive);
            LayoutRebuilder.ForceRebuildLayoutImmediate(selfRectTransfrom);
            LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
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
