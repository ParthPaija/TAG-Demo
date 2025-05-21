using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tag.HexaStack.Editor
{
    public class ObstacalIdAttributesDrawer : BaseIdAttributesDrawer<ObstacalIdAttribute>
    {
        protected override void Initialize()
        {
            itemList = AssetDatabase.LoadAssetAtPath<BaseIDMappingConfig>(EditorCosntant.MAPPING_IDS_PATH + "/ObstacalIdMappings.asset");
            values = new List<string>();
            names = new List<string>();
        }
    }
}
