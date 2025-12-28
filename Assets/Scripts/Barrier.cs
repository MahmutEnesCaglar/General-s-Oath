using UnityEngine;
using UnityEngine.UI; // Health bar için

namespace TowerDefense.Buildings
{
    public class Barrier : MonoBehaviour
    {
        [Header("Özellikler")]
        public int maxHealth = 500;
        public int currentHealth;

        [Header("Görsel Ayarlar")]
        public SpriteRenderer spriteRenderer;
        
        [Header("UI")]
        public GameObject healthBarObj; // Canvas içindeki bar
        public Image healthFillImage;   // Doluluk oranı resmi

        private void Start()
        {
            currentHealth = maxHealth;
            UpdateHealthBar();
        }

        // Bariyer kurulurken doğru sprite'ı atamak için
        public void Initialize(Sprite selectedSprite)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = selectedSprite;
                
                // Collider boyutunu yeni sprite'a göre güncelle (Opsiyonel)
                // GetComponent<BoxCollider2D>().size = spriteRenderer.bounds.size; 
            }
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            UpdateHealthBar();

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void UpdateHealthBar()
        {
            if (healthFillImage != null)
            {
                float ratio = (float)currentHealth / maxHealth;
                healthFillImage.fillAmount = ratio;
            }
        }

        private void Die()
        {
            // Patlama efekti vs. eklenebilir
            Destroy(gameObject);
        }
    }
}