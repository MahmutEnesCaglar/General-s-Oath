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

        [Header("Harita Verileri")]
        public MapData currentMap;
        private List<MapData> allMaps = new List<MapData>();

        [Header("Oyun Durumu")]
        public int currentWave = 0;
        public int playerMoney = 100;
        public int playerLives = 5;
        public bool isGameActive = false;

        [Header("Referanslar")]
        public WaveConfigurator waveConfigurator;
        public TowerConfigurator towerConfigurator;
        public FinalBossConfigurator bossConfigurator;
        public Enemy.EnemySpawner enemySpawner;

        [Header("Hero System")]
        public GameObject heroPrefab;
        public Transform heroSpawnPoint;
        public TowerDefense.Hero.Hero currentHero;
        public bool enableHeroRespawn = false;
        public float heroRespawnDelay = 5f;

        [Header("UI System")]
        public HealthBar healthBar; // ← DEĞİŞTİ (SpriteHealthBar → HealthBar)

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

            if (waveConfigurator == null)
                waveConfigurator = gameObject.AddComponent<WaveConfigurator>();

            if (towerConfigurator == null)
                towerConfigurator = gameObject.AddComponent<TowerConfigurator>();

            if (bossConfigurator == null)
                bossConfigurator = gameObject.AddComponent<FinalBossConfigurator>();

            LoadAllMaps();
            PrintGameConfiguration();
        }

        private void LoadAllMaps()
        {
            allMaps.Clear();

            allMaps.Add(waveConfigurator.CreateGrifonMap());
            allMaps.Add(waveConfigurator.CreateKirinMap());
            allMaps.Add(waveConfigurator.CreateEjderhaMap());

            Debug.Log($"✓ {allMaps.Count} harita yüklendi");
        }

        public void StartMap(int mapIndex)
        {
            if (mapIndex < 0 || mapIndex >= allMaps.Count)
            {
                Debug.LogError($"Geçersiz harita indexi: {mapIndex}");
                return;
            }

            currentMap = allMaps[mapIndex];
            currentWave = 0;
            playerMoney = currentMap.startingMoney;
            playerLives = 5;
            isGameActive = true;

            Debug.Log($"\n=== {currentMap.mapName} BAŞLADI ===");
            Debug.Log($"General: {currentMap.generalName}");
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
            if (!isGameActive || currentMap == null)
            {
                Debug.LogWarning("Oyun aktif değil!");
                return;
            }

            if (currentWave >= currentMap.waves.Count)
            {
                Debug.Log("Tüm wave'ler tamamlandı!");
                OnMapCompleted();
                return;
            }

            currentWave++;
            WaveData wave = currentMap.waves[currentWave - 1];

            Debug.Log($"\n--- WAVE {currentWave} BAŞLADI ---");

            if (enemySpawner != null)
            {
                enemySpawner.SpawnWave(wave);
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
            Debug.Log($"\n🎉 {currentMap.mapName} TAMAMLANDI! 🎉");
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
            waveConfigurator.InitializeAllMaps();
            Debug.Log("\n" + new string('=', 70));
            towerConfigurator.PrintAllTowerStats();
            Debug.Log("\n" + new string('=', 70));
            bossConfigurator.PrintAllBossInfo();
            Debug.Log("\n" + new string('=', 70) + "\n");
        }

        public MapData GetMap(int index)
        {
            if (index >= 0 && index < allMaps.Count)
                return allMaps[index];
            return null;
        }

        public int GetTotalMapCount()
        {
            return allMaps.Count;
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
    }
}