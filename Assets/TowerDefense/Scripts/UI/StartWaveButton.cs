using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using TowerDefense.Core;

namespace TowerDefense.UI
{
    /// <summary>
    /// Medieval/Fantasy temalı Start Wave butonu
    /// Hover, click animasyonları ve wave bilgisi gösterir
    /// </summary>
    public class StartWaveButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("UI References")]
        public Button button;
        public Image buttonImage;
        public TextMeshProUGUI buttonText;
        public TextMeshProUGUI waveInfoText; // "Wave 1/10" gibi

        [Header("Button Styles")]
        public Color normalColor = new Color(0.8f, 0.6f, 0.4f);      // Taş/tahta rengi
        public Color hoverColor = new Color(1f, 0.8f, 0.5f);         // Hover'da biraz aydınlık
        public Color pressedColor = new Color(0.6f, 0.4f, 0.2f);     // Basılıyken koyu
        public Color disabledColor = new Color(0.4f, 0.4f, 0.4f);    // Devre dışıyken gri

        [Header("Animation Settings")]
        public float hoverScale = 1.05f;
        public float pressScale = 0.95f;
        public float animationSpeed = 10f;

        [Header("Glow Effect (Optional)")]
        public Image glowImage; // Parlama efekti için
        public float glowPulseSpeed = 2f;
        public Color glowColor = new Color(1f, 0.9f, 0.5f, 0.5f);

        private Vector3 originalScale;
        private Vector3 targetScale;
        private bool isHovering = false;
        private bool isPressing = false;

        private void Awake()
        {
            if (button == null) button = GetComponent<Button>();
            originalScale = transform.localScale;
            targetScale = originalScale;

            // Buton click event'i
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClicked);
            }
        }

        private void Update()
        {
            // Scale animasyonu
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);

            // Wave bilgisini güncelle
            UpdateWaveInfo();

            // Buton aktif/pasif durumunu kontrol et
            UpdateButtonState();

            // Glow efekti (eğer varsa)
            UpdateGlowEffect();
        }

        /// <summary>
        /// Wave bilgisini günceller (Wave 1/10 gibi)
        /// </summary>
        private void UpdateWaveInfo()
        {
            if (waveInfoText != null && WaveManager.Instance != null)
            {
                int currentWave = WaveManager.Instance.currentWaveIndex;
                int totalWaves = 10; // Toplam 10 wave

                if (currentWave >= totalWaves)
                {
                    waveInfoText.text = "ALL WAVES COMPLETE!";
                }
                else
                {
                    waveInfoText.text = $"WAVE {currentWave + 1}/{totalWaves}";
                }
            }
        }

        /// <summary>
        /// Buton aktif/pasif durumunu kontrol eder
        /// </summary>
        private void UpdateButtonState()
        {
            if (button == null || WaveManager.Instance == null) return;

            bool shouldBeInteractable = !WaveManager.Instance.isWaveActive && !WaveManager.Instance.isSpawning;
            button.interactable = shouldBeInteractable;

            // Renk güncelle
            if (buttonImage != null)
            {
                if (!button.interactable)
                {
                    buttonImage.color = disabledColor;
                }
                else if (isPressing)
                {
                    buttonImage.color = pressedColor;
                }
                else if (isHovering)
                {
                    buttonImage.color = hoverColor;
                }
                else
                {
                    buttonImage.color = normalColor;
                }
            }

            // Buton text'ini güncelle
            if (buttonText != null)
            {
                if (WaveManager.Instance.isWaveActive)
                {
                    buttonText.text = "WAVE IN PROGRESS...";
                }
                else if (WaveManager.Instance.currentWaveIndex >= 10)
                {
                    buttonText.text = "VICTORY!";
                }
                else
                {
                    buttonText.text = "START WAVE";
                }
            }
        }

        /// <summary>
        /// Glow efektini günceller (nabız gibi)
        /// </summary>
        private void UpdateGlowEffect()
        {
            if (glowImage != null && button != null && button.interactable)
            {
                float pulse = (Mathf.Sin(Time.time * glowPulseSpeed) + 1f) * 0.5f; // 0-1 arası
                Color color = glowColor;
                color.a = pulse * glowColor.a;
                glowImage.color = color;
            }
            else if (glowImage != null)
            {
                glowImage.color = Color.clear; // Pasifken glow yok
            }
        }

        // ============= EVENT HANDLERS =============

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (button != null && button.interactable)
            {
                isHovering = true;
                targetScale = originalScale * hoverScale;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
            isPressing = false;
            targetScale = originalScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (button != null && button.interactable)
            {
                isPressing = true;
                targetScale = originalScale * pressScale;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressing = false;
            if (isHovering)
            {
                targetScale = originalScale * hoverScale;
            }
            else
            {
                targetScale = originalScale;
            }
        }

        private void OnButtonClicked()
        {
            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.StartWaveManually();
            }
        }
    }
}
