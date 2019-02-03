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

        public void Pick(int? pickedAmount)
        {
            if (!picked)
            {
                DisplayPickedData(pickedAmount == null || pickedAmount > amount ? amount : (int)pickedAmount);

                picked = true;

                if (audioSource == null)
                    audioSource = GetComponent<AudioSource>();

                audioSource.PlayOneShot(pickupSound);
                pickedParticles.Play();

                Destroy(gameObject, 30f);
            }
        }

        protected virtual void DisplayPickedData(int pickedAmount) { }
    }
}
