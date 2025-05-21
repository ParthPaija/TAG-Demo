using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class TutorialElementHandler : SerializedManager<TutorialElementHandler>
    {
        #region PUBLIC_VARS

        public SpriteRenderer tutorialBackGroundImage;
        public SpriteRenderer tutorialBackGroundImageObejct;
        public Transform tutorialUIBlockerImage;
        public TutorialHandAnimation tutorialHandAnimationUI;
        public TutorialHandAnimation tutorialHandAnimationObjects;
        public TutorialChatView tutorialChatView;

        #endregion

        #region PRIVATE_VARS

        private Action _highlighterMouseUpAction;
        private Action _highlightercEndDragAction;
        [SerializeField] private Action _highlightercPointerTap;
        [SerializeField] private TutorialHighlighterData[] highlighterDatasUI;
        [SerializeField] private TutorialHighlighterData[] highlighterDatasObjects;

        #endregion

        #region UNITY_CALLBACKS

        public override void Awake()
        {
            base.Awake();
            InitHandler();
            OnLoadingDone();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void InitHandler()
        {
            tutorialChatView.Init();
        }

        public void SetUIBlocker(bool state)
        {
            tutorialUIBlockerImage.gameObject.SetActive(state);
        }

        public void SetActiveTapHandUI(bool state, Vector3 target, bool playTranslateAnimation = false)
        {
            tutorialHandAnimationUI.gameObject.SetActive(state);
            if (state)
            {
                tutorialHandAnimationUI.endPosition.position = target;
                tutorialHandAnimationUI.startPosition.position = Vector3.zero;
                tutorialHandAnimationUI.OnTapAnimation(playTranslateAnimation);
            }
        }

        public void SetActiveTapHandObject(bool state, Vector3 target, bool playTranslateAnimation = false)
        {
            tutorialHandAnimationObjects.gameObject.SetActive(state);
            if (state)
            {
                tutorialHandAnimationObjects.endPosition.position = target;
                tutorialHandAnimationObjects.startPosition.position = Vector3.zero;
                tutorialHandAnimationObjects.OnTapAnimation(playTranslateAnimation);
            }
        }

        public void RegisterOnHighlighterTap(Action highlighterMouseUpAction)
        {
            _highlightercPointerTap = highlighterMouseUpAction;
        }

        public void DeregisterOnHighlighterActions()
        {
            _highlightercEndDragAction = null;
            _highlighterMouseUpAction = null;
            _highlightercPointerTap = null;
        }

        public void SetActivateBackGround(bool state, float alpha)
        {
            if (state)
            {
                tutorialBackGroundImage.gameObject.SetActive(true);
                if (alpha > 0)
                    StartCoroutine(BackGroundIn(alpha));
                else
                    StartCoroutine(BackGroundOut());
            }
            else
                StartCoroutine(BackGroundOut(() => tutorialBackGroundImage.gameObject.SetActive(false)));
        }

        public void SetBGForObjectsTurorials(bool state)
        {
            tutorialBackGroundImageObejct.gameObject.SetActive(state);
        }

        public TutorialHighlighterData GetUIHighlighterTransformByType(TutorialHighliterType tutorialHighliterType)
        {
            for (int i = 0; i < highlighterDatasUI.Length; i++)
            {
                if (tutorialHighliterType == highlighterDatasUI[i].tutorialHighliterType)
                    return highlighterDatasUI[i];
            }
            return null;
        }

        public TutorialHighlighterData GetObjectHighlighterTransformByType(TutorialHighliterType tutorialHighliterType)
        {
            for (int i = 0; i < highlighterDatasObjects.Length; i++)
            {
                if (tutorialHighliterType == highlighterDatasObjects[i].tutorialHighliterType)
                    return highlighterDatasObjects[i];
            }
            return null;
        }

        public void SetUIHighlighter_ByWorldPosition(Vector3 worldPosition, TutorialHighliterType tutorialHighliterType = TutorialHighliterType.CircleHighlighter, bool isShowHighlighterImage = false, bool isEnableHighlighterTapButton = true)
        {
            var highLighter = GetUIHighlighterTransformByType(tutorialHighliterType);
            highLighter.mainRectTransform.gameObject.SetActive(true);
            highLighter.mainRectTransform.gameObject.transform.position = worldPosition;
            highLighter.highLighterImage.gameObject.SetActive(isShowHighlighterImage);
            highLighter.highLighterButton.interactable = isEnableHighlighterTapButton;
        }

        public void SetObjectHighlighter_ByWorldPosition(Vector3 worldPosition, TutorialHighliterType tutorialHighliterType = TutorialHighliterType.CircleHighlighter, bool isShowHighlighterImage = false, bool isEnableHighlighterTapButton = true)
        {
            var highLighter = GetObjectHighlighterTransformByType(tutorialHighliterType);
            highLighter.mainRectTransform.gameObject.SetActive(true);
            highLighter.mainRectTransform.gameObject.transform.position = worldPosition;
            highLighter.highLighterImage.gameObject.SetActive(isShowHighlighterImage);
            highLighter.highLighterButton.interactable = isEnableHighlighterTapButton;
        }

        public void ResetHighLighters()
        {
            for (int i = 0; i < highlighterDatasUI.Length; i++)
            {
                highlighterDatasUI[i].mainRectTransform.gameObject.SetActive(false);
            }

            for (int i = 0; i < highlighterDatasObjects.Length; i++)
            {
                highlighterDatasObjects[i].mainRectTransform.gameObject.SetActive(false);
            }
        }

        public void ResetTutorialView()
        {
            tutorialChatView.Hide();
            SetUIBlocker(false);
            SetActivateBackGround(false, 0f);
            SetActiveTapHandUI(false, Vector3.zero);
            SetActiveTapHandObject(false, Vector3.zero);
            ResetHighLighters();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        IEnumerator BackGroundIn(float alpha)
        {
            Color color = tutorialBackGroundImage.color;
            float i = color.a;
            float rate = 1 / 0.25f;
            while (i < alpha)
            {
                i += Time.deltaTime * rate;
                color.a = i;
                tutorialBackGroundImage.color = color;
                yield return null;
            }
        }

        IEnumerator BackGroundOut(Action onDone = null)
        {
            Color color = tutorialBackGroundImage.color;
            float i = color.a;
            float rate = 1 / 0.25f;
            while (i > 0)
            {
                i -= Time.deltaTime * rate;
                color.a = i;
                tutorialBackGroundImage.color = color;
                yield return null;
            }
            onDone?.Invoke();
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       
        public void OnHighlighterClicked()
        {
            _highlightercPointerTap?.Invoke();
        }
        public void OnHighlighterEndDrag()
        {
            _highlightercEndDragAction?.Invoke();
        }
        public void OnHighlighterPointerUp()
        {
            _highlighterMouseUpAction?.Invoke();
        }

        #endregion
    }

    [Serializable]
    public class TutorialHighlighterData
    {
        public TutorialHighliterType tutorialHighliterType;
        public RectTransform mainRectTransform;

        public Image highLighterImage;
        public Button highLighterButton;
    }

    public enum TutorialHighliterType
    {
        None = 0,
        CircleHighlighter = 1,
        FullScreenHighlighter = 2,
    }
}