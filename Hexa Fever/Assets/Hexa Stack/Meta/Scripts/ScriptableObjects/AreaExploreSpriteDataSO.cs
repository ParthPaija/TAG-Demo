using UnityEngine;

namespace Tag.MetaGame
{
    [CreateAssetMenu(fileName = "AreaExploreSpriteDataSO", menuName = "Scriptable Objects/Meta/AreaExploreSpriteDataSO")]

    public class AreaExploreSpriteDataSO : ScriptableObject
    {
        #region PUBLIC_VARS
        public string areaName;
        public Sprite areaEmptySprite;
        public Sprite areaCompleteSprite;

        #endregion


    }
}
