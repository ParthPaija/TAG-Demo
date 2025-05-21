using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Tag.HexaStack
{
    public class HammerBoosterUseStep : BaseTutorialStep
    {
        #region PUBLIC_VARS

        [CurrencyId] public int boosterId;
        public UnityEvent OnStartStep;
        public UnityEvent OnEndStep;
        public Vector3 offset;
        public Vector3 handOffset;
        public Transform objectTransfrom;

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private int cellId;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        [Button]
        public override void OnStartStep1()
        {
            HammerBooster.OnHammerUseEvent += OnTap; 
            base.OnStartStep1();

            if (LevelManager.Instance.LoadedLevel.GetCellById(cellId).HasItem)
            {
                InputManager.StopInteraction = true;
                LevelManager.Instance.LoadedLevel.GridRotator.isRotationActive = false;

                objectTransfrom.position = LevelManager.Instance.LoadedLevel.GetCellById(cellId).transform.position;

                TutorialElementHandler.SetBGForObjectsTurorials(true);
                Debug.LogError("Start Step");
                OnStartStep?.Invoke();
            }
            else
            {
                EndStep();
            }
        }

        public override void EndStep()
        {
            Debug.LogError("EndStep");
            TutorialElementHandler.SetBGForObjectsTurorials(false);
            InputManager.StopInteraction = false;
            LevelManager.Instance.LoadedLevel.GridRotator.isRotationActive = true;
            OnEndStep?.Invoke();
            base.EndStep();
            HammerBooster.OnHammerUseEvent -= OnTap;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void OnTap()
        {
            EndStep();
            //HammerBooster hammerBooster = (HammerBooster)BoosterManager.Instance.GetBooster(boosterId);
            //if (hammerBooster != null)
            //{
            //    hammerBooster.OnHammerUse(LevelManager.Instance.LoadedLevel.GetCellById(cellId));
            //}
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
