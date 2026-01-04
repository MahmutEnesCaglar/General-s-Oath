using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TowerDefense.Core
{
    /// <summary>
    /// DialogManager - Wave 10 Boss Dialog Sistemi
    /// Anka Simurg (Phoenix Boss) için özel dialog gösterir
    /// </summary>
    public class DialogManager : MonoBehaviour
    {
        public static DialogManager Instance { get; private set; }

        [Header("Dialog UI Elements")]
        [Tooltip("Dialog panel (Canvas içinde Panel/Image)")]
        public GameObject dialogPanel;

        [Tooltip("Dialog text (Text/TextMeshPro)")]
        public Text dialogText; // Veya TextMeshProUGUI

        [Tooltip("Karakter ismi gösterimi (örn: 'Anka Simurg')")]
        public Text characterNameText;

        [Header("Dialog Settings")]
        [Tooltip("Her karakter yazma hızı (saniye)")]
        public float typingSpeed = 0.05f;

        [Tooltip("Dialog otomatik kapanma süresi (0 = manuel)")]
        public float autoCloseDelay = 5f;

        [Header("Anka Simurg Dialog (Narrator)")]
        [TextArea(3, 6)]
        [Tooltip("Anka Simurg'un narrator dialog metni - Boss'un geldiğini haber verir")]
        public string narratorDialogText = "Brave warriors...\n\nYou have proven yourselves worthy by defeating my Elite guards.\n\nBut now... the TRUE test begins.\n\nBehold, the arrival of the DARK SORCERER!\n\nMay the light guide your path...";

        [Tooltip("Narrator karakter ismi")]
        public string narratorCharacterName = "ANKA SIMURG - The Phoenix Guardian";

        private bool isDialogActive = false;
        private Coroutine typingCoroutine;

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Dialog başlangıçta kapalı
            if (dialogPanel != null)
            {
                dialogPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Anka Simurg narrator dialog'unu gösterir (Wave 10)
        /// Boss spawn'lanmadan ÖNCE görünür - Boss'un geldiğini haber verir
        /// WaveManager tarafından çağrılır
        /// </summary>
        public void ShowBossDialog()
        {
            if (isDialogActive) return;

            StartCoroutine(DisplayNarratorDialog());
        }

        /// <summary>
        /// Anka Simurg narrator dialog gösterme rutini
        /// Elite'ler öldükten sonra Boss'un geldiğini haber verir
        /// </summary>
        private IEnumerator DisplayNarratorDialog()
        {
            isDialogActive = true;

            Debug.Log($"<color=magenta>═══════════════════════════════════</color>");
            Debug.Log($"<color=magenta>ANKA SIMURG SPEAKS...</color>");
            Debug.Log($"<color=magenta>═══════════════════════════════════</color>");

            // Panel'i aç
            if (dialogPanel != null)
            {
                dialogPanel.SetActive(true);
            }

            // Karakter ismini ayarla (Narrator - Anka Simurg)
            if (characterNameText != null)
            {
                characterNameText.text = narratorCharacterName;
            }

            // Metni typewriter efektiyle yaz
            if (dialogText != null)
            {
                dialogText.text = "";
                typingCoroutine = StartCoroutine(TypeText(narratorDialogText));
            }

            // Typewriter bitene kadar bekle
            while (typingCoroutine != null)
            {
                yield return null;
            }

            // Otomatik kapanma
            if (autoCloseDelay > 0)
            {
                yield return new WaitForSeconds(autoCloseDelay);
                CloseDialog();
            }
        }

        /// <summary>
        /// Typewriter efekti - metni karakter karakter yazar
        /// </summary>
        private IEnumerator TypeText(string text)
        {
            dialogText.text = "";

            foreach (char c in text)
            {
                dialogText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }

            typingCoroutine = null;
        }

        /// <summary>
        /// Dialog'u kapatır
        /// </summary>
        public void CloseDialog()
        {
            if (!isDialogActive) return;

            // Typewriter'ı durdur
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            // Panel'i kapat
            if (dialogPanel != null)
            {
                dialogPanel.SetActive(false);
            }

            isDialogActive = false;

            Debug.Log("<color=yellow>Dialog Closed!</color>");
        }

        /// <summary>
        /// Genel dialog gösterme (gelecek genişletmeler için)
        /// </summary>
        public void ShowDialog(string characterName, string message, float displayDuration = 3f)
        {
            if (isDialogActive) return;

            StartCoroutine(DisplayCustomDialog(characterName, message, displayDuration));
        }

        /// <summary>
        /// Özel dialog gösterme rutini
        /// </summary>
        private IEnumerator DisplayCustomDialog(string characterName, string message, float duration)
        {
            isDialogActive = true;

            if (dialogPanel != null)
            {
                dialogPanel.SetActive(true);
            }

            if (characterNameText != null)
            {
                characterNameText.text = characterName;
            }

            if (dialogText != null)
            {
                dialogText.text = "";
                typingCoroutine = StartCoroutine(TypeText(message));
            }

            // Typewriter bitene kadar bekle
            while (typingCoroutine != null)
            {
                yield return null;
            }

            // Belirtilen süre kadar göster
            yield return new WaitForSeconds(duration);

            CloseDialog();
        }

        private void Update()
        {
            // Manuel kapatma: Space/Enter tuşu
            if (isDialogActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
            {
                // Typewriter devam ediyorsa hızlıca bitir
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    typingCoroutine = null;
                    dialogText.text = narratorDialogText; // Tüm metni göster
                }
                else
                {
                    // Dialog'u kapat
                    CloseDialog();
                }
            }
        }
    }
}
