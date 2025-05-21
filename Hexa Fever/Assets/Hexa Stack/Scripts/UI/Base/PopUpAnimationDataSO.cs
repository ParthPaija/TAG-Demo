using UnityEngine;

namespace Tag.HexaStack
{
    [CreateAssetMenu(fileName = "PopUpAnimationDataSO", menuName = "Scriptable Objects/PopUpAnimationDataSO")]
    public class PopUpAnimationDataSO : ScriptableObject
    {
        public Vector2 interval;
        public Vector2 speed;
        public Vector3 startScale;
        public Vector3 endScale;

        [Header("Curves")]
        public AnimationCurve xInCurve;
        public AnimationCurve yInCurve;
        public AnimationCurve xOutCurve;
        public AnimationCurve yOutCurve;

        [Header("BG")]
        public Gradient bgColor;
        public Vector2 bgSpeed;
        public Vector2 canvasAlpha = new Vector2(0, 1);
        public float canvasAlphaFadeSpeed = 2f;
    }
}
