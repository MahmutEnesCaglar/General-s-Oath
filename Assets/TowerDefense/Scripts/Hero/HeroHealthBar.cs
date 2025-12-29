using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TowerDefense.Hero
{
    /// <summary>
    /// Hero için Screen Space health bar
    /// Haritanın üst ortasında sabit pozisyonda durur
    /// Slider tipi - yeşil bar sola doğru azalır
    /// </summary>
    public class HeroHealthBar : MonoBehaviour
    {
        [Header("UI References")]
        public Image backgroundImage;        // Arka plan (koyu renk)
        public Image fillImage;              // Health bar fill (yeşil/sarı/kırmızı)
        public TextMeshProUGUI healthText;   // Can yazısı (örn: "150/150")

        [Header("Health Bar Size & Position")]
        public float barWidth = 750f;
        public float barHeight = 30f;
        public Vector3 barPosition = new Vector3(0f, 7.5f, 0f); // Pozisyon (Y:7.5 = üstte)

        [Header("Colors")]
        public Color fullHealthColor = new Color(0f, 1f, 0f);      // Yeşil
        public Color halfHealthColor = new Color(1f, 1f, 0f);      // Sarı
        public Color lowHealthColor = new Color(1f, 0f, 0f);       // Kırmızı
        public Color backgroundColor = new Color(0.2f, 0.2f, 0.2f); // Koyu gri

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            SetupHealthBar();
        }

        /// <summary>
        /// Health bar'ı haritanın üst ortasına konumlandırır ve boyutlandırır
        /// </summary>
        private void SetupHealthBar()
        {
            if (rectTransform != null)
            {
                // Üst orta pozisyon (Anchor: Top Center)
                rectTransform.anchorMin = new Vector2(0.5f, 1f);
                rectTransform.anchorMax = new Vector2(0.5f, 1f);
                rectTransform.pivot = new Vector2(0.5f, 1f);

                // Pozisyon: Kullanıcı tarafından belirlenen pozisyon
                rectTransform.anchoredPosition = new Vector2(barPosition.x, barPosition.y);

                // Boyut
                rectTransform.sizeDelta = new Vector2(barWidth, barHeight);
            }

            // Arka plan rengini ayarla
            if (backgroundImage != null)
            {
                backgroundImage.color = backgroundColor;
            }

            // Fill image ayarları
            if (fillImage != null)
            {
                // Slider tipi: Sol taraftan sağa doğru dolacak
                fillImage.type = Image.Type.Filled;
                fillImage.fillMethod = Image.FillMethod.Horizontal;
                fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
                fillImage.fillAmount = 1f; // Başlangıçta full
                fillImage.color = fullHealthColor;
            }
        }

        /// <summary>
        /// Health bar'ı güncelle
        /// </summary>
        public void UpdateHealthBar(int currentHealth, int maxHealth)
        {
            if (fillImage != null)
            {
                // Fill amount (0-1 arası) - can azaldıkça sola doğru azalır
                float fillAmount = Mathf.Clamp01((float)currentHealth / maxHealth);
                fillImage.fillAmount = fillAmount;

                // Renk değişimi (can azaldıkça: yeşil -> sarı -> kırmızı)
                if (fillAmount > 0.5f)
                {
                    // %50-100 arası: Yeşil -> Sarı
                    fillImage.color = Color.Lerp(halfHealthColor, fullHealthColor, (fillAmount - 0.5f) * 2f);
                }
                else
                {
                    // %0-50 arası: Kırmızı -> Sarı
                    fillImage.color = Color.Lerp(lowHealthColor, halfHealthColor, fillAmount * 2f);
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
        /// Artık hero transform'una ihtiyaç yok çünkü sabit pozisyonda
        /// </summary>
        public void Initialize(Transform hero)
        {
            // Artık hero'yu takip etmiyoruz, sadece setup yapıyoruz
            SetupHealthBar();
        }
    }
}
