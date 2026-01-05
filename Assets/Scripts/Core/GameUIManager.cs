using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using TowerDefense.Enemy;

namespace TowerDefense.Core
{
    /// <summary>
    /// Oyun UI'ını yönetir (Para, Can, Wave, Victory, Game Over)
    /// </summary>
    public class VictoryDefeatManager : MonoBehaviour
    {
        public static VictoryDefeatManager Instance { get; private set; }
        
        [Header("Text Elemanları")]
        public TextMeshProUGUI moneyText;
        public TextMeshProUGUI livesText;
        public TextMeshProUGUI waveText;

        [Header("Butonlar")]
        public Button startWaveButton;

        [Header("Victory Popup")]
        public GameObject victoryPopup;
        public Image star1;
        public Image star2;
        public Image star3;
        public Button victoryRetryButton;
        public Button victoryWorldMapButton;
        
        [Header("GameOver Popup")]
        public GameObject gameOverPopup;
        public Button gameOverRetryButton;
        public Button gameOverWorldMapButton;
        
        [Header("Yıldız Sprite'ları")]
        public Sprite starEmpty;
        public Sprite starFilled;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            // Popup'ları gizle
            if (victoryPopup != null)
                victoryPopup.SetActive(false);
            
            if (gameOverPopup != null)
                gameOverPopup.SetActive(false);
            
            // Start Wave butonu
            if (startWaveButton != null)
                startWaveButton.onClick.AddListener(OnStartWaveButtonClicked);
            
            // Victory butonları
            if (victoryRetryButton != null)
                victoryRetryButton.onClick.AddListener(RetryLevel);
            
            if (victoryWorldMapButton != null)
                victoryWorldMapButton.onClick.AddListener(ReturnToWorldMap);
            
            // GameOver butonları
            if (gameOverRetryButton != null)
                gameOverRetryButton.onClick.AddListener(RetryLevel);
            
            if (gameOverWorldMapButton != null)
                gameOverWorldMapButton.onClick.AddListener(ReturnToWorldMap);

            UpdateResourceUI();
            UpdateUI();
        }

        private void Update()
        {
            UpdateUI();
        }

        public void UpdateResourceUI()
        {
            if (GameManager.Instance == null) return;

            if (moneyText != null)
                moneyText.text = $"{GameManager.Instance.playerMoney} G";

            if (livesText != null)
                livesText.text = $"Can: {GameManager.Instance.playerLives}";
        }

        public void UpdateUI()
        {
            if (GameManager.Instance == null) return;

            UpdateResourceUI();

            if (waveText != null && WaveManager.Instance != null)
                waveText.text = $"Wave: {WaveManager.Instance.currentWaveIndex}/10";

            if (startWaveButton != null && WaveManager.Instance != null)
            {
                bool canStartWave = !WaveManager.Instance.isWaveActive && !WaveManager.Instance.isSpawning;
                startWaveButton.interactable = canStartWave;
            }
        }

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

        public void ShowGameOver()
        {
            Debug.Log("💀 Yenildi!");
            Time.timeScale = 0f;
            
            if (gameOverPopup != null)
                gameOverPopup.SetActive(true);
        }

        public void ShowVictory()
        {
            Debug.Log("=== SHOWVICTORY BAŞLADI ===");
            
            try
            {
                Debug.Log("ShowVictory() çağrıldı!");
                
                Time.timeScale = 0f;
                Debug.Log("Time.timeScale = 0 yapıldı");
                
                int stars = CalculateStars();
                
                Debug.Log($"CalculateStars() sonucu: {stars} yıldız");
                
                if (victoryPopup != null)
                {
                    Debug.Log("victoryPopup açılıyor...");
                    victoryPopup.SetActive(true);
                }
                else
                {
                    Debug.LogError("❌ victoryPopup NULL!");
                }
                
                UpdateStars(stars);
                SaveLevelProgress(stars);
                UnlockNextLevel();
                
                Debug.Log($"🎉 Zafer! {stars} yıldız kazanıldı!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ShowVictory() HATA: {e.Message}\n{e.StackTrace}");
            }
        }

        int CalculateStars()
        {
            Debug.Log("CalculateStars() başladı!");
            
            int currentHealth = 0;
            
            Debug.Log($"GameManager.Instance null mu? {GameManager.Instance == null}");
            
            if (GameManager.Instance != null)
            {
                Debug.Log($"healthBar null mu? {GameManager.Instance.healthBar == null}");
                
                if (GameManager.Instance.healthBar != null)
                {
                    currentHealth = GameManager.Instance.healthBar.currentHealth;
                    Debug.Log($"✅ Kalan can: {currentHealth}");
                }
                else
                {
                    Debug.LogWarning("⚠️ HealthBar NULL!");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ GameManager.Instance NULL!");
            }
            
            int stars = 0;
            if (currentHealth >= 5)
                stars = 3;      // 5 can → 3 yıldız
            else if (currentHealth >= 3)
                stars = 2;      // 3-4 can → 2 yıldız
            else if (currentHealth >= 1)
                stars = 1;      // 1-2 can → 1 yıldız
            else
                stars = 0;      // 0 can → 0 yıldız (ama buraya gelmemeli, zaten Game Over)
            
            Debug.Log($"🌟 Hesaplanan yıldız: {stars} (Can: {currentHealth})");
            
            return stars;
        }

        void UpdateStars(int starCount)
        {
            if (star1 != null)
                star1.sprite = starCount >= 1 ? starFilled : starEmpty;
            
            if (star2 != null)
                star2.sprite = starCount >= 2 ? starFilled : starEmpty;
            
            if (star3 != null)
                star3.sprite = starCount >= 3 ? starFilled : starEmpty;
        }

        void SaveLevelProgress(int stars)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            
            int previousStars = PlayerPrefs.GetInt(currentScene + "_Stars", 0);
            
            if (stars > previousStars)
            {
                PlayerPrefs.SetInt(currentScene + "_Stars", stars);
                PlayerPrefs.Save();
                Debug.Log($"✓ {currentScene}: {stars} yıldız kaydedildi");
            }
        }

        void UnlockNextLevel()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            
            if (currentScene == "Map_Grifon")
            {
                PlayerPrefs.SetInt("Map_Kirin_Unlocked", 1);
                Debug.Log("🔓 Map_Kirin kilidi açıldı!");
            }
            else if (currentScene == "Map_Kirin")
            {
                PlayerPrefs.SetInt("Map_Ejderha_Unlocked", 1);
                Debug.Log("🔓 Map_Ejderha kilidi açıldı!");
            }
            
            PlayerPrefs.Save();
        }

        void RetryLevel()
        {
            Time.timeScale = 1f;
            
            string currentScene = SceneManager.GetActiveScene().name;
            
            if (SceneTransition.Instance != null)
                SceneTransition.Instance.LoadScene(currentScene);
            else
                SceneManager.LoadScene(currentScene);
        }

        void ReturnToWorldMap()
        {
            Time.timeScale = 1f;
            
            if (SceneTransition.Instance != null)
                SceneTransition.Instance.LoadScene("WorldMap");
            else
                SceneManager.LoadScene("WorldMap");
        }
    }
}