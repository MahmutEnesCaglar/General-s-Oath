using UnityEngine;
using TowerDefense.Enemy;

namespace TowerDefense.Tower
{
    /// <summary>
    /// Kule mermisi - Hedefe doğru hareket eder ve hasar verir
    /// Arkadaşın sistemi + damage desteği eklendi
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [Header("Hareket")]
        public float speed = 12f;
        private GameObject target;
        private int damage;

        /// <summary>
        /// Projectile'ı başlatır (Arkadaşın API'si ile uyumlu)
        /// </summary>
        public void Setup(GameObject _target, int _damage)
        {
            target = _target;
            damage = _damage;
        }

        void Update()
        {
            // Eğer hedef yok olmuşsa mermiyi yok et
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            // Hedefe doğru yönel ve ilerle
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Mermiyi hedefe doğru döndür (opsiyonel - ok gibi görseller için)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Hedefe ulaştı mı? (0.2 birim kala ulaştı sayıyoruz)
            if (Vector3.Distance(transform.position, target.transform.position) < 0.2f)
            {
                HitTarget();
            }
        }

        /// <summary>
        /// Hedefe çarptı - Hasar ver ve mermiyi yok et
        /// </summary>
        private void HitTarget()
        {
            // Düşmana hasar ver
            Enemy.Enemy enemy = target.GetComponent<Enemy.Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"[Projectile] {target.name}'e {damage} hasar verildi!");
            }
            else
            {
                Debug.LogWarning($"[Projectile] {target.name}'de Enemy component bulunamadı!");
            }

            // Mermiyi yok et
            Destroy(gameObject);
        }
    }
}
