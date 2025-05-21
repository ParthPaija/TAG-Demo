using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Tag.HexaStack
{
    public class CurrencyAnimation : MonoBehaviour
    {
        #region PUBLIC_VARIABLES

        [Header("References")] public AnimateObject objectToAnimate;
        [SerializeField] private RectTransform StartRect;
        [SerializeField] private Transform midPos1;
        [SerializeField] private Transform midPos2;
        [SerializeField] private Transform midPos3;

        public Transform endPos;
        [SerializeField] private ParticleSystem feedBackParticles;
        //public TMP_Text textTopBarComponent;

        [Space(2)][Header("Values")] public float speed;
        public float interval;
        public float punchTime;
        [Space(2)][Header("Curves")] public AnimationCurve moveCurve;
        public AnimationCurve scaleCurve;
        public Gradient colorOverLifeTime;
        [Space(2)][Header("Vectors")] public Vector3 startSize;
        public Vector3 endSize;

        [Space(2)]
        [Header("On Place Animation")]
        public bool useOnPlaceAnimation;

        public float speedOnPlace;
        public Vector3 startSizeOnPlace;
        public Vector3 endSizeOnPlace;
        public AnimationCurve scaleCurveOnPlace;

        public bool isLeftDirection = true;
        [Space(2)] public bool isInitOnEnable = true;

        //[SerializeField] private bool canAnimateText = false;
        [SerializeField] private bool canPlaySound = false;

        [SerializeField] SoundType soundToPlayOnPlace;
        #endregion

        #region propertice

        public bool IsAnimationInProgress => animatedObjectList.Count > 0;

        #endregion

        #region PRIVATE_VARIABLES

        private Vector3 scale;
        private Vector3 p1;
        private Vector3 p2;
        private Vector3 p3;
        private Vector3 t1;
        private Vector3 t2;
        private Vector3 tempScale;
        private Vector3 viewportPoint;
        private Image imageItems;
        private int spawnedCurrencyObjectValue = 0;
        private float totalCurrency = 0;
        private float decrementScale;
        private bool isRandomStart = false;
        private List<Action<int, bool>> onObjectAnimationComeplete = new List<Action<int, bool>>();
        private List<Action<int, int, bool>> onItemObjectAnimationComeplete = new List<Action<int, int, bool>>();
        private List<AnimateObject> animatedObjectList = new List<AnimateObject>();

        private int quantity;
        #endregion

        #region UNITY_CALLBACKS

        private void Awake()
        {
            Init();
            onObjectAnimationComeplete = new List<Action<int, bool>>();
            animatedObjectList = new List<AnimateObject>();
        }

        public void Start()
        {
            tempScale = startSize;
        }

        #endregion

        #region PUBLIC_METHODS


        public void StartAnimation(Vector3 startWorldPos, int amount, Sprite rewardSprite, bool isReverseAnimation = false)
        {
            viewportPoint = InputManager.EventCamera.WorldToViewportPoint(startWorldPos);
            if (rewardSprite != null)
                imageItems.sprite = rewardSprite;
            StartRect.anchoredPosition = new Vector3(0, 0, 0);
            StartRect.anchorMax = viewportPoint;
            StartRect.anchorMin = viewportPoint;
            isRandomStart = false;
            Animate(amount, isReverseAnimatation: isReverseAnimation);
        }

        public void StartAnimation(Vector3 startWorldPos, Transform endPosition, int amount, Sprite rewardSprite, bool isReverseAnimation = false, int itemId = 0)
        {
            viewportPoint = InputManager.EventCamera.WorldToViewportPoint(startWorldPos);
            imageItems.sprite = rewardSprite;
            StartRect.anchoredPosition = new Vector3(0, 0, 0);
            StartRect.anchorMax = viewportPoint;
            StartRect.anchorMin = viewportPoint;
            endPos = endPosition;
            isRandomStart = false;
            Animate(amount, isReverseAnimatation: isReverseAnimation, itemId: itemId);
        }

        public void UIStartAnimation(Vector3 anchorPosition, int objects = 2, bool isReverseAnimation = false, string layer = "UI", int sortingOrder = 0, Sprite sprite = null)
        {
            if (sprite != null)
                imageItems.sprite = sprite;
            endPos.position = anchorPosition;
            isRandomStart = true;
            Animate(objects, isReverseAnimation, layer, sortingOrder);
        }

        public void UIStartAnimation(Vector3 anchorPosition, Transform endPos = null, int objects = 2, bool isReverseAnimation = false, string layer = "UI", int sortingOrder = 0)
        {
            if (endPos != null)
                this.endPos = endPos;
            StartRect.position = anchorPosition;
            isRandomStart = true;
            Animate(objects, isReverseAnimation, layer, sortingOrder);
        }
        public void UIStartAnimation(Sprite sprite,Vector3 anchorPosition, Transform endPos = null, int objects = 2, bool isReverseAnimation = false, string layer = "UI", int sortingOrder = 0)
        {
            if (sprite != null)
                imageItems.sprite = sprite;
            if (endPos != null)
                this.endPos = endPos;
            StartRect.position = anchorPosition;
            isRandomStart = true;
            Animate(objects, isReverseAnimation, layer, sortingOrder);
        }

        public void Animate(int tempAmount, bool isReverseAnimatation = false, string layer = "UI", int sortingOrder = 0, int itemId = 0)
        {
            quantity = tempAmount;
            int amount = Mathf.Clamp(tempAmount, 1, 8);
            int[] valueArray = AssignCurrencyToSpawnedObjects(tempAmount, amount);
            Vector3[] temp = new Vector3[amount];
            Vector3[] offsetTemp = new Vector3[amount];
            AnimateObject[] animatedObjects = new AnimateObject[amount];

            for (int i = 0; i < amount; i++)
            {
                animatedObjects[i] = objectToAnimate.Spawn(transform, StartRect.position);
                animatedObjects[i].SetSortingOrder(layer, sortingOrder);
                animatedObjects[i].gameObject.SetActive(false);
                animatedObjects[i].SetDetails(valueArray[i]);
                offsetTemp[i] = isLeftDirection ? (StartRect.position + new Vector3(Random.Range(0, -1f), Random.Range(0, 1f), 0f)) : (StartRect.position /*+ new Vector3(Random.Range(0.5f, -2f), -Random.Range(0.0f, 2f), 0)*/);
            }
            if (!isReverseAnimatation && isLeftDirection)
                StartCoroutine(AnimateThroughLoop(animatedObjects, amount, temp, offsetTemp, isReverseAnimatation, itemId));
            else if (!isReverseAnimatation && !isLeftDirection)
                StartCoroutine(AnimateTemp(animatedObjects, amount, temp, offsetTemp, isReverseAnimatation, itemId));
            else
                StartCoroutine(AnimateThroughLoopReverse(animatedObjects, amount, temp, StartRect.position, isReverseAnimatation, itemId));
        }

        public void SetScaleWithCamera()
        {
            decrementScale = ((Camera.main.orthographicSize - 2.7f) * 0.4f) / 2.6f;
            startSize.x = tempScale.x - decrementScale;
            startSize.y = tempScale.y - decrementScale;
            startSize.z = tempScale.z - decrementScale;
        }

        public int[] AssignCurrencyToSpawnedObjects(int value, int count)
        {
            int[] temp = new int[count];
            int counter = 0;
            spawnedCurrencyObjectValue = value / count;
            for (int i = 0; i < temp.Length - 1; i++)
            {
                temp[i] = spawnedCurrencyObjectValue;
                counter += spawnedCurrencyObjectValue;
            }

            temp[temp.Length - 1] = value - counter;
            return temp;
        }


        [ContextMenu("Mid 2 As Self")]
        public void AssignMidTwoAsSelf()
        {
            //midPos2 = midPos;
        }

        #endregion

        #region PRIVATE_METHODS
        public void RegisterObjectAnimationComplete(Action<int, bool> action)
        {
            if (!onObjectAnimationComeplete.Contains(action))
                onObjectAnimationComeplete.Add(action);
        }

        public void DeregisterObjectAnimationComplete(Action<int, bool> action)
        {
            if (onObjectAnimationComeplete.Contains(action))
                onObjectAnimationComeplete.Remove(action);
        }

        private void OnObjectAnimationComplete(int size, bool isLastObject)
        {
            for (int i = 0; i < onObjectAnimationComeplete.Count; i++)
            {
                onObjectAnimationComeplete[i]?.Invoke(size, isLastObject);
            }
        }

        public void RegisterObjectAnimationComplete(Action<int, int, bool> action)
        {
            if (!onItemObjectAnimationComeplete.Contains(action))
                onItemObjectAnimationComeplete.Add(action);
        }

        public void DeregisterObjectAnimationComplete(Action<int, int, bool> action)
        {
            if (onItemObjectAnimationComeplete.Contains(action))
                onItemObjectAnimationComeplete.Remove(action);
        }

        private void OnObjectAnimationComplete(int itemId, int size, bool isLastObject)
        {
            for (int i = 0; i < onItemObjectAnimationComeplete.Count; i++)
            {
                onItemObjectAnimationComeplete[i]?.Invoke(itemId, size, isLastObject);
            }
        }

        private void Init()
        {
            //if (midPos2 == null)
            //{
            //    midPos2 = midPos;
            //}
            imageItems = objectToAnimate.Image;
        }

        private void PlayPlaceSoundClip()
        {
            if (soundToPlayOnPlace != SoundType.None)
                SoundHandler.Instance.PlaySound(soundToPlayOnPlace);
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region CO-ROUTINES

        public IEnumerator AnimateThroughLoop(AnimateObject[] animateObjects, int amount, Vector3[] temp, Vector3[] offset, bool isReverseAnimatation = false, int itemId = 0)
        {
            animatedObjectList.AddRange(animateObjects);
            for (int i = 0; i < amount; i++)
            {
                temp[i] = quantity % 2 == 0 ? midPos1.position : midPos2.position;
                animateObjects[i].gameObject.SetActive(true);
                StartCoroutine(PunchPositionLerp(animateObjects[i], i, (i == amount - 1), temp, offset, StartRect.position, isReverseAnimatation));

                yield return new WaitForSeconds(0.05f);
            }
            //yield return new WaitForSeconds(0.25f);
            StartCoroutine(AnimateTemp(animateObjects, amount, temp, offset, isReverseAnimatation, itemId));
        }

        public IEnumerator AnimateTemp(AnimateObject[] animateObjects, int amount, Vector3[] temp, Vector3[] offset, bool isReverseAnimatation = false, int itemId = 0)
        {
            animatedObjectList.AddRange(animateObjects);
            for (int i = 0; i < amount; i++)
            {
                temp[i] = quantity % 2 == 0 ? midPos1.position : midPos2.position;
                animateObjects[i].gameObject.SetActive(true);
                StartCoroutine(AnimateLerp(animateObjects[i], i == 0, (i == amount - 1), temp[i], offset[i], isReverseAnimatation, itemId));
                yield return new WaitForSeconds(interval);
            }
        }

        public IEnumerator AnimateThroughLoopReverse(AnimateObject[] animateObjects, int amount, Vector3[] temp, Vector3 startPosition, bool isReverseAnimatation = false, int itemId = 0)
        {
            animatedObjectList.AddRange(animateObjects);
            for (int i = 0; i < amount; i++)
            {
                temp[i] = midPos2.position;
                animateObjects[i].gameObject.SetActive(true);
                StartCoroutine(AnimateLerpReverse(animateObjects[i], i == 0, (i == amount - 1), temp[i], startPosition, isReverseAnimatation, itemId));

                yield return new WaitForSeconds(interval);
            }
        }

        private IEnumerator PunchPositionLerp(AnimateObject animateObject, int count, bool isLastObject, Vector3[] temp, Vector3[] offset, Vector3 startTransformPos, bool isReverseAnimation = false)
        {
            if (animateObject == null)
                yield break;

            int tempIndex = Random.Range(0, temp.Length);

            // offset[count] = isLeftDirection ? (startTransformPos + new Vector3(-1f, -1f, 0f)) : (startTransformPos + new Vector3(0.5f, -0.5f, 0));
            AnimationCurve easeOutCurve = VFXManager.Instance.easeOutCurve;
            float i = 0;
            while (i < 1)
            {
                if (animateObject == null)
                    yield break;
                i += (Time.deltaTime * (1 / punchTime));
                animateObject.transform.position = Vector3.LerpUnclamped(false ? (startTransformPos + (temp[tempIndex] / 4)) : (startTransformPos), offset[count], easeOutCurve.Evaluate(isReverseAnimation ? (1 - i) : i));
                animateObject.transform.localScale = Vector3.LerpUnclamped(startSize, endSize * 0.6f, easeOutCurve.Evaluate(isReverseAnimation ? (1 - i) : i));
                yield return 0;
            }
        }

        private IEnumerator AnimateLerp(AnimateObject animateObject, bool isFirstObject, bool isLastObject, Vector3 mid, Vector3 start, bool isReverseAnimatation = false, int itemId = 0)
        {
            float i = 0;
            float rate = (Vector3.Distance(start, endPos.position) /*+ Vector3.Distance(midPos.position, midPos2.position) + Vector3.Distance(midPos2.position, endPos.position)*/) / (speed * 1);
            // offset = isLeftDirection ? (startTransformPos + new Vector3(-1f, -1f, 0f)) : (startTransformPos + new Vector3(0.5f, -0.5f, 0));
            i = 0;
            while (i < 1)
            {
                i += (Time.deltaTime / rate);
                float lerp = isReverseAnimatation ? (1 - i) : i;
                p1 = Vector3.LerpUnclamped(start, mid, moveCurve.Evaluate(lerp));
                //p2 = Vector3.LerpUnclamped(temp, midPos3.position, moveCurve.Evaluate(lerp));
                p3 = Vector3.LerpUnclamped(mid, endPos.position, moveCurve.Evaluate(lerp));
                //t1 = Vector3.LerpUnclamped(p1, p2, moveCurve.Evaluate(i));
                t2 = Vector3.LerpUnclamped(p1, p3, moveCurve.Evaluate(lerp));
                //animateObject.transform.position = Vector3.LerpUnclamped(t1, t2, moveCurve.Evaluate(i));
                animateObject.transform.position = t2;
                animateObject.transform.localScale = Vector3.LerpUnclamped(endSize * 0.6f, endSize, scaleCurve.Evaluate(lerp));

                yield return 0;
            }

            if (useOnPlaceAnimation && isLastObject)
                StartCoroutine(AnimateOnPlaceScale());
            animatedObjectList.Remove(animateObject);
            if (itemId != 0)
                OnObjectAnimationComplete(itemId, animateObject.CurrencyItemPoints, isLastObject);
            else
                OnObjectAnimationComplete(animateObject.CurrencyItemPoints, isLastObject);
            animateObject.Recycle();
            PlayPlaceSoundClip();
        }

        private IEnumerator AnimateLerpReverse(AnimateObject animateObject, bool isFirstObject, bool isLastObject, Vector3 midPoint, Vector3 start, bool isReverseAnimatation = false, int itemId = 0)
        {
            float i = 0;
            float rate = (Vector3.Distance(start, endPos.position) /*+ Vector3.Distance(midPos.position, midPos2.position) + Vector3.Distance(midPos2.position, endPos.position)*/) / (speed * 1);
            // offset = isLeftDirection ? (startTransformPos + new Vector3(-1f, -1f, 0f)) : (startTransformPos + new Vector3(0.5f, -0.5f, 0));
            i = 0;
            while (i < 1)
            {
                i += (Time.deltaTime / rate);
                p1 = Vector3.LerpUnclamped(start, midPoint, moveCurve.Evaluate(i));
                //p2 = Vector3.LerpUnclamped(temp, midPos3.position, moveCurve.Evaluate(lerp));
                p3 = Vector3.LerpUnclamped(midPoint, endPos.position, moveCurve.Evaluate(i));
                //t1 = Vector3.LerpUnclamped(p1, p2, moveCurve.Evaluate(i));
                t2 = Vector3.LerpUnclamped(p1, p3, moveCurve.Evaluate(i));
                //animateObject.transform.position = Vector3.LerpUnclamped(t1, t2, moveCurve.Evaluate(i));
                animateObject.transform.position = t2;
                animateObject.transform.localScale = Vector3.LerpUnclamped(endSize * 0.6f, endSize, scaleCurve.Evaluate(i));

                yield return 0;
            }

            if (useOnPlaceAnimation && isLastObject)
                StartCoroutine(AnimateOnPlaceScale());
            animatedObjectList.Remove(animateObject);
            if (itemId != 0)
                OnObjectAnimationComplete(itemId, animateObject.CurrencyItemPoints, isLastObject);
            else
                OnObjectAnimationComplete(animateObject.CurrencyItemPoints, isLastObject);
            animateObject.Recycle();
            PlayPlaceSoundClip();
        }

        private IEnumerator AnimateOnPlaceScale()
        {
            if (feedBackParticles != null)
            {
                feedBackParticles.transform.position = endPos.position;
                feedBackParticles.Stop();
                feedBackParticles.Play();
            }

            float i = 0;
            float rate = 1 / speedOnPlace;
            while (i < 1)
            {
                i += rate * Time.deltaTime;
                endPos.transform.localScale = Vector3.LerpUnclamped(startSizeOnPlace, endSizeOnPlace, scaleCurveOnPlace.Evaluate(i));
                yield return 0;
            }

            i = 1;
            endPos.transform.localScale = Vector3.LerpUnclamped(startSizeOnPlace, endSizeOnPlace, scaleCurveOnPlace.Evaluate(i));
        }

        #endregion
    }
}