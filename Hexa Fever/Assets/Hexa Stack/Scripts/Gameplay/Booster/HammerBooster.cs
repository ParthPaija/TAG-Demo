using DG.Tweening;
using System;
using UnityEngine;

namespace Tag.HexaStack
{
    public class HammerBooster : BaseBooster
    {
        #region PUBLIC_VARS

        public LayerMask itemLayerMask;

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Animator hammerFx;
        [SerializeField] private string hammerAnimationName;
        private RaycastHit hit;
        private bool inProcess = false;
        public static event Action OnHammerUseEvent;

        #endregion

        #region UNITY_CALLBACKS

        private void Update()
        {
            if (!isActive)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                if (GetRayHit(cameraMain.ScreenToWorldPoint(Input.mousePosition), out hit, itemLayerMask))
                {
                    BaseCell baseCell = hit.collider.GetComponent<BaseCell>();
                    if (baseCell != null)
                    {
                        if (!baseCell.IsCellLocked() && baseCell.HasItem && !inProcess)
                        {
                            OnHammerUse(baseCell);
                        }
                        else if (baseCell.IsCellLocked() && baseCell.BaseCellUnlocker.CanUseBooster() && !inProcess)
                        {
                            OnHammerUse(baseCell);
                        }
                    }
                }
            }
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public override void OnActive(Action onUse)
        {
            ActiveConfirmationView();
            InputManager.StopInteraction = true;
            base.OnActive(onUse);
        }

        public override void OnUse()
        {
            GameplayManager.Instance.SaveAllDataOfLevel();
            DeActiveConfirmationView();
            base.OnUse();
            InputManager.StopInteraction = false;
        }

        public override void OnUnUse()
        {
            base.OnUnUse();
            DeActiveConfirmationView();
            InputManager.StopInteraction = false;
        }

        public bool GetRayHit(Vector3 pos, out RaycastHit hit, LayerMask layerMask)
        {
            return Physics.Raycast(pos, InputManager.eventTranform.forward, out hit, Mathf.Infinity, layerMask);
        }

        public void OnHammerUse(BaseCell baseCell)
        {
            OnHammerUseEvent?.Invoke();
            inProcess = true;
            GameplayManager.Instance.CancelUndoBooster();
            hammerFx.transform.SetParent(baseCell.transform);
            hammerFx.transform.localPosition = baseCell.HasItem ? baseCell.ItemStack.GetTopPosition() : Vector3.zero;
            hammerFx.gameObject.SetActive(true);
            hammerFx.Play(hammerAnimationName);

            CoroutineRunner.Instance.Wait(hammerFx.GetAnimationLength(hammerAnimationName) - 0.3f, () =>
            {
                Vibrator.Vibrate(Vibrator.hugeIntensity);
                hammerFx.transform.SetParent(transform);
                hammerFx.gameObject.SetActive(false);
                baseCell.OnBoosterUse(null);
                OnUse();
                inProcess = false;
            });
            SoundHandler.Instance.PlaySound(SoundType.Hammer);
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
