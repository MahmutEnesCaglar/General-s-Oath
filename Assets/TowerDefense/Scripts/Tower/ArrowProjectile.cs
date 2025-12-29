using UnityEngine;
using TowerDefense.Enemy; // Enemy scriptine erişim için

namespace TowerDefense.Tower
{
    public class ArrowProjectile : MonoBehaviour
    {
        public float speed = 15f;
        private GameObject target;
        private int damage; // EKLENDİ: Hasar miktarı

        public void Setup(GameObject _target, int _damage)
        {
            target = _target;
            damage = _damage; // EKLENDİ: Hasarı hafızaya al
        }

        void Update()
        {
            // Hedef öldüyse oku yok et
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            // 1. Hedefe Doğru Hareket
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // 2. Okun Ucunu Hedefe Çevir
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // 3. Mesafe Kontrolü (Vurma Anı)
            if (Vector3.Distance(transform.position, target.transform.position) < 0.2f)
            {
                HitTarget();
            }
        }

        void HitTarget()
        {
            if (target != null)
            {
                BaseEnemy enemyScript = target.GetComponent<BaseEnemy>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(damage);
                }
            }

            Destroy(gameObject);
        }
    }
}