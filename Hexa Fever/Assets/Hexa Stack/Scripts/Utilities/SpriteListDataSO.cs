using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tag.HexaStack
{
    [CreateAssetMenu(fileName = "SpriteListDataSO", menuName = Constant.GAME_NAME + "/Common/SpriteListDataSO")]
    public class SpriteListDataSO : SerializedScriptableObject
    {
        #region PUBLIC_VARS

        public List<Sprite> data;

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public List<Sprite> GetRandomList(int count)
        {
            List<Sprite> newList = new List<Sprite>();
            newList.AddRange(data);
            newList.Shuffle();
            return newList.Take(count).ToList();
        }

        public Sprite GetDataAtIndex(int index)
        {
            if (index >= 0 && index < data.Count)
                return data[index];

            return data.GetRandomItemFromList();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS

#if UNITY_EDITOR

        public List<Sprite> sprites;

        [Button]
        public void SetData(int count)
        {
            data = new List<Sprite>();
            for (int i = 0; i < count; i++)
            {
                data.Add(sprites[Random.Range(0, sprites.Count)]);
            }
        }

#endif

        #endregion
    }
}
