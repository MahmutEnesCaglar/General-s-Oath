using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using TowerDefense.Enemy;
// TowerPlacement referansı artık gerekmediği için sildik.
// using TowerDefense.Tower; // Gerekirse ekle ama şuan UI için şart değil.

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

            // Wave
            if (waveText != null)
                waveText.text = $"Wave: {GameManager.Instance.currentWave}"; // /10 kısmını WaveConfigurator'dan almak daha doğru olur ama şimdilik böyle kalsın.

            // Start Wave butonu kontrolü
            if (startWaveButton != null)
            {
                // Wave aktifse butonu devre dışı bırak
                // EnemySpawner'ı bulmak maliyetli olabilir, Singleton yaparsan daha iyi olur.
                EnemySpawner spawner = FindAnyObjectByType<EnemySpawner>();
                if (spawner != null)
                {
                    startWaveButton.interactable = !spawner.IsWaveActive();
                }
            }
        }

        /// <summary>
        /// Start Wave butonuna tıklandığında
        /// </summary>
        private void OnStartWaveButtonClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartNextWave();
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