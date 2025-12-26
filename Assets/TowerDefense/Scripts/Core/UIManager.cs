using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro kullanıyorsanız
using TowerDefense.Enemy;
using TowerDefense.Tower;

namespace TowerDefense.Core
{
    /// <summary>
    /// Oyun UI'ını yönetir
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Text Elemanları")]
        public TextMeshProUGUI moneyText;      // Para göstergesi
        public TextMeshProUGUI livesText;      // Can göstergesi
        public TextMeshProUGUI waveText;       // Wave numarası

        // Eğer TextMeshPro yerine normal Text kullanıyorsanız:
        // public Text moneyText;
        // public Text livesText;
        // public Text waveText;

        [Header("Butonlar")]
        public Button startWaveButton;         // Wave başlat butonu
        public Button groundTowerButton;       // Okçu kulesi butonu
        public Button universalTowerButton;    // Büyücü kulesi butonu
        public Button aoeTowerButton;          // Bomba kulesi butonu

        [Header("Paneller")]
        public GameObject gameOverPanel;       // Game Over paneli
        public GameObject victoryPanel;        // Zafer paneli
        public GameObject towerSelectionPanel; // Kule seçim paneli

        [Header("Kule Fiyat Göstergeleri")]
        public TextMeshProUGUI groundTowerCostText;
        public TextMeshProUGUI universalTowerCostText;
        public TextMeshProUGUI aoeTowerCostText;

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
            // Buton listener'ları ekle
            if (startWaveButton != null)
                startWaveButton.onClick.AddListener(OnStartWaveButtonClicked);

            if (groundTowerButton != null)
                groundTowerButton.onClick.AddListener(() => OnTowerButtonClicked("ground"));

            if (universalTowerButton != null)
                universalTowerButton.onClick.AddListener(() => OnTowerButtonClicked("universal"));

            if (aoeTowerButton != null)
                aoeTowerButton.onClick.AddListener(() => OnTowerButtonClicked("aoe"));

            // Başlangıç değerlerini güncelle
            UpdateUI();
            UpdateTowerCosts();
        }

        private void Update()
        {
            // Her frame UI'ı güncelle (performans için Update yerine event-based yapılabilir)
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
                waveText.text = $"Wave: {GameManager.Instance.currentWave}/10";

            // Start Wave butonu
            if (startWaveButton != null)
            {
                // Wave aktifse butonu devre dışı bırak
                EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
                if (spawner != null)
                {
                    startWaveButton.interactable = !spawner.IsWaveActive();
                }
            }
        }

        /// <summary>
        /// Kule fiyatlarını gösterir
        /// </summary>
        private void UpdateTowerCosts()
        {
            if (GameManager.Instance == null || GameManager.Instance.towerConfigurator == null)
                return;

            // Ground Tower
            var groundTower = GameManager.Instance.towerConfigurator.CreateGroundTower();
            if (groundTowerCostText != null && groundTower != null)
                groundTowerCostText.text = $"{groundTower.GetStatsForLevel(1).upgradeCost}";

            // Universal Tower
            var universalTower = GameManager.Instance.towerConfigurator.CreateUniversalTower();
            if (universalTowerCostText != null && universalTower != null)
                universalTowerCostText.text = $"{universalTower.GetStatsForLevel(1).upgradeCost}";

            // AOE Tower
            var aoeTower = GameManager.Instance.towerConfigurator.CreateAOETower();
            if (aoeTowerCostText != null && aoeTower != null)
                aoeTowerCostText.text = $"{aoeTower.GetStatsForLevel(1).upgradeCost}";
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

        /// <summary>
        /// Kule butonuna tıklandığında
        /// </summary>
        private void OnTowerButtonClicked(string towerType)
        {
            Debug.Log($"{towerType} kulesi seçildi! (Yerleştirme modu aktif)");

            // TowerPlacement sistemi burada devreye girer
            TowerPlacement placement = FindObjectOfType<TowerPlacement>();
            if (placement != null)
            {
                placement.SelectTower(towerType);
            }
        }

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
            // Ana menü scene'ini yükle
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
