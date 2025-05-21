using System.Collections.Generic;
using UnityEditor;

namespace Tag.HexaStack.Editor
{
    public class PlayerProfileFrameIdAttributesDrawer : BaseIdAttributesDrawer<PlayerProfileFrameIdAttribute>
    {
        protected override void Initialize()
        {
            itemList = AssetDatabase.LoadAssetAtPath<BaseIDMappingConfig>(EditorCosntant.MAPPING_IDS_PATH + "/PlayerProfileFrameId.asset");
            values = new List<string>();
            names = new List<string>();
        }
    }
}