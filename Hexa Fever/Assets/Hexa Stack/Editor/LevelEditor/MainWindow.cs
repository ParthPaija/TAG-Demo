using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Tag.HexaStack.Editor
{
    public class MainWindow : OdinEditorWindow
    {
        #region PUBLIC_VARS

        public TitleBar titleBar;

        public GamePlayMapCreatore mapCreatore;
        public TileSetCreatore tileSetCreatore;

        public static MainWindow Instance
        {
            get { return instance; }
        }

        #endregion

        #region PRIVATE_VARS

        private static MainWindow instance;

        #endregion

        #region PUBLIC_FUNCTIONS

        [MenuItem(Constant.GAME_NAME + "/Editor/Level Editor")]
        private static void OpenWindow()
        {
            instance = GetWindow<MainWindow>();
            instance?.Show();
        }

        protected override void OnEnable()
        {
            if (!instance)
                instance = GetWindow<MainWindow>();
            titleBar = new TitleBar(Constant.GAME_NAME.ToUpper(), null);
            if (mapCreatore == null)
                mapCreatore = new GamePlayMapCreatore();
            if (tileSetCreatore == null)
                tileSetCreatore = new TileSetCreatore();
            SceneView.duringSceneGui += this.OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= this.OnSceneGUI;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        void OnSceneGUI(SceneView sceneView)
        {
            if (mapCreatore != null)
            {
                mapCreatore.OnSceneGui();
            }

            if (tileSetCreatore != null)
            {
                tileSetCreatore.OnSceneGui();
            }
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
