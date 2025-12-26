using UnityEngine;
using System.Collections.Generic;
using TowerDefense.Enemy;

namespace TowerDefense.Tower
{
    /// <summary>
    /// Kule sistemi - Fizik tabanlı hedefleme ile düşmanları tespit eder
    /// İzometrik menzil desteği ile oval algılama alanı
    /// </summary>
    public class Tower : MonoBehaviour
    {
        [Header("Temel Ayarlar")]
        public string towerName = "Kule";
        public float range = 5f;
        public float fireRate = 1f;
        public int damage = 10;

        [Header("İzometrik Ayar")]
        [Range(0.1f, 1f)]
        public float verticalRangeModifier = 0.5f; // Y eksenini daraltan çarpan (oval menzil için)

        [Header("Hedefleme ve Liste")]
        public List<GameObject> enemiesInRange = new List<GameObject>();
        public GameObject currentTarget;
        protected float fireCooldown = 0f;

        [Header("Mermi Ayarları")]
        public GameObject projectilePrefab;

        protected RotatableTowerSprite rotatableVisual;

        protected virtual void Start()
        {
            rotatableVisual = GetComponentInChildren<RotatableTowerSprite>();

            // CircleCollider2D'yi izometrik menzile göre ayarla
            UpdateColliderScale();
        }

        protected virtual void Update()
        {
            fireCooldown -= Time.deltaTime;

            // 1. Listeyi temizle ve en yakın hedefi seç
            UpdateTarget();

            if (currentTarget != null)
            {
                // 2. Hedefe dön (eğer dönen sprite varsa)
                if (rotatableVisual != null)
                    rotatableVisual.RotateTowards(currentTarget.transform.position);

                // 3. Ateş et
                if (fireCooldown <= 0)
                {
                    Attack();
                    fireCooldown = 1f / fireRate;
                }
            }
        }

        protected virtual void UpdateTarget()
        {
            // Null/yok olan düşmanları listeden temizle
            enemiesInRange.RemoveAll(item => item == null);

            // Hedef yoksa ve listede düşman varsa ilkini seç
            if (currentTarget == null && enemiesInRange.Count > 0)
            {
                currentTarget = GetClosestEnemy();
            }
        }

        /// <summary>
        /// Listedeki düşmanlar arasından izometrik olarak en yakın olanı bulur
        /// </summary>
        private GameObject GetClosestEnemy()
        {
            GameObject bestTarget = null;
            float closestDistance = Mathf.Infinity;

            foreach (GameObject enemy in enemiesInRange)
            {
                if (enemy == null) continue;

                float dist = GetIsometricDistance(transform.position, enemy.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    bestTarget = enemy;
                }
            }
            return bestTarget;
        }

        /// <summary>
        /// İzometrik mesafe hesabı (Oval Menzil)
        /// Y eksenini daraltarak kamera açısına uygun menzil oluşturur
        /// </summary>
        protected float GetIsometricDistance(Vector3 posA, Vector3 posB)
        {
            float diffX = posA.x - posB.x;
            float diffY = (posA.y - posB.y) / verticalRangeModifier;
            return Mathf.Sqrt(diffX * diffX + diffY * diffY);
        }

        /// <summary>
        /// Hedefe saldırır - Projectile fırlatır
        /// </summary>
        protected virtual void Attack()
        {
            if (projectilePrefab != null && currentTarget != null)
            {
                // Fire point offset sistemi (eğer varsa)
                Vector3 spawnPos = transform.position;
                if (rotatableVisual != null)
                {
                    Vector2 offset = rotatableVisual.GetCurrentFirePointOffset(rotatableVisual.currentSegmentIndex);
                    spawnPos = transform.position + (Vector3)offset;
                }

                // Projectile oluştur
                GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
                Projectile projScript = projObj.GetComponent<Projectile>();

                if (projScript != null)
                {
                    // Setup çağır (arkadaşın API'si ile uyumlu)
                    projScript.Setup(currentTarget, damage);
                }
            }
        }

        /// <summary>
        /// Fizik tetikleyicisi - Düşman menzile girdi
        /// NOT: Enemy GameObject'lerinin "Enemy" tag'i olmalı
        /// </summary>
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                if (!enemiesInRange.Contains(other.gameObject))
                {
                    enemiesInRange.Add(other.gameObject);
                    Debug.Log($"[Tower] {other.name} menzile girdi! Toplam düşman: {enemiesInRange.Count}");
                }
            }
        }

        /// <summary>
        /// Fizik tetikleyicisi - Düşman menzilden çıktı
        /// </summary>
        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                enemiesInRange.Remove(other.gameObject);
                if (currentTarget == other.gameObject)
                {
                    currentTarget = null;
                }
                Debug.Log($"[Tower] {other.name} menzilden çıktı!");
            }
        }

        /// <summary>
        /// Collider'ı menzile göre ayarla
        /// NOT: CircleCollider2D'nin IsTrigger = true olmalı
        /// </summary>
        private void UpdateColliderScale()
        {
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col != null)
            {
                col.radius = range;
                col.isTrigger = true; // Trigger olarak ayarla
            }
        }

        /// <summary>
        /// Editor'de oval menzili göster
        /// </summary>
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            // Oval menzili çizmek için matrisi değiştiriyoruz
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, verticalRangeModifier, 1));
            Gizmos.DrawWireSphere(Vector3.zero, range);
            Gizmos.matrix = oldMatrix;
        }
    }
}
