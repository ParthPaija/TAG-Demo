using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack
{
    [CreateAssetMenu(fileName = "ElementIntroData", menuName = Constant.GAME_NAME + "/ElementIntroData")]
    public class ElementIntroDataSO : SerializedScriptableObject
    {
        #region PUBLIC_VARS

        public List<ElementIntroData> elementIntroDatas = new List<ElementIntroData>();

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public ElementIntroData CanShowElementIntro(int level)
        {
            for (int i = 0; i < elementIntroDatas.Count; i++)
            {
                if (elementIntroDatas[i].CanShow(level))
                    return elementIntroDatas[i];
            }
            return null;
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

    public class ElementIntroData
    {
        [ObstacalId] public int elementID;
        public string elementName;
        public string elementPrefskey;
        public string elementDes;
        public int unlockLevel;
        public Sprite elementSprite;

        public bool CanShow(int level)
        {
            return PlayerPrefs.GetInt(elementPrefskey) == 0 && unlockLevel == level;
        }

        public void SetAsShow()
        {
            PlayerPrefs.SetInt(elementPrefskey, 1);
        }
    }
}
