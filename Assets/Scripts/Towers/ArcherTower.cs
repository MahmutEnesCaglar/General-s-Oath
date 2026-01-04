using TowerDefense.Tower;
using UnityEngine;

namespace TowerDefense.Tower
{
    public class ArcherTower : Tower
    {
        private ArcherRotation archerVisual;

        [Header("Audio")]
        public AudioClip[] shootSounds;
        private AudioSource audioSource;

        protected override void Start()
        {
            base.Start();
            archerVisual = GetComponentInChildren<ArcherRotation>();
            
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        protected override void Update()
        {
            base.Update();
            if (currentTarget != null && archerVisual != null)
            {
                archerVisual.RotateTowards(currentTarget.transform.position);
            }
        }
        protected override void Attack()
        {
            if (projectilePrefab != null && currentTarget != null)
            {
                // ArcherRotation'dan o anki yönün ofsetini al
                Vector2 offset = Vector2.zero;
                if (archerVisual != null)
                {
                    offset = archerVisual.GetCurrentFirePointOffset(archerVisual.currentSegmentIndex);
                }
                
                Vector3 spawnPos = transform.position + (Vector3)offset;

                GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
                
                ArrowProjectile arrow = projObj.GetComponent<ArrowProjectile>();
                if (arrow != null)
                {
                    arrow.Setup(currentTarget, damage);
                }

                if (shootSounds != null && shootSounds.Length > 0 && audioSource != null)
                {
                    int index = Random.Range(0, shootSounds.Length);
                    if (shootSounds[index] != null)
                    {
                        audioSource.PlayOneShot(shootSounds[index]);
                    }
                }
            }
        }
    }
}