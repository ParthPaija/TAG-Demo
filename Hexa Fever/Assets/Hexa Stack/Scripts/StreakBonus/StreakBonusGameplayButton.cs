using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class StreakBonusGameplayButton : MonoBehaviour
    {
        #region PUBLIC_VARS

        [SerializeField] private Text unlockAtText;
        [SerializeField] private GameObject lockGO;
        [SerializeField] private GameObject unlockGO;
        [SerializeField] private GameObject zeroPropellerGO;
        [SerializeField] private Text propellerText;
        [SerializeField] private float cooldownTime = 2f;
        [SerializeField] private string inActive;
        [SerializeField] private string noTargets;
        [SerializeField] private string outOfPropellers;
        [SerializeField] private Propeller propellerPrefab;

        #endregion

        #region PRIVATE_VARS

        private bool canClick;
        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void Init()
        {
            gameObject.SetActive(StreakBonusManager.Instance.StreakBonusData.streakBonusDataConfig.isActive);
            SetUi();
        }


        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetUi()
        {
            unlockAtText.text = $"Lv.{StreakBonusManager.Instance.StreakBonusData.streakBonusDataConfig.openAt}";
            bool isStreakBonusActive = StreakBonusManager.Instance.IsSystemActive();
            bool isPropellerAvailable = StreakBonusManager.Instance.GetCurrentStreakBonusData().propellersCount > 0;
            lockGO.SetActive(!isStreakBonusActive);
            unlockGO.SetActive(isStreakBonusActive && isPropellerAvailable);
            zeroPropellerGO.SetActive(isStreakBonusActive && !isPropellerAvailable);
            if (isStreakBonusActive)
            {
                SetPropellerCount();
            }
            canClick = true;
        }

        private void SetPropellerCount()
        {
            propellerText.text = StreakBonusManager.Instance.CurrentPropellers.ToString();
        }

        private void OnStreakBonusButtonClick()
        {
            int currentPropellers = StreakBonusManager.Instance.CurrentPropellers;
            if (currentPropellers > 0)
            {
                canClick = false;
                UsePropeller(transform.position);
            }
            else
            {
                ShowToastForZeroPropeller();
            }
        }

        private void ShowToastForZeroPropeller()
        {
            if (StreakBonusManager.Instance.GetCurrentStreakBonusData().propellersCount > 0)
            {
                ShowToastMsg(outOfPropellers);
            }
            else
            {
                ShowToastMsg(inActive);
            }

        }
        private void ShowToastMsg(string msg)
        {
            GlobalUIManager.Instance.GetView<ToastMessageView>().ShowMessage(msg);
        }
        private void UsePropeller(Vector3 pos)
        {
            BaseCell targetCell = GetBeseCellForPropellerUse();
            if (targetCell != null)
            {
                //InputManager.StopInteraction = true;
                StreakBonusManager.Instance.UsePropeller();
                OnPropellerUse();

                PropellerCellUnlocker.AddCell(targetCell);

                CameraCache.TryFetchCamera(CameraCacheType.MAINSCENE_UI_CAMERA, out Camera uiCamera);
                Vector3 screenPointCenter = RectTransformUtility.WorldToScreenPoint(uiCamera, pos); // Gets screen pos of pivot

                Vector3 worldPosOnCanvas;
                Vector3 screenPointWithDepth = new Vector3(screenPointCenter.x, screenPointCenter.y, 1);
                worldPosOnCanvas = InputManager.EventCamera.ScreenToWorldPoint(screenPointWithDepth);

                Propeller propeller = Instantiate(propellerPrefab, targetCell.transform);
                propeller.transform.position = worldPosOnCanvas;
                propeller.PlayPropellerAnimation(worldPosOnCanvas, targetCell, (newtarget) =>
                {
                    propeller.gameObject.SetActive(false);
                    Destroy(propeller);
                    if (newtarget != null && newtarget.CanUseBooster())
                    {
                        newtarget.OnBoosterUse(
                            () =>
                            {
                                LevelManager.Instance.LoadedLevel.ArrangeStackItem();
                                GameplayManager.Instance.SaveAllDataOfLevel();
                                OnPropellerComplete();
                            }, true);
                    }
                    else
                    {
                        OnPropellerComplete();
                    }
                    PropellerCellUnlocker.RemoveCell(targetCell);
                });
            }
            else
            {
                OnPropellerNoTargets();
                OnPropellerComplete();
            }
        }
        private BaseCell GetBeseCellForPropellerUse()
        {
            List<BaseCell> cells = LevelManager.Instance.LoadedLevel.BaseCells;
            BaseCell bestCell = null;

            for (int i = 0; i < cells.Count; i++)
            {
                if (!cells[i].CanUseBooster())
                    continue;

                if (bestCell == null)
                {
                    bestCell = cells[i];
                }
                else
                {
                    bestCell = GetBestCellBasedOnPriority(bestCell, cells[i]);
                }
            }
            return bestCell;
        }
        private BaseCell GetBestCellBasedOnPriority(BaseCell one, BaseCell two)
        {
            BaseCell bestCell = null;
            if (one != null && two != null)
            {
                if (one.GetPriority() < two.GetPriority())
                {
                    return one;
                }
                else if (one.GetPriority() > two.GetPriority())
                {
                    return two;
                }
                else
                {
                    int oneUniqueItem = one.HasItem ? one.ItemStack.GetUniqueItemCount() : 1000;
                    int twoUniqueItem = two.HasItem ? two.ItemStack.GetUniqueItemCount() : 1000;

                    if (oneUniqueItem > twoUniqueItem)
                    {
                        bestCell = one;
                    }
                    else if (twoUniqueItem > oneUniqueItem)
                    {
                        bestCell = two;
                    }
                    else
                    {
                        int oneTotoalItem = one.HasItem ? one.ItemStack.GetItems().Count : 1000;
                        int twoTotoalItem = two.HasItem ? two.ItemStack.GetItems().Count : 1000;

                        if (oneTotoalItem <= twoTotoalItem)
                        {
                            bestCell = one;
                        }
                        else
                        {
                            bestCell = two;
                        }
                    }

                    return bestCell;
                }
            }
            return bestCell;
        }
        private void OnPropellerUse()
        {
            SetPropellerCount();
        }
        private void OnPropellerNoTargets()
        {
            ShowToastMsg(noTargets);
        }
        private void OnPropellerComplete()
        {
            canClick = true;
            //InputManager.StopInteraction = true;
        }
        #endregion

        #region CO-ROUTINES

        IEnumerator CooldownCO()
        {
            yield return new WaitForSecondsRealtime(cooldownTime);
            canClick = true;
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void OnButtonClick()
        {
            if (canClick)
            {
                OnStreakBonusButtonClick();
            }
        }

        public void LockButtonClick()
        {
            //ShowToastMsg(inActive);
        }

        public void ZeroPropellerButtonClick()
        {
            ShowToastMsg(inActive);
        }
        #endregion
    }
}
