using UnityEngine;

namespace Tag.MetaGame
{
    [CreateAssetMenu(fileName = "DecoreOptionDataSO", menuName = "Scriptable Objects/Meta/DecoreOptionDataSO")]
    public class DecoreOptionDataSO : ScriptableObject
    {
        public OptionData[] optionDatas;
    }
}
