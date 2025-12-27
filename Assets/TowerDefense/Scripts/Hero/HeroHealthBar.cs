using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TowerDefense.Hero
{
    /// <summary>
    /// Hero için World Space health bar
    /// Hero'nun üstünde can barı ve can miktarı gösterir
    /// </summary>
    public class HeroHealthBar : MonoBehaviour
    {
        [Header("UI References")]
        public Image fillImage;              // Health bar fill (kırmızı kısım)
        public TextMeshProUGUI healthText;   // Can yazısı (örn: "150/150")

        [Header("Settings")]
        public Vector3 offset = new Vector3(0, 1.5f, 0); // Hero'nun üstünde ne kadar yüksekte olacak
        public Color fullHealthColor = Color.green;
        public Color halfHealthColor = Color.yellow;
        public Color lowHealthColor = Color.red;

        private Transform heroTransform;
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            // Health bar her zaman Hero'yu takip etsin
            if (heroTransform != null)
            {
                transform.position = heroTransform.position + offset;

                // Health bar her zaman kameraya baksın (Billboard effect)
                transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
            }
        }

        /// <summary>
        /// Health bar'ı başlat
        /// </summary>
        public void Initialize(Transform hero)
        {
            heroTransform = hero;
        }

        /// <summary>
        /// Health bar'ı güncelle
        /// </summary>
        public void UpdateHealthBar(int currentHealth, int maxHealth)
        {
            if (fillImage != null)
            {
                // Fill amount (0-1 arası)
                float fillAmount = (float)currentHealth / maxHealth;
                fillImage.fillAmount = fillAmount;

                // Renk değişimi (can azaldıkça kırmızıya döner)
                if (fillAmount > 0.5f)
                {
                    fillImage.color = Color.Lerp(halfHealthColor, fullHealthColor, (fillAmount - 0.5f) * 2f);
                }
                else
                {
                    fillImage.color = Color.Lerp(lowHealthColor, halfHealthColor, fillAmount * 2f);
                }
            }

            if (healthText != null)
            {
                // Can yazısını güncelle
                healthText.text = $"{currentHealth}/{maxHealth}";
            }
        }

        /// <summary>
        /// Health bar'ı göster/gizle
        /// </summary>
        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
