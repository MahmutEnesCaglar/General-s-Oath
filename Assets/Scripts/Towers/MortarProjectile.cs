using UnityEngine;
using TowerDefense.Enemy;

namespace TowerDefense.Tower
{
    public class MortarProjectile : MonoBehaviour
    {
        private Vector3 startPosition;
        private Vector3 targetPosition;
        private int damage;
        private float explosionRadius; // ARTIK PRIVATE (Kule'den gelecek)
        
        [Header("Uçuş Ayarları")]
        public float duration = 1.0f; 
        public float arcHeight = 2.0f;  
        
        [Header("Efekt")]
        public GameObject explosionEffectPrefab; 

        private float elapsedTime = 0f;

        // YENİ SETUP: Artık _radius (yarıçap) da istiyor
        public void Setup(GameObject _target, int _damage, float _radius)
        {
            startPosition = transform.position;
            damage = _damage;
            explosionRadius = _radius; // Kule'den gelen değeri hafızaya al

            if (_target != null)
            {
                targetPosition = _target.transform.position;
            }
            else
            {
                targetPosition = transform.position; 
                Destroy(gameObject);
            }
            
            elapsedTime = 0f;
        }

        void Update()
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration; 

            if (progress <= 1.0f)
            {
                Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, progress);
                float arc = Mathf.Sin(progress * Mathf.PI) * arcHeight;
                transform.position = new Vector3(currentPos.x, currentPos.y + arc, currentPos.z);
            }
            else
            {
                Explode();
            }
        }

        void Explode()
        {
            // Burada artık kuleden gelen "explosionRadius" değerini kullanıyor
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

            foreach (Collider2D hitCol in hitColliders)
            {
                if (hitCol.CompareTag("Enemy"))
                {
                    BaseEnemyRefactored enemyScript = hitCol.GetComponent<BaseEnemyRefactored>();
                    if (enemyScript != null)
                    {
                        // Tower'dan gelen hasarı enemy'nin damageFromTower değeri ile uygula
                        enemyScript.TakeDamageFromTower(damage);
                    }
                }
            }

            if (explosionEffectPrefab != null)
            {
                Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            // Gizmo'da varsayılan bir değer gösteriyoruz hata vermesin diye
            Gizmos.DrawWireSphere(transform.position, explosionRadius > 0 ? explosionRadius : 1.5f);
        }
    }
}