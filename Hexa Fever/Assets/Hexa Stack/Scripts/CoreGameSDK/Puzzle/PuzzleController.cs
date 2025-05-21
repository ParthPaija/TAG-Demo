using UnityEngine;

namespace Hexa_Stack.Scripts.CoreGameSDK.Puzzle
{
    public class PuzzleController
    {
        private static PuzzleController instance;
        private readonly LevelNativeBridge levelNativeBridge;

        private PuzzleController()
        {
            levelNativeBridge = new LevelNativeBridge();
        }

        public static PuzzleController GetInstance()
        {
            return instance ??= new PuzzleController();
        }

        public void OnLevelStart(int levelNumber)
        {
            Debug.LogError("Adjust Event Level Start :- " + levelNumber);
#if UNITY_ANDROID && !UNITY_EDITOR
            levelNativeBridge.onLevelStart(levelNumber);
#endif
        }

        public void OnLevelComplete(int levelNumber, int timeToClearLevel)
        {
            Debug.LogError("Adjust Event Level Complete :- " + levelNumber + " Time To Clear Level " + timeToClearLevel);
#if UNITY_ANDROID && !UNITY_EDITOR
            levelNativeBridge.onLevelComplete(levelNumber, timeToClearLevel);
#endif
        }
    }
}