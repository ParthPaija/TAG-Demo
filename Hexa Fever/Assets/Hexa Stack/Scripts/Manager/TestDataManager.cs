using UnityEngine;
using System.IO;

namespace Tag.HexaStack
{
    public class TestDataManager : Manager<TestDataManager>
    {
        #region PUBLIC_VARS

        public static bool isDevBuild = true;

        #endregion

        #region PRIVATE_VARS

        private static string path;
        private static string fileName = "Hexa Stack Test Result.txt";

        #endregion

        #region UNITY_CALLBACKS

        public override void Awake()
        {
            path = Path.Combine(Application.persistentDataPath, fileName);
            if (!isDevBuild)
                return;
            CreateFile();
        }

        public override void OnDestroy()
        {
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public static void Print(string printString)
        {
            if (!isDevBuild)
                return;

            string logText = $"{printString} \n";

            //Debug.LogError(printString);

            File.AppendAllText(path, logText);
        }

        public static void NewLine()
        {
            File.AppendAllText(path, "\n");
        }

        private void CreateFile()
        {
            if (!isDevBuild)
                return;

            if (!File.Exists(path))
            {
                File.Create(path);
            }
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        [ContextMenu("Test")]
        public void Print()
        {
            Debug.Log(Application.persistentDataPath);
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
