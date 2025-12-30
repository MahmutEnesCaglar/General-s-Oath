using UnityEngine;
using TowerDefense.Data;
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
        public int playerMoney = 100;
        public int playerLives = 5;
        public bool isGameActive = false;

        [Header("Referanslar")]
        public WaveManager waveManager;
        public TowerConfigurator towerConfigurator;
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

        private void InitializeGame()
        {
            Debug.Log("=== TOWER DEFENSE OYUNU BAŞLIYOR ===\n");

            if (waveManager == null)
                waveManager = FindAnyObjectByType<WaveManager>();

            if (towerConfigurator == null)
                towerConfigurator = gameObject.AddComponent<TowerConfigurator>();

            if (bossConfigurator == null)
                bossConfigurator = gameObject.AddComponent<FinalBossConfigurator>();

            PrintGameConfiguration();
        }

        public void StartGame()
        {
            currentWave = 0;
            playerMoney = 100;
            playerLives = 5;
            isGameActive = true;

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
            playerMoney += moneyReward;
            Debug.Log($"+ {moneyReward} coin! Toplam: {playerMoney} coin");
        }

        public bool PurchaseTower(string towerType)
        {
            TowerStats tower = GetTowerStats(towerType);
            if (tower == null) return false;

            int cost = tower.GetStatsForLevel(1).upgradeCost;

            if (playerMoney >= cost)
            {
                playerMoney -= cost;
                Debug.Log($"{tower.towerName} satın alındı! Kalan: {playerMoney}");
                return true;
            }
            
            Debug.Log($"Yetersiz para!");
            return false;
        }

        public bool UpgradeTower(string towerType, int currentLevel)
        {
            if (currentLevel >= 3) return false;

            TowerStats tower = GetTowerStats(towerType);
            if (tower == null) return false;

            int upgradeCost = tower.GetStatsForLevel(currentLevel + 1).upgradeCost;

            if (playerMoney >= upgradeCost)
            {
                playerMoney -= upgradeCost;
                return true;
            }
            
            return false;
        }

        private TowerStats GetTowerStats(string towerType)
        {
            switch (towerType.ToLower())
            {
                case "ground": return towerConfigurator.CreateGroundTower();
                case "universal": return towerConfigurator.CreateUniversalTower();
                case "aoe": return towerConfigurator.CreateAOETower();
                default: return null;
            }
        }

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
            if (towerConfigurator != null)
                towerConfigurator.PrintAllTowerStats();
            Debug.Log("\n" + new string('=', 70));
            if (bossConfigurator != null)
                bossConfigurator.PrintAllBossInfo();
            Debug.Log("\n" + new string('=', 70) + "\n");
        }

        // HERO SYSTEM
        private void SpawnHero()
        {
            if (heroPrefab == null) return;

            Vector3 spawnPos = GetHeroSpawnPosition();
            GameObject heroObj = Instantiate(heroPrefab, spawnPos, Quaternion.identity);
            currentHero = heroObj.GetComponent<TowerDefense.Hero.Hero>();

            Debug.Log($"✓ Hero spawned");
        }

        private Vector3 GetHeroSpawnPosition()
        {
            if (heroSpawnPoint != null)
                return heroSpawnPoint.position;

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
        // GameManager.cs içine ekle:

        public bool HasMoney(int amount)
        {
            return playerMoney >= amount;
        }

        public void SpendMoney(int amount)
        {
            if (playerMoney >= amount)
            {
                playerMoney -= amount;
                // UI güncelleme metodu varsa çağır (UpdateResourceUI gibi)
                if (uiManager != null) uiManager.UpdateResourceUI(); 
            }
        }
    }
}