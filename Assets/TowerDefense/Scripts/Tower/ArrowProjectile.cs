using UnityEngine;
using TowerDefense.Enemy;

namespace TowerDefense.Tower
{
    public class ArrowProjectile : MonoBehaviour
    {
        public float speed = 15f;
        private Vector3 moveDirection; // Sabit yön
        private int damage;
        private float lifeTime = 3f;

        public void Setup(GameObject _target, int _damage)
        {
            damage = _damage;
            
            if (_target != null)
            {
                // Hedefin o anki pozisyonuna doğru yönü hesapla ve kilitle
                moveDirection = (_target.transform.position - transform.position).normalized;
                
                // Okun ucunu o yöne çevir
                float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Update()
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                Destroy(gameObject);
                return;
            }

            // Sabit yönde ilerle
            transform.position += moveDirection * speed * Time.deltaTime;

            CheckCollision();
        }

        void CheckCollision()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.2f);
            
            foreach (Collider2D hit in hits)
            {
                BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }
}