using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Tag.HexaStack
{
    [CustomPropertyDrawer(typeof(TimeDuration))]
    public class TimeDurationPropertyDrawer : PropertyDrawer
    {
        #region PUBLIC_VARIABLES
        #endregion

        #region PRIVATE_VARIABLES
        private const float SubLabelSpacing = 4;
        private const float BottomSpacing = 2;
        #endregion

        #region PROPERTIES
        #endregion

        #region UNITY_CALLBACKS
        #endregion

        #region PUBLIC_METHODS
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            pos.height -= BottomSpacing;
            label = EditorGUI.BeginProperty(pos, label, prop);
            var contentRect = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label);
            var labels = new[] { new GUIContent("H:", "Hours"), new GUIContent("M:", "Minutes"), new GUIContent("S:", "Seconds") };
            var properties = new[] { prop.FindPropertyRelative("hours"), prop.FindPropertyRelative("minutes"), prop.FindPropertyRelative("seconds") };
            DrawMultiplePropertyFields(contentRect, labels, properties);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + BottomSpacing;
        }
        #endregion

        #region PRIVATE_METHODS
        private static void DrawMultiplePropertyFields(Rect pos, GUIContent[] subLabels, SerializedProperty[] props)
        {
            // backup gui settings
            var indent = EditorGUI.indentLevel;
            var labelWidth = EditorGUIUtility.labelWidth;

            // draw properties
            var propsCount = props.Length;
            var width = (pos.width - (propsCount - 1) * SubLabelSpacing) / propsCount;
            var contentPos = new Rect(pos.x, pos.y, width, pos.height);
            EditorGUI.indentLevel = 0;
            for (var i = 0; i < propsCount; i++)
            {
                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(subLabels[i]).x;
                EditorGUI.PropertyField(contentPos, props[i], subLabels[i]);
                contentPos.x += width + SubLabelSpacing;
            }

            // restore gui settings
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = indent;
        }
        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region COROUTINES
        #endregion

        #region UI_CALLBACKS
        #endregion
    }
}