using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TowerDefense.UI
{
    /// <summary>
    /// Boss için Screen Space health bar.
    /// HeroHealthBar ile benzer çalışır ama Boss'a özel ayarlar içerebilir.
    /// </summary>
    public class BossHealthBar : MonoBehaviour
    {
        [Header("UI References")]
        public Image backgroundImage;        // Arka plan
        public Image fillImage;              // Doluluk barı
        public TextMeshProUGUI bossNameText; // Boss ismi (Opsiyonel)
        public TextMeshProUGUI healthText;   // Can değeri (Opsiyonel)

        [Header("Health Bar Size & Position")]
        public float barWidth = 750f;
        public float barHeight = 30f;
        public Vector3 barPosition = new Vector3(0f, -100f, 0f); // Boss barı biraz daha aşağıda olsun (Hero barı ile çakışmasın)

        [Header("Colors")]
        public Color fullHealthColor = new Color(0.8f, 0f, 0f);    // Koyu Kırmızı
        public Color lowHealthColor = new Color(0.4f, 0f, 0f);     // Daha koyu kırmızı
        public Color backgroundColor = new Color(0.2f, 0.2f, 0.2f); // Koyu gri

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            SetupHealthBar();
        }

        private void SetupHealthBar()
        {
            if (rectTransform != null)
            {
                // Üst orta pozisyon (Anchor: Top Center)
                rectTransform.anchorMin = new Vector2(0.5f, 1f);
                rectTransform.anchorMax = new Vector2(0.5f, 1f);
                rectTransform.pivot = new Vector2(0.5f, 1f);

                // Pozisyon
                rectTransform.anchoredPosition = new Vector2(barPosition.x, barPosition.y);

                // Boyut
                rectTransform.sizeDelta = new Vector2(barWidth, barHeight);
            }

            // Arka plan rengini ayarla
            if (backgroundImage != null)
            {
                backgroundImage.color = backgroundColor;
            }

            // Başlangıç ayarları
            if (fillImage != null)
            {
                fillImage.type = Image.Type.Filled;
                fillImage.fillMethod = Image.FillMethod.Horizontal;
                fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
                fillImage.color = fullHealthColor;
            }
        }

        public void Initialize(string bossName, int maxHealth)
        {
            if (bossNameText != null)
                bossNameText.text = bossName;
            
            UpdateHealthBar(maxHealth, maxHealth);
        }

        public void UpdateHealthBar(int currentHealth, int maxHealth)
        {
            if (fillImage != null)
            {
                float fillAmount = Mathf.Clamp01((float)currentHealth / maxHealth);
                fillImage.fillAmount = fillAmount;
                
                // Renk değişimi (Opsiyonel)
                fillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, fillAmount);
            }

            if (healthText != null)
            {
                healthText.text = $"{currentHealth}/{maxHealth}";
            }
        }
    }
}
