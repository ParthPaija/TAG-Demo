using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tag.HexaStack
{
    public class VFXManager : Manager<VFXManager>
    {
        #region public veriables

        #endregion

        #region private veriables

        public AnimationCurve animationCurve;
        public AnimationCurve easeInCurve;
        public AnimationCurve easeOutCurve;
        [SerializeField] private GameObject selectedCellObj;
        [SerializeField] private ParticleSystem cellUnlockPs;
        [SerializeField] private ParticleSystem honeyCellUnlockPs;

        #endregion

        #region public methods

        public void PlaySelecedCellAnimation(BaseCell cell)
        {
            selectedCellObj.SetActive(true);
            selectedCellObj.transform.position = cell.transform.position;
            selectedCellObj.transform.localScale = cell.transform.lossyScale;
        }

        public void PlayUnlockCellAnimation(BaseCell cell)
        {
            ParticleSystem particleSystem = Instantiate(cellUnlockPs, transform);
            particleSystem.gameObject.SetActive(true);
            if (cell.HasItem)
                particleSystem.transform.position = cell.ItemStack.GetTopItem().transform.position + new Vector3(0, 0.6f, -0.35f);
            else
                // particleSystem.transform.position = cell.transform.position.With(null, 0.1f, null);
                particleSystem.transform.position = cell.transform.position + new Vector3(0, 0.15f, -0.05f);
            particleSystem.transform.localScale = cell.transform.lossyScale;
            particleSystem.Play();
        }
        public void PlayUnlockHoneyCellAnimation(BaseCell cell)
        {
            if (honeyCellUnlockPs != null)
            {
                ParticleSystem particleSystem = Instantiate(honeyCellUnlockPs, transform);
                particleSystem.gameObject.SetActive(true);
                if (cell.HasItem)
                    particleSystem.transform.position = cell.ItemStack.GetTopItem().transform.position + new Vector3(0, 1.6f, -0.85f);
                else
                    // particleSystem.transform.position = cell.transform.position.With(null, 0.1f, null);
                    particleSystem.transform.position = cell.transform.position + new Vector3(0, 1.15f, -0.55f);
                particleSystem.transform.localScale = cell.transform.lossyScale;
                particleSystem.Play();
            }
        }

        public void StopSelecedCellAnimation()
        {
            selectedCellObj.SetActive(false);
        }

        #endregion

        #region Co-Routine

        #endregion
    }
}