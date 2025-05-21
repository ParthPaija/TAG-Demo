using System.Collections.Generic;
using UnityEditor;

namespace Tag.HexaStack.Editor
{
    public class ItemIdAttributesDrawer : BaseIdAttributesDrawer<ItemIdAttribute>
    {
        protected override void Initialize()
        {
            itemList = AssetDatabase.LoadAssetAtPath<BaseIDMappingConfig>(EditorCosntant.MAPPING_IDS_PATH + "/ItemIdMappings.asset");
            values = new List<string>();
            names = new List<string>();
        }
    }
}
