using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack.Editor
{
    [HideReferenceObjectPicker, HideLabel, PropertyOrder(-2)]
    public class TitleBar
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        private Texture back;
        private Action backAction;
        private string title;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public TitleBar(string title, Action action)
        {
            this.title = title;
            backAction = action;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        [OnInspectorGUI, PropertyOrder(-10)]
        private void OnInspectorGUI()
        {
            var title = new GUIStyle(SirenixGUIStyles.SectionHeaderCentered);
            title.fixedHeight = 50;
            title.fontSize = 30;
            GUIHelper.RequestRepaint();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUIContent gUIContent = new GUIContent("back");
            if (back == null)
            {
                //back = EditorExtension.GetEditorTexture("back.png");
            }

            if (back != null)
            {
                gUIContent = new GUIContent(back);
            }

            if (backAction != null)
            {
                if (GUILayout.Button(gUIContent, new GUILayoutOption[] { GUILayout.Width(50), GUILayout.Height(50) }))
                    OnBackButton();
            }

            GUILayout.Label(new GUIContent(this.title), title);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            SirenixEditorGUI.HorizontalLineSeparator();
            GUILayout.Space(10);
            GUILayout.EndVertical();
        }

        private void OnBackButton()
        {
            if (backAction != null)
                backAction.Invoke();
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
