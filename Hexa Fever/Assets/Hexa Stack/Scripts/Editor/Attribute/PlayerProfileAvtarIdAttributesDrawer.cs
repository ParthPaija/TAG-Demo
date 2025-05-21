using System.Collections.Generic;
using UnityEditor;

namespace Tag.HexaStack.Editor
{
    public class PlayerProfileAvtarIdAttributesDrawer : BaseIdAttributesDrawer<PlayerProfileAvtarIdAttribute>
    {
        protected override void Initialize()
        {
            itemList = AssetDatabase.LoadAssetAtPath<BaseIDMappingConfig>(EditorCosntant.MAPPING_IDS_PATH + "/PlayerProfileAvtarId.asset");
            values = new List<string>();
            names = new List<string>();
        }
    }
}