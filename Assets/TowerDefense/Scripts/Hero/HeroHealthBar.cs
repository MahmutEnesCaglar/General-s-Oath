using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TowerDefense.Hero
{
    /// <summary>
    /// Hero için Screen Space health bar
    /// Ekranın üst ortasında sabit pozisyonda durur (zoom bağımsız)
    /// Health azaldıkça bar sağdan sola doğru kayar (fillAmount sistemi)
    /// Canvas Render Mode: Screen Space - Overlay (zoom'dan etkilenmez)
    /// </summary>
    public class HeroHealthBar : MonoBehaviour
    {
        [Header("UI References")]
        public Image healthFillImage;        // Health bar fill (sağdan sola azalır)
        public TextMeshProUGUI healthText;   // Can yazısı (örn: "150/150")

        [Header("Colors")]
        public Color fullHealthColor = new Color(0f, 1f, 0f);      // Yeşil
        public Color halfHealthColor = new Color(1f, 1f, 0f);      // Sarı
        public Color lowHealthColor = new Color(1f, 0f, 0f);       // Kırmızı

        /// <summary>
        /// Health bar'ı güncelle - Barrier pattern ile aynı
        /// </summary>
        public void UpdateHealthBar(int currentHealth, int maxHealth)
        {
            if (healthFillImage != null)
            {
                // Fill amount (1.0 = full, 0.0 = empty) - sağdan sola kayar
                float fillAmount = Mathf.Clamp01((float)currentHealth / maxHealth);
                healthFillImage.fillAmount = fillAmount;

                // Renk değişimi (can azaldıkça: yeşil -> sarı -> kırmızı)
                if (fillAmount > 0.5f)
                {
                    // %50-100 arası: Yeşil -> Sarı
                    healthFillImage.color = Color.Lerp(halfHealthColor, fullHealthColor, (fillAmount - 0.5f) * 2f);
                }
                else
                {
                    // %0-50 arası: Kırmızı -> Sarı
                    healthFillImage.color = Color.Lerp(lowHealthColor, halfHealthColor, fillAmount * 2f);
                }
            }

            // Can yazısını güncelle
            if (healthText != null)
            {
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

        /// <summary>
        /// Initialize metodu (backward compatibility için)
        /// </summary>
        public void Initialize(Transform hero)
        {
            // Prefab üzerinde Canvas ayarları yapılmış durumda
            // Ek setup gerekmez
        }
    }
}
