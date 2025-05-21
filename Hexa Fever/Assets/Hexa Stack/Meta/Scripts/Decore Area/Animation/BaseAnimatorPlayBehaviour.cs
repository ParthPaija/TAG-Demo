using UnityEngine;

namespace Tag.MetaGame.Animation
{
    public abstract class BaseAnimatorPlayBehaviour : MonoBehaviour
    {
        #region PUBLIC_VARS

        public ParticleSystem[] _taskCompletionEffects;

        #endregion

        #region PRIVATE_VARS

        //private SoundHandler SoundHandler { get { return FindObjectFromLibrary<SoundHandler>(); } }

        #endregion

        #region UNITY_CALLBACKS
        #endregion

        #region PUBLIC_FUNCTIONS

        public abstract void OnPlayAnimation();
        public virtual void PlayIdelAnimation()
        {

        }
        public virtual void PlayOnAnimation()
        {

        }
        public virtual void PlayOffAnimation()
        {

        }
        public virtual void PlayParticles()
        {
            //SoundHandler.PlaySound(SoundType.TaskComplete3);
            for (int i = 0; i < _taskCompletionEffects.Length; i++)
            {
                _taskCompletionEffects[i].Play();
            }
        }
        public abstract void SetTaskStatus(bool isActive);
        public abstract void OnStopAnimation();
        public abstract float GetAnimationTime();
        public abstract float GetParticleTime();

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
