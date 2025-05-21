using System.Collections.Generic;
using UnityEditor;

namespace Tag.HexaStack.Editor
{
    public class CurrencyIdAttributesDrawer : BaseIdAttributesDrawer<CurrencyIdAttribute>
    {
        protected override void Initialize()
        {
            itemList = AssetDatabase.LoadAssetAtPath<BaseIDMappingConfig>(EditorCosntant.MAPPING_IDS_PATH + "/CurrencyIdMappings.asset");
            values = new List<string>();
            names = new List<string>();
        }
    }
}