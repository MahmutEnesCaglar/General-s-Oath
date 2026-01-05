using UnityEngine;
using TowerDefense.Enemy;

namespace TowerDefense.Tower
{
    /// <summary>
    /// Kule mermisi - Hedefe doğru hareket eder ve hasar verir
    /// GÜNCELLENDİ: Artık güdümlü değil, ilk hedeflenen yöne düz gider.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [Header("Hareket")]
        public float speed = 12f;
        private Vector3 moveDirection; // Sabit yön
        private int damage;
        private float lifeTime = 3f; // Sonsuza kadar gitmesin diye ömür süresi

        /// <summary>
        /// Projectile'ı başlatır
        /// </summary>
        public void Setup(GameObject _target, int _damage)
        {
            damage = _damage;
            
            if (_target != null)
            {
                // Hedefin o anki pozisyonuna doğru yönü hesapla ve kilitle
                moveDirection = (_target.transform.position - transform.position).normalized;
                
                // Görseli o yöne çevir
                float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            else
            {
                // Hedef yoksa mermiyi hemen yok et
                Destroy(gameObject);
            }
        }

        void Update()
        {
            // Ömür kontrolü
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                Destroy(gameObject);
                return;
            }

            // Sabit yönde ilerle (Güdümsüz)
            transform.position += moveDirection * speed * Time.deltaTime;

            // Çarpışma Kontrolü
            CheckCollision();
        }

        private void CheckCollision()
        {
            // Merminin çok yakınında bir düşman var mı?
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.2f);
            
            foreach (Collider2D hit in hits)
            {
                // Sadece düşmanlara çarp
                BaseEnemyRefactored enemy = hit.GetComponent<BaseEnemyRefactored>();
                if (enemy != null)
                {
                    HitTarget(enemy);
                    return; // İlk düşmana çarpınca işlemi bitir
                }
            }
        }

        private void HitTarget(BaseEnemyRefactored enemy)
        {
            if (enemy != null)
            {
                // Tower'dan gelen hasarı enemy'nin damageFromTower değeri ile uygula
                enemy.TakeDamageFromTower(damage);
            }
            // Mermiyi yok et
            Destroy(gameObject);
        }
    }
}
