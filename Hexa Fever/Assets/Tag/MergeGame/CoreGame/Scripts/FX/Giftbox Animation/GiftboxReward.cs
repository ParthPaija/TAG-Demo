using UnityEngine;
using UnityEngine.UI;

namespace Tag.CoreGame
{
    public class GiftboxReward : MonoBehaviour
    {
        public Image rewardImage;
        public Text rewardCount;
        public Animator rewardMovement;
        public string idleAnimation;

        public ParticleSystem[] ps;

        public void PlayParticle()
        {
            for (int i = 0; i < ps.Length; i++)
            {
                ps[i].gameObject.SetActive(true);
                ps[i].Play();
            }
        }

        public void PlayRewardIdleAnimation()
        {
            rewardMovement.Play(idleAnimation);
        }
        public void ShowView()
        {
            gameObject.SetActive(true);
        }
        public void HideView()
        {
            gameObject.SetActive(false);
        }
    }
}
