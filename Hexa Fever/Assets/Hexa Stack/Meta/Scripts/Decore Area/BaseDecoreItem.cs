using System;
using UnityEngine;

namespace Tag.MetaGame
{
    public abstract class BaseDecoreItem : MonoBehaviour
    {
        #region PUBLIC_VARS

        public string id;

        #endregion

        #region PRIVATE_VARS

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public abstract void LoadData(string data, Action saveDataAction);
        public abstract void LoadItem();
        public abstract void StartDecor();
        public abstract string GetData();
        public abstract void PlayRevealAnimation();
        public abstract void PlayCompleteParticle();
        public abstract float GetAnimationTime();
        public abstract float GetParticleTime();
        public abstract void HideItem();

        public abstract bool IsItemDecore();

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