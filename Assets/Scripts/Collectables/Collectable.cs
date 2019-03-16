using UnityEngine;

namespace Assets.Scripts.Collectables
{
    public abstract class Collectable : MonoBehaviour
    {
        public int amount = 0;
        public ParticleSystem pickedParticles;
        public AudioClip pickupSound;
        private AudioSource audioSource;
        public bool picked { get; protected set; }

        public Collectable()
        {
            picked = false;
        }
        private void Start()
        {
            Destroy(gameObject, 120);
        }

        public void Pick(int? pickedAmount)
        {
            if (!picked)
            {
                //GameManager.instance.playerIsPicking = true;
                DisplayPickedData(pickedAmount == null || pickedAmount > amount ? amount : (int)pickedAmount);

                picked = true;

                if (audioSource == null)
                    audioSource = GetComponent<AudioSource>();

                #region picking animation
                //AnimatorClipInfo[] clipInfos = null;
                //float pickingAnimLength = 0f;
                //try
                //{
                //    GameManager.instance.playerAnimator.SetTrigger("pickup");
                //    clipInfos = GameManager.instance.playerAnimator.GetCurrentAnimatorClipInfo(0);
                //}
                //catch (System.Exception e) { Debug.Log(e.Message); }

                //if (clipInfos != null)
                //    pickingAnimLength = clipInfos[0].clip.length;

                //StartCoroutine(PickingYield(pickingAnimLength));
                #endregion

                audioSource.PlayOneShot(pickupSound);
                pickedParticles.Play();

                Destroy(gameObject, 30f);
            }
        }

        protected virtual void DisplayPickedData(int pickedAmount) { }

    }
}
