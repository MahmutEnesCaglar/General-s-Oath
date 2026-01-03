using UnityEngine;
using TowerDefense.Tower;

namespace TowerDefense.Tower
{
    public class CannonTower : Tower
    {
        [Header("Audio")]
        public AudioClip shootSound;
        private AudioSource audioSource;

        protected override void Start()
        {
            base.Start();
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        protected override void Attack()
        {
            base.Attack();

            if (projectilePrefab != null && currentTarget != null)
            {
                if (shootSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(shootSound);
                }
            }
        }
    }
}
