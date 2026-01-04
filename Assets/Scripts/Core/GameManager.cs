using UnityEngine;
using UnityEngine.SceneManagement; // Eklendi
using TowerDefense.Tower;
using TowerDefense.Enemy;
using System.Collections.Generic;

namespace TowerDefense.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Oyun Durumu")]
        public int currentWave = 0;
        public int playerMoney = 1000;  // Başlangıç parası
        public int playerLives = 5;
        public bool isGameActive = false;

        [Header("Referanslar")]
        public WaveManager waveManager;
        public FinalBossConfigurator bossConfigurator;

        [Header("Hero System")]
        public GameObject heroPrefab;
        public Transform heroSpawnPoint;
        public TowerDefense.Hero.Hero currentHero;
        public bool enableHeroRespawn = false;
        public float heroRespawnDelay = 5f;

        [Header("UI System")]
        public HealthBar healthBar; // Can barı görseli (Inspector'dan atanacak)
        
        [Header("UI Yöneticisi")]
        public UIManager uiManager; // <--- BU SATIRI EKLE

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializeGame();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // MainMenu veya WorldMap değilse, oyun sahnesidir
            // Bu sahnelerde GameManager'ı yeniden başlatıyoruz
            if (scene.name != "MainMenuSahne" && scene.name != "WorldMap")
            {
                Debug.Log($"[GameManager] Yeni sahne yüklendi: {scene.name}. Oyun başlatılıyor...");
                InitializeGame();
                StartGame();
            }
        }

        private void InitializeGame()
        {
            Debug.Log("=== TOWER DEFENSE OYUNU BAŞLIYOR ===\n");
            
            // Zamanı normalleştir
            Time.timeScale = 1f;

            if (waveManager == null)
                waveManager = FindAnyObjectByType<WaveManager>();

            if (bossConfigurator == null)
                bossConfigurator = gameObject.AddComponent<FinalBossConfigurator>();
            
            // UI Manager'ı bul
            if (uiManager == null)
                uiManager = FindAnyObjectByType<UIManager>();

            PrintGameConfiguration();
        }

        public void StartGame()
        {
            currentWave = 0;
            playerMoney = 1000;
            playerLives = 5;
            isGameActive = true;

            // MoneyManager'ı da senkronize et
            if (MoneyManager.Instance != null)
            {
                MoneyManager.Instance.currentMoney = playerMoney;
                MoneyManager.Instance.UpdateMoneyUI();
            }

            Debug.Log($"\n=== OYUN BAŞLADI ===");
            Debug.Log($"Başlangıç Parası: {playerMoney} coin");
            Debug.Log($"Can: {playerLives}");

            InitializeHealthBar();
            SpawnHero();
        }

        private void InitializeHealthBar()
        {
            if (healthBar == null)
            {
                healthBar = FindAnyObjectByType<HealthBar>(); // ← DEĞİŞTİ
            }

            if (healthBar != null)
            {
                healthBar.maxHealth = 5;
                healthBar.currentHealth = 5;
                // UpdateHealthSprite() yok artık, direkt UpdateHealth() çağrılacak
                Debug.Log("✓ Can barı başlatıldı: 5/5");
            }
            else
            {
                Debug.LogWarning("⚠️ HealthBar bulunamadı!");
            }
        }

        public void StartNextWave()
        {
            if (!isGameActive)
            {
                Debug.LogWarning("Oyun aktif değil!");
                return;
            }

            if (waveManager != null)
            {
                waveManager.StartWaveManually();
                currentWave = waveManager.currentWaveIndex;
            }
            else
            {
                Debug.LogWarning("WaveManager bulunamadı!");
            }
        }

        public void OnEnemyKilled(int moneyReward)
        {
            Debug.Log($"<color=cyan>[GameManager] OnEnemyKilled çağrıldı - Reward: {moneyReward}</color>");

            // MoneyManager üzerinden para ekle
            if (MoneyManager.Instance != null)
            {
                MoneyManager.Instance.AddMoney(moneyReward);
                // MoneyManager'ı GameManager ile senkronize et
                int oldPlayerMoney = playerMoney;
                playerMoney = MoneyManager.Instance.currentMoney;
                Debug.Log($"<color=cyan>[GameManager] Senkronize: {oldPlayerMoney} → {playerMoney}</color>");
            }
            else
            {
                // Fallback: MoneyManager yoksa sadece GameManager'ı güncelle
                playerMoney += moneyReward;
                Debug.LogWarning("<color=orange>[GameManager] MoneyManager bulunamadı! Fallback kullanıldı.</color>");
            }
        }



        // GetTowerStats() metodu kaldırıldı - Artık BuildManager prefab-based sistem kullanıyor

        public void OnEnemyReachedBase(int damage)
        {
            playerLives -= 1;
            Debug.Log($"⚠️ Düşman üsse ulaştı! Kalan can: {playerLives}");

            // ← DEĞİŞTİ (UpdateHealthBar() SİLİNDİ)
            if (healthBar != null)
            {
                healthBar.TakeDamage(1); // TakeDamage zaten UpdateHealth() çağırıyor
            }

            if (playerLives <= 0)
            {
                OnGameOver();
            }
        }

        // ← UpdateHealthBar() FONKSİYONU SİLİNDİ

        private void OnMapCompleted()
        {
            Debug.Log($"\n🎉 HARITA TAMAMLANDI! 🎉");
            isGameActive = false;
        }

        private void OnGameOver()
        {
            Debug.Log("\n💀 OYUN BİTTİ 💀");
            isGameActive = false;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameOver();
            }
        }

        private void PrintGameConfiguration()
        {
            Debug.Log("\n" + new string('=', 70));
            Debug.Log("TOWER DEFENSE - GAME CONFIGURATION");
            Debug.Log(new string('=', 70));
            Debug.Log("Tower sistemi BuildManager tarafından yönetiliyor (Prefab-based)");
            Debug.Log("\n" + new string('=', 70));
            if (bossConfigurator != null)
                bossConfigurator.PrintAllBossInfo();
            Debug.Log("\n" + new string('=', 70) + "\n");
        }

        // HERO SYSTEM
        private void SpawnHero()
        {
            if (heroPrefab == null) return;

            // Eğer sahnede zaten bir hero varsa (örn. yanlışlıkla çift spawn), onu temizle
            if (currentHero != null)
            {
                // Unity null check (destroyed object check)
                if (currentHero.gameObject != null)
                    Destroy(currentHero.gameObject);
            }

            Vector3 spawnPos = GetHeroSpawnPosition();
            GameObject heroObj = Instantiate(heroPrefab, spawnPos, Quaternion.identity);
            currentHero = heroObj.GetComponent<TowerDefense.Hero.Hero>();

            Debug.Log($"✓ Hero spawned at {spawnPos}");
        }

        private Vector3 GetHeroSpawnPosition()
        {
            // 1. Inspector referansı (İlk yüklemede çalışır)
            if (heroSpawnPoint != null)
                return heroSpawnPoint.position;

            // 2. İsimle bulma (Sahne yeniden yüklendiğinde çalışır)
            GameObject spawnPointObj = GameObject.Find("HeroSpawnPoint");
            if (spawnPointObj != null)
                return spawnPointObj.transform.position;

            // 3. EnemyPath fallback
            GameObject pathObj = GameObject.Find("EnemyPath");
            if (pathObj != null && pathObj.transform.childCount > 0)
            {
                Transform lastWaypoint = pathObj.transform.GetChild(pathObj.transform.childCount - 1);
                return lastWaypoint.position;
            }

            return Vector3.zero;
        }

        public void OnHeroKilled()
        {
            Debug.Log("⚔️ Hero has fallen!");

            if (enableHeroRespawn)
            {
                StartCoroutine(RespawnHeroAfterDelay());
            }
            else
            {
                playerLives = 0;
                OnGameOver();
            }
        }

        private System.Collections.IEnumerator RespawnHeroAfterDelay()
        {
            yield return new WaitForSeconds(heroRespawnDelay);
            SpawnHero();
        }

    }
}