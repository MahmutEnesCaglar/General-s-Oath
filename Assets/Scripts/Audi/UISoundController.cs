using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerDefense.UI
{
    /// <summary>
    /// UI elemanlarına (Button, Slider, Toggle vb.) tıklama ve hover sesi ekler.
    /// Inspector'dan sürükleyip bırakarak kullanılır.
    /// </summary>
    public class UISoundController : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        [Header("Ses Efektleri")]
        public AudioClip hoverSound;
        public AudioClip clickSound;
        
        private AudioSource audioSource;
        private Button button;
        private Toggle toggle;
        private Slider slider;

        void Start()
        {
            // AudioSource yoksa ekle
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }

            // Bileşenleri al (Interactable kontrolü için)
            button = GetComponent<Button>();
            toggle = GetComponent<Toggle>();
            slider = GetComponent<Slider>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsInteractable()) return;

            if (hoverSound != null)
            {
                // SFXManager üzerinden çal (Volume ayarına uyar)
                SFXManager.PlaySound(hoverSound, audioSource);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInteractable()) return;

            if (clickSound != null)
            {
                // SFXManager üzerinden çal (Volume ayarına uyar)
                SFXManager.PlaySound(clickSound, audioSource);
            }
        }

        /// <summary>
        /// UI elemanı etkileşime kapalıysa ses çalma
        /// </summary>
        private bool IsInteractable()
        {
            if (button != null && !button.interactable) return false;
            if (toggle != null && !toggle.interactable) return false;
            if (slider != null && !slider.interactable) return false;
            
            // CanvasGroup kontrolü (Parent'ta interactable kapalı olabilir)
            CanvasGroup canvasGroup = GetComponentInParent<CanvasGroup>();
            if (canvasGroup != null && !canvasGroup.interactable) return false;

            return true;
        }
    }
}
