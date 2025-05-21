using DG.Tweening;
using I2.Loc;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingAnimaton : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private Text displayText;
    [SerializeField] private Localize displayTextLocalize;

    [Header("Text Content")]
    [SerializeField]
    private string[] textMessages = new string[]
    {
        "Loading.",
        "Loading..",
        "Loading..."
    };

    [Header("Animation Settings")]
    [SerializeField] private float displayDuration = 1.5f;

    private Sequence animationSequence;
    private bool isAnimating = false;

    private void Start()
    {
        StartContinuousTextLoop();
    }

    public void StartContinuousTextLoop()
    {
        if (isAnimating)
            return;

        isAnimating = true;

        StartCoroutine(LoopTextMessages());
    }

    private IEnumerator LoopTextMessages()
    {
        int currentIndex = 0;

        while (isAnimating)
        {
            string currentMessage = textMessages[currentIndex];


            if (animationSequence != null) animationSequence.Kill();

            animationSequence = DOTween.Sequence();

            animationSequence.AppendCallback(() => displayTextLocalize.SetTerm(currentMessage));

            yield return animationSequence.WaitForCompletion();

            yield return new WaitForSeconds(displayDuration);

            currentIndex = (currentIndex + 1) % textMessages.Length;
        }
    }
    public void StopTextLoop()
    {
        isAnimating = false;
        StopAllCoroutines();

        if (animationSequence != null)
            animationSequence.Kill();
    }

    private void OnDisable()
    {
        StopTextLoop();
    }

    private void OnDestroy()
    {
        if (animationSequence != null)
            animationSequence.Kill();
    }
}
