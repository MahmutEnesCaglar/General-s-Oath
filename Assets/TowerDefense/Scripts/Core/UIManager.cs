using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TowerDefense.Core
{
    /// <summary>
    /// Oyun UI'ını yönetir (Para, Can, Wave, Game Over)
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Text Elemanları")]
        public TextMeshProUGUI moneyText;      // Para göstergesi
        public TextMeshProUGUI livesText;      // Can göstergesi
        public TextMeshProUGUI waveText;       // Wave numarası

        [Header("Butonlar")]
        public Button startWaveButton;         // Wave başlat butonu

        [Header("Paneller")]
        public GameObject gameOverPanel;       // Game Over paneli
        public GameObject victoryPanel;        // Zafer paneli
        // public GameObject towerSelectionPanel; // ARTIK YOK (BuildManager yönetiyor)

        private void Awake()
        {
            // Singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Panelleri gizle
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (victoryPanel != null) victoryPanel.SetActive(false);
        }

        private void Start()
        {
            // Start Wave butonu dinleyicisi
            if (startWaveButton != null)
                startWaveButton.onClick.AddListener(OnStartWaveButtonClicked);

            // Başlangıç değerlerini güncelle
            UpdateUI();
        }

        private void Update()
        {
            // Her frame UI'ı güncelle
            UpdateUI();
        }

        /// <summary>
        /// UI'ı günceller
        /// </summary>
        public void UpdateUI()
        {
            if (GameManager.Instance == null) return;

            // Para
            if (moneyText != null)
                moneyText.text = $"Para: {GameManager.Instance.playerMoney}";

            // Can
            if (livesText != null)
                livesText.text = $"Can: {GameManager.Instance.playerLives}";

            // Wave (WaveManager'dan alınıyor)
            if (waveText != null && WaveManager.Instance != null)
                waveText.text = $"Wave: {WaveManager.Instance.currentWaveIndex}/10";

            // Start Wave butonu kontrolü (WaveManager'dan alınıyor)
            if (startWaveButton != null && WaveManager.Instance != null)
            {
                // Wave aktif değilse ve spawn olmuyorsa buton aktif
                bool canStartWave = !WaveManager.Instance.isWaveActive && !WaveManager.Instance.isSpawning;
                startWaveButton.interactable = canStartWave;
            }
        }

        /// <summary>
        /// Start Wave butonuna tıklandığında
        /// </summary>
        private void OnStartWaveButtonClicked()
        {
            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.StartWaveManually();
            }
            else
            {
                Debug.LogError("WaveManager bulunamadı!");
            }
        }

        // ESKİ KULE BUTON FONKSİYONLARI SİLİNDİ (BuildManager artık bu işi yapıyor)

        /// <summary>
        /// Game Over ekranını gösterir
        /// </summary>
        public void ShowGameOver()
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);
        }

        /// <summary>
        /// Zafer ekranını gösterir
        /// </summary>
        public void ShowVictory()
        {
            if (victoryPanel != null)
                victoryPanel.SetActive(true);
        }

        /// <summary>
        /// Oyunu yeniden başlatır
        /// </summary>
        public void RestartGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }

        /// <summary>
        /// Ana menüye döner
        /// </summary>
        public void ReturnToMainMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}