using UnityEngine;
using System.Collections;
using TowerDefense.Enemy; // Enemy scriptine ulaşmak için

namespace TowerDefense.Abilities
{
    public class MeteorStrike : MonoBehaviour
    {
        [Header("Hareket Ayarları")]
        public float fallSpeed = 15f;      // Düşüş hızı
        public float startHeight = 10f;    // Hedefin ne kadar yukarısından başlasın?
        public float startOffsetX = 5f;    // Ne kadar sağdan gelsin? (Çapraz düşüş için)

        [Header("Saldırı Ayarları")]
        public int damage = 50;            // Vereceği hasar
        public float explosionRadius = 3f; // Patlama alanı çapı

        private Vector3 targetPosition;    // Düşeceği nokta
        private bool hasHit = false;
        private SpriteRenderer spriteRenderer;

        public void Initialize(Vector3 targetPos)
        {
            targetPosition = targetPos;
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            // Meteoru başlangıç noktasına taşı (Hedefin sağ-yukarısına)
            transform.position = targetPosition + new Vector3(startOffsetX, startHeight, 0);

            // Meteor düşerken en önde görünsün diye sorting order artırıyoruz
            if (spriteRenderer != null) spriteRenderer.sortingOrder = 10000;
        }

        void Update()
        {
            if (hasHit) return;

            // Hedefe doğru hareket et
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, fallSpeed * Time.deltaTime);

            // Hedefe ulaştı mı? (Mesafesi çok azaldıysa)
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                Explode();
            }
        }

        void Explode()
        {
            hasHit = true;

            // 1. Alan Hasarı Ver (AoE)
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(targetPosition, explosionRadius);
            
            foreach (Collider2D col in hitEnemies)
            {
                BaseEnemy enemy = col.GetComponent<BaseEnemy>();

                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }

            Debug.Log("BOOM! Meteor çarptı.");

            // 2. Görsel Efektler (Yoksa geç)
            // Instantiate(explosionPrefab, targetPosition, Quaternion.identity);

            // 3. Meteoru Yok Et
            StartCoroutine(FadeAndDestroy());
        }

        IEnumerator FadeAndDestroy()
        {
            // Rengi yavaşça şeffaflaştır
            float duration = 0.5f;
            float timer = 0f;
            Color initialColor = spriteRenderer.color;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                if (spriteRenderer != null)
                {
                    float alpha = Mathf.Lerp(1f, 0f, timer / duration);
                    spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
                }
                yield return null;
            }

            Destroy(gameObject);
        }

        // Editörde patlama alanını görmek için
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPosition, explosionRadius);
        }
    }
}