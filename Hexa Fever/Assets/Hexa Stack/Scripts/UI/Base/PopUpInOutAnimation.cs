using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class PopUpInOutAnimation : BaseUiAnimation
    {
        #region PRIVATE_VARS

        [SerializeField] private PopUpAnimationDataSO _popUpAnimationDataSO;
        [SerializeField] private Transform[] _objects;
        [SerializeField] private List<Transform> _objectsToAnimaateAfter;
        [SerializeField] private CanvasGroup _popUpCanvasGroup;
        [SerializeField] private Image _blackBG;
        [SerializeField] private List<ScrollRect> _scrolls;
        [SerializeField] private List<Vector2> _scrollNormalizedPositions;
        #endregion

        #region PUBLIC_FUNCTIONS

        private void Start()
        {
            GetNormalizedPosition();
        }
        public void OnHideViewWithoutFX(Action OnHideAnimationComplete = null)
        {
            for (int i = 0; i < _objects.Length; i++)
            {
                _objects[i].gameObject.SetActive(false);
            }
            OnHideAnimationComplete?.Invoke();
        }

        [Button("Set Scrolls")]
        public void GetScrollViews()
        {
            //Scale Animation na lidhe scroll hali jatu tu etle aavu karyu che!! delete naa marva namra vinanti!
            if (_scrolls != null)
                _scrolls.Clear();
            _scrolls = new List<ScrollRect>();
            _scrolls = GetComponentsInChildren<ScrollRect>().ToList();
            _scrollNormalizedPositions = new List<Vector2>();

            GetNormalizedPosition();
        }

        private void GetNormalizedPosition()
        {
            //_scrollNormalizedPositions.Clear();
            for (int i = 0; i < _scrolls.Count; i++)
            {
                _scrollNormalizedPositions.Add(_scrolls[i].normalizedPosition);

            }
        }

        private void SetScrollToNormalPosition()
        {
            for (int i = 0; i < _scrolls.Count; i++)
            {
                _scrolls[i].normalizedPosition = _scrollNormalizedPositions[i];
            }
        }

        public override void AddToObjectAfterAnimate(Transform transform)
        {
            _objectsToAnimaateAfter.RemoveAll(x => x == null);
            _objectsToAnimaateAfter.Add(transform);
        }

        public override void ClearObjectAfterAnimatList()
        {
            _objectsToAnimaateAfter.Clear();
            _objectsToAnimaateAfter = new List<Transform>();
        }
        public override void AddNewObjectToObjectAfterAnimate(Transform transform)
        {
            if (_objectsToAnimaateAfter.Contains(transform))
            {
                _objectsToAnimaateAfter.Remove(transform);
                _objectsToAnimaateAfter.RemoveAll(x => x == null);
                _objectsToAnimaateAfter.Add(transform);
            }
        }

        public override void ShowAnimation(Action OnShowAnimationComplete = null)
        {
            _objectsToAnimaateAfter.RemoveAll(x => x == null);
            for (int i = 0; i < _objectsToAnimaateAfter.Count; i++)
            {
                _objectsToAnimaateAfter[i].transform.localScale = Vector3.zero;
            }
            StartCoroutine(OnShowAnimation(() =>
            {
                SetScrollToNormalPosition();
                OnShowAnimationComplete?.Invoke();
            }));
        }

        public void InitPopup()
        {
            if (_blackBG != null)
                _blackBG.color = _popUpAnimationDataSO.bgColor.Evaluate(0);
            _popUpCanvasGroup.alpha = Mathf.LerpUnclamped(_popUpAnimationDataSO.canvasAlpha.x, _popUpAnimationDataSO.canvasAlpha.y, Mathf.Clamp(0 * _popUpAnimationDataSO.canvasAlphaFadeSpeed, 0, 1));
            for (int i = 0; i < _objects.Length; i++)
            {
                Vector3 tempVec;
                tempVec = _objects[i].localScale;
                tempVec.x = Mathf.LerpUnclamped(_popUpAnimationDataSO.startScale.x, _popUpAnimationDataSO.endScale.x, _popUpAnimationDataSO.xInCurve.Evaluate(0));
                tempVec.y = Mathf.LerpUnclamped(_popUpAnimationDataSO.startScale.y, _popUpAnimationDataSO.endScale.y, _popUpAnimationDataSO.yInCurve.Evaluate(0));
                _objects[i].localScale = tempVec;
            }
        }

        public override void HideAnimation(Action OnHideAnimationComplete = null)
        {
            StartCoroutine(OnHideAnimation(OnHideAnimationComplete));
        }

        #endregion

        #region CO-ROUTINES

        private IEnumerator OnShowAnimation(Action OnShowAnimationComplete = null)
        {
            StartCoroutine(BgInAnimation());
            StartCoroutine(DoShowFx(OnShowAnimationComplete));
            yield return null;
        }

        private IEnumerator OnHideAnimation(Action OnHideAnimationComplete = null)
        {
            StartCoroutine(DoHideFx(OnHideAnimationComplete));
            yield return null;
        }

        private IEnumerator DoShowFx(Action onShowComplete)
        {
            _popUpCanvasGroup.interactable = false;
            for (int i = 0; i < _objects.Length; i++)
            {
                _objects[i].gameObject.SetActive(true);
                StartCoroutine(LerpObject(_objects[i], _popUpAnimationDataSO.startScale, _popUpAnimationDataSO.endScale, _popUpAnimationDataSO.xInCurve, _popUpAnimationDataSO.yInCurve, _popUpAnimationDataSO.speed.x));
                yield return new WaitForSecondsRealtime(_popUpAnimationDataSO.interval.x);
            }
            yield return 0;
            _popUpCanvasGroup.interactable = true;
            onShowComplete?.Invoke();
            for (int i = 0; i < _objectsToAnimaateAfter.Count; i++)
            {
                _objectsToAnimaateAfter[i].gameObject.SetActive(true);
                StartCoroutine(LerpObject(_objectsToAnimaateAfter[i], _popUpAnimationDataSO.startScale, _popUpAnimationDataSO.endScale, _popUpAnimationDataSO.xInCurve, _popUpAnimationDataSO.yInCurve, _popUpAnimationDataSO.speed.x));
                yield return new WaitForSecondsRealtime(_popUpAnimationDataSO.interval.x);
            }

        }

        private IEnumerator DoHideFx(Action onHideComplete)
        {
            _popUpCanvasGroup.interactable = false;

            for (int i = _objects.Length - 1; i >= 0; i--)
            {
                StartCoroutine(LerpObject(_objects[i], _popUpAnimationDataSO.endScale, _popUpAnimationDataSO.startScale, _popUpAnimationDataSO.xOutCurve, _popUpAnimationDataSO.yOutCurve, _popUpAnimationDataSO.speed.y));
                if (i == 0)
                {
                    StartCoroutine(BgOutAnimation());
                    yield return new WaitForSecondsRealtime(_popUpAnimationDataSO.interval.y + _popUpAnimationDataSO.speed.y);
                }
                else
                {
                    yield return new WaitForSecondsRealtime(_popUpAnimationDataSO.interval.y);
                }
            }
            onHideComplete?.Invoke();
            yield return 0;
        }

        private IEnumerator LerpObject(Transform transformObject, Vector3 startPos, Vector3 endPos, AnimationCurve xCurve, AnimationCurve yCurve, float runSpeed)
        {
            float i = 0;
            float rate = 1 / runSpeed;
            Vector3 tempVec;
            while (i < 1)
            {
                tempVec = transformObject.localScale;
                i += rate * Time.unscaledDeltaTime;
                tempVec.x = Mathf.LerpUnclamped(startPos.x, endPos.x, xCurve.Evaluate(i));
                tempVec.y = Mathf.LerpUnclamped(startPos.y, endPos.y, yCurve.Evaluate(i));
                transformObject.localScale = tempVec;
                yield return 0;
            }

            tempVec.x = Mathf.LerpUnclamped(startPos.x, endPos.x, xCurve.Evaluate(1));
            tempVec.y = Mathf.LerpUnclamped(startPos.y, endPos.y, yCurve.Evaluate(1));
            yield return 0;
        }

        private IEnumerator BgInAnimation()
        {
            float i = 0;
            float rate = 1 / _popUpAnimationDataSO.bgSpeed.x;
            while (i < 1)
            {
                i += rate * Time.unscaledDeltaTime;
                if (_blackBG != null)
                    _blackBG.color = _popUpAnimationDataSO.bgColor.Evaluate(i);
                _popUpCanvasGroup.alpha = Mathf.LerpUnclamped(_popUpAnimationDataSO.canvasAlpha.x, _popUpAnimationDataSO.canvasAlpha.y, Mathf.Clamp(i * _popUpAnimationDataSO.canvasAlphaFadeSpeed, 0, 1));
                yield return 0;
            }

            i = 1;
            if (_blackBG != null)
                _blackBG.color = _popUpAnimationDataSO.bgColor.Evaluate(i);
            _popUpCanvasGroup.alpha = Mathf.LerpUnclamped(_popUpAnimationDataSO.canvasAlpha.x, _popUpAnimationDataSO.canvasAlpha.y, Mathf.Clamp(i * _popUpAnimationDataSO.canvasAlphaFadeSpeed, 0, 1));

            yield return 0;
        }

        private IEnumerator BgOutAnimation()
        {
            float i = 0;
            float rate = 1 / _popUpAnimationDataSO.bgSpeed.y;
            while (i < 1)
            {
                i += rate * Time.unscaledDeltaTime;
                if (_blackBG != null)
                    _blackBG.color = _popUpAnimationDataSO.bgColor.Evaluate(1 - i);
                _popUpCanvasGroup.alpha = Mathf.LerpUnclamped(_popUpAnimationDataSO.canvasAlpha.y, _popUpAnimationDataSO.canvasAlpha.x, Mathf.Clamp(i * _popUpAnimationDataSO.canvasAlphaFadeSpeed, 0, 1));
                yield return 0;
            }

            i = 1;
            if (_blackBG != null)
                _blackBG.color = _popUpAnimationDataSO.bgColor.Evaluate(1 - i);
            _popUpCanvasGroup.alpha = Mathf.LerpUnclamped(_popUpAnimationDataSO.canvasAlpha.y, _popUpAnimationDataSO.canvasAlpha.x, Mathf.Clamp(i * _popUpAnimationDataSO.canvasAlphaFadeSpeed, 0, 1));
            yield return 0;
        }


//# if UNITY_EDITOR
//        public void SetRef(GameObject viewObject, PopUpAnimation popUpAnimation)
//        {
//            _popUpAnimationDataSO = AssetDatabase.LoadAssetAtPath<PopUpAnimationDataSO>("Assets/MainGame/Scripts/VFX/Scriptable/PopUpAnimationDataSO.asset");
//            if (!viewObject.GetComponent<CanvasGroup>())
//            {
//                CanvasGroup tempCanvas = viewObject.AddComponent<CanvasGroup>();
//            }
//            _popUpCanvasGroup = viewObject.GetComponent<CanvasGroup>();
//            if (_blackBG == null)
//                _blackBG = transform.GetChild(0).GetComponent<Image>();
//            gameObject.GetComponent<BaseView>().viewAnimation = this;
//            assinfg();
//            DestroyImmediate(popUpAnimation);
//            GetScrollViews();
//        }

//        public void SetRef(GameObject viewObject, PopUpCharacterAnimation popUpAnimation)
//        {
//            _popUpAnimationDataSO = AssetDatabase.LoadAssetAtPath<PopUpAnimationDataSO>("Assets/MainGame/Scripts/VFX/Scriptable/PopUpAnimationDataSO.asset");
//            if (!viewObject.GetComponent<CanvasGroup>())
//            {
//                CanvasGroup tempCanvas = viewObject.AddComponent<CanvasGroup>();
//            }
//            _popUpCanvasGroup = viewObject.GetComponent<CanvasGroup>();
//            if (_blackBG == null)
//                _blackBG = transform.GetChild(0).GetComponent<Image>();
//            gameObject.GetComponent<BaseView>().viewAnimation = this;
//            assinfg();
//            DestroyImmediate(popUpAnimation);
//            GetScrollViews();
//        }

//        [Button]
//        public void assinfg()
//        {
//            List<Transform> temp = new List<Transform>();
//            //temp.AddRange(_objects);
//            Transform tempRect = _popUpCanvasGroup.GetComponent<Transform>();
//            if (!temp.Contains(tempRect))
//                temp.Add(tempRect);
//            _objects = new Transform[temp.Count];
//            for (int i = 0; i < temp.Count; i++)
//            {
//                _objects[i] = temp[i];
//            }

//        }
//#endif
        #endregion

    }
}