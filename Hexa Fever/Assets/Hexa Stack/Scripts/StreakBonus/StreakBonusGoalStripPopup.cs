using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class StreakBonusGoalStripPopup : SerializedMonoBehaviour
    {
        #region PUBLIC_VARS

        [SerializeField] private Text propellerCountText;
        [SerializeField] private Image propellerImage;
        [SerializeField] private List<Text> streakText;
        [SerializeField] private Color normalStreakTextColor;
        [SerializeField] private Color currentStreakTextColor;
        [SerializeField] private GameObject rotator;
        [SerializeField] private Dictionary<int, Sprite> propellerSprites;
        [SerializeField] private Image streakFillBar;
        [SerializeField] private GameObject ps;
        [SerializeField] private AnimationCurve animationCurve;

        [SerializeField] private Animator animator;
        [SerializeField] private String inAnimation;
        [SerializeField] private String outAnimation;

        #endregion

        #region PRIVATE_VARS

        private int oldStreak;
        private int currentStreak;
        private int currentPropellerCount;
        private int oldPropellerCount;
        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void Init()
        {
            Debug.LogError("StreakBonusGoalStripPopup : " + StreakBonusManager.Instance.IsSystemActive());
            if (StreakBonusManager.Instance.IsSystemActive() && !StreakBonusManager.Instance.isPlayLoadedLevel)
            {
                oldStreak = StreakBonusManager.Instance.OldStreak;
                currentStreak = StreakBonusManager.Instance.CurrentStreak;
                currentPropellerCount = StreakBonusManager.Instance.CurrentPropellers;
                oldPropellerCount = /*StreakBonusManager.Instance.isPlayLoadedLevel ? 0 :*/ StreakBonusManager.Instance.GetPropellerCount(oldStreak);
                SetUi();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void Hide()
        {
            ps.SetActive(false);
            StreakBonusManager.Instance.OnGoalStripShown();
            MovePropeller();
            animator.Play(outAnimation);
            CoroutineRunner.Instance.Wait(animator.GetAnimatorClipLength(outAnimation), () =>
            {
                gameObject.SetActive(false);
            });
        }
        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetUi()
        {
            propellerCountText.text = oldPropellerCount.ToString();
            SetPropellerImage(oldStreak);
            SetStreakText(oldStreak, false);
            SetDefaultRotator();
            streakFillBar.fillAmount = currentPropellerCount == 0 ? 0f : oldPropellerCount == currentPropellerCount ? 1f : 0f;
            gameObject.SetActive(true);
            animator.Play(inAnimation);


            CoroutineRunner.Instance.Wait(animator.GetAnimatorClipLength(inAnimation), () =>
           {
               Animate();
           });
        }
        private void SetPropellerImage(int streak)
        {
            streak = Mathf.Clamp(streak, 0, propellerSprites.Count - 1);
            propellerImage.sprite = propellerSprites[streak];
        }
        private void SetStreakText(int streak, bool isAnimate)
        {
            isAnimate = isAnimate && (oldStreak != currentStreak);
            for (int i = 0; i < streakText.Count; i++)
            {
                if (i == streak)
                {
                    streakText[i].color = currentStreakTextColor;
                    if (isAnimate)
                    {
                        streakText[i].transform.DOPunchScale(0.2f * Vector3.one, 0.35f, 1, 1).OnComplete(() =>
                        {
                            streakText[i].transform.localScale = Vector3.one;
                        });
                    }
                }
                else
                    streakText[i].color = normalStreakTextColor;
            }
        }

        private void SetDefaultRotator()
        {
            float value = oldStreak * (-45);
            rotator.transform.localRotation = Quaternion.Euler(0f, 0f, value);
        }
        private void MoveRotator()
        {
            // Calculate the current and target angles
            float startAngle = oldStreak * (-45f);
            float targetAngle = currentStreak * (-45f);
            float duration = 0.5f;

            // Decide rotation direction and calculate the correct target for full round if needed
            float to = targetAngle;
            if (currentStreak > oldStreak)
            {
                // Clockwise: always decrease angle (more negative)
                if (to >= startAngle)
                    to = startAngle - (360f - (to - startAngle));
            }
            else if (currentStreak < oldStreak)
            {
                // Counter-clockwise: always increase angle (less negative)
                if (to <= startAngle)
                    to = startAngle + (360f - (startAngle - to));
            }

            StartCoroutine(RotateFromTo(startAngle, to, duration));

            IEnumerator RotateFromTo(float from, float to, float time)
            {
                float elapsed = 0f;
                while (elapsed < time)
                {
                    float t = animationCurve != null ? animationCurve.Evaluate(elapsed / time) : (elapsed / time);
                    float z = Mathf.LerpUnclamped(from, to, t);
                    rotator.transform.localRotation = Quaternion.Euler(0f, 0f, z);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                rotator.transform.localRotation = Quaternion.Euler(0f, 0f, to);

                //propellerCountText.text = currentPropellerCount.ToString();
                SetPropellerImage(currentStreak);
                SetStreakText(currentStreak, true);

                yield return new WaitForSeconds(0.5f);
                Hide();
            }
        }
        private void Animate()
        {
            if (oldStreak != currentStreak)
            {
                MoveRotator();

                if (oldPropellerCount < currentPropellerCount)
                {
                    StreakIncreaseAnimation();
                }
                else if (oldPropellerCount > currentPropellerCount)
                {
                    StreakDecreaseAnimation();
                }
            }
            else
            {
                ps.SetActive(currentPropellerCount != 0);
                streakFillBar.fillAmount = currentPropellerCount == 0 ? 0f : 1f;
                DOVirtual.DelayedCall(1, Hide);
            }

        }

        private void StreakIncreaseAnimation()
        {
            Debug.LogError("StreakIncreaseAnimation");//-35 10

            ps.SetActive(true);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(propellerCountText.transform.DOLocalMove(new Vector3(-35, 10, 0), 0.25f).SetEase(Ease.OutBack));
            //alpha to 0f
            sequence.Join(propellerCountText.DOFade(0f, 0.25f).SetEase(Ease.OutBack));
            sequence.AppendCallback(() =>
            {
                //set propeller count text to current propeller count
                propellerCountText.text = currentPropellerCount.ToString();

            });
            //set at x=35 y=10
            sequence.Append(propellerCountText.transform.DOLocalMove(new Vector3(35, 10, 0), 0.001f).SetEase(Ease.OutBack));
            sequence.Append(propellerCountText.transform.DOLocalMove(Vector3.zero, 0.25f).SetEase(Ease.OutBack));
            //alpha to 1f
            sequence.Join(propellerCountText.DOFade(1f, 0.25f).SetEase(Ease.OutBack));
            DOVirtual.Float(0f, 1f, 0.33f, (value) =>
            {
                streakFillBar.fillAmount = value;
            }).OnComplete(() =>
            {
                streakFillBar.fillAmount = 1f;
            });
        }

        private void StreakDecreaseAnimation()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(propellerCountText.transform.DOLocalMove(new Vector3(35, 10, 0), 0.25f).SetEase(Ease.OutBack));
            //alpha to 0f
            sequence.Join(propellerCountText.DOFade(0f, 0.25f).SetEase(Ease.OutBack));
            sequence.AppendCallback(() =>
            {
                //set propeller count text to current propeller count
                propellerCountText.text = currentPropellerCount.ToString();

            });
            //set at x=35 y=10
            sequence.Append(propellerCountText.transform.DOLocalMove(new Vector3(-35, 10, 0), 0.001f).SetEase(Ease.OutBack));
            sequence.Append(propellerCountText.transform.DOLocalMove(Vector3.zero, 0.25f).SetEase(Ease.OutBack));
            //alpha to 1f
            sequence.Join(propellerCountText.DOFade(1f, 0.25f).SetEase(Ease.OutBack));
            DOVirtual.Float(1f, 0f, 0.33f, (value) =>
            {
                streakFillBar.fillAmount = value;
            }).OnComplete(() =>
            {
                streakFillBar.fillAmount = 0f;
            });
        }
        private void MovePropeller()
        {
            if (currentPropellerCount > 0)
            {
                MainSceneUIManager.Instance.GetView<VFXView>().ItemAnimation.UIStartAnimation(propellerImage.sprite, propellerImage.transform.position, MainSceneUIManager.Instance.GetView<GameplayBottomView>().GetPropellerPos(), 1, isReverseAnimation: true);
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
