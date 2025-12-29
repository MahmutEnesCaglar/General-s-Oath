using UnityEngine;
using TowerDefense.Data;
using TowerDefense.Tower;
using TowerDefense.Enemy;
using System.Collections.Generic;

namespace TowerDefense.Core
{
    /// <summary>
    /// Oyunun ana yönetici sınıfı
    /// Tüm sistemleri koordine eder
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // Singleton pattern
        public static GameManager Instance { get; private set; }

        [Header("Harita Verileri")]
        public MapData currentMap;
        private List<MapData> allMaps = new List<MapData>();

        [Header("Oyun Durumu")]
        public int currentWave = 0;
        public int playerMoney = 100;
        public int playerLives = 5; // Her düşman geçince -1 can, toplam 5 can
        public bool isGameActive = false;

        [Header("Referanslar")]
        public WaveConfigurator waveConfigurator;
        public TowerConfigurator towerConfigurator;
        public FinalBossConfigurator bossConfigurator;
        public Enemy.EnemySpawner enemySpawner;

        [Header("Hero System")]
        public GameObject heroPrefab;
        public Transform heroSpawnPoint; // Optional manual spawn point
        public TowerDefense.Hero.Hero currentHero;
        public bool enableHeroRespawn = false;
        public float heroRespawnDelay = 5f;

        [Header("UI System")]
        public SpriteHealthBar healthBar; // Can barı görseli (Inspector'dan atanacak)
        
        [Header("UI Yöneticisi")]
        public UIManager uiManager; // <--- BU SATIRI EKLE

        private void Awake()
        {
            // Singleton kontrolü
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

        /// <summary>
        /// Oyunu başlatır ve tüm haritaları yükler
        /// </summary>
        private void InitializeGame()
        {
            Debug.Log("=== TOWER DEFENSE OYUNU BAŞLIYOR ===\n");

            // Configurator'ları oluştur
            if (waveConfigurator == null)
                waveConfigurator = gameObject.AddComponent<WaveConfigurator>();

            if (towerConfigurator == null)
                towerConfigurator = gameObject.AddComponent<TowerConfigurator>();

            if (bossConfigurator == null)
                bossConfigurator = gameObject.AddComponent<FinalBossConfigurator>();

            // Tüm haritaları oluştur
            LoadAllMaps();

            // Debug: Tüm verileri yazdır
            PrintGameConfiguration();
        }

        /// <summary>
        /// Tüm haritaları yükler
        /// </summary>
        private void LoadAllMaps()
        {
            allMaps.Clear();

            allMaps.Add(waveConfigurator.CreateGrifonMap());
            allMaps.Add(waveConfigurator.CreateKirinMap());
            allMaps.Add(waveConfigurator.CreateEjderhaMap());

            Debug.Log($"✓ {allMaps.Count} harita yüklendi");
        }

        /// <summary>
        /// Belirli bir haritayı başlatır
        /// </summary>
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
            playerLives = 5; // 5 can ile başla
            isGameActive = true;

            Debug.Log($"\n=== {currentMap.mapName} BAŞLADI ===");
            Debug.Log($"General: {currentMap.generalName}");
            Debug.Log($"Efsanevi Yaratık: {currentMap.mythicalCreature}");
            Debug.Log($"Başlangıç Parası: {playerMoney} coin");
            Debug.Log($"Can: {playerLives}");

            // Can barını başlat
            InitializeHealthBar();

            // Spawn hero at base
            SpawnHero();
        }

        /// <summary>
        /// Can barını başlatır ve 5/5 can gösterir
        /// </summary>
        private void InitializeHealthBar()
        {
            // Eğer healthBar Inspector'dan atanmamışsa, sahnede bul
            if (healthBar == null)
            {
                healthBar = FindAnyObjectByType<SpriteHealthBar>();
            }

            if (healthBar != null)
            {
                healthBar.maxHealth = 5;
                healthBar.currentHealth = 5;
                healthBar.UpdateHealthSprite();
                Debug.Log("✓ Can barı başlatıldı: 5/5");
            }
            else
            {
                Debug.LogWarning("⚠️ SpriteHealthBar bulunamadı! Sahnede CanBariGorseli objesi olduğundan emin olun.");
            }
        }

        /// <summary>
        /// Sonraki wave'i başlatır
        /// </summary>
        public void StartNextWave()
        {
            if (!isGameActive || currentMap == null)
            {
                Debug.LogWarning("Oyun aktif değil veya harita seçilmedi!");
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
            Debug.Log($"Kazanılabilecek Para: {wave.totalMoneyReward} coin");

            if (currentWave == 10)
            {
                Debug.Log($"⚠️ FİNAL BOSS: {currentMap.finalBossName} ⚠️");
            }

            // Enemy spawner'ı tetikle
            if (enemySpawner != null)
            {
                enemySpawner.SpawnWave(wave);
            }
            else
            {
                Debug.LogError("EnemySpawner atanmamış!");
            }
        }

        /// <summary>
        /// Düşman öldürüldüğünde para kazandırır
        /// </summary>
        public void OnEnemyKilled(int moneyReward)
        {
            playerMoney += moneyReward;
            Debug.Log($"+ {moneyReward} coin! Toplam: {playerMoney} coin");
        }

        /// <summary>
        /// Kule satın alır
        /// </summary>
        public bool PurchaseTower(string towerType)
        {
            TowerStats tower = GetTowerStats(towerType);
            if (tower == null) return false;

            int cost = tower.GetStatsForLevel(1).upgradeCost;

            if (playerMoney >= cost)
            {
                playerMoney -= cost;
                Debug.Log($"{tower.towerName} satın alındı! Kalan para: {playerMoney} coin");
                return true;
            }
            else
            {
                Debug.Log($"Yetersiz para! Gerekli: {cost}, Mevcut: {playerMoney}");
                return false;
            }
        }

        /// <summary>
        /// Kule yükseltir
        /// </summary>
        public bool UpgradeTower(string towerType, int currentLevel)
        {
            if (currentLevel >= 3)
            {
                Debug.Log("Kule zaten maksimum seviyede!");
                return false;
            }

            TowerStats tower = GetTowerStats(towerType);
            if (tower == null) return false;

            int upgradeCost = tower.GetStatsForLevel(currentLevel + 1).upgradeCost;

            if (playerMoney >= upgradeCost)
            {
                playerMoney -= upgradeCost;
                Debug.Log($"{tower.towerName} Seviye {currentLevel + 1}'e yükseltildi! Kalan para: {playerMoney} coin");
                return true;
            }
            else
            {
                Debug.Log($"Yetersiz para! Gerekli: {upgradeCost}, Mevcut: {playerMoney}");
                return false;
            }
        }

        /// <summary>
        /// Kule istatistiklerini döndürür
        /// </summary>
        private TowerStats GetTowerStats(string towerType)
        {
            switch (towerType.ToLower())
            {
                case "ground":
                    return towerConfigurator.CreateGroundTower();
                case "universal":
                    return towerConfigurator.CreateUniversalTower();
                case "aoe":
                    return towerConfigurator.CreateAOETower();
                default:
                    Debug.LogError($"Bilinmeyen kule tipi: {towerType}");
                    return null;
            }
        }

        /// <summary>
        /// Düşman üsse ulaştığında can kaybı
        /// Her düşman için -1 can
        /// </summary>
        public void OnEnemyReachedBase(int damage)
        {
            // Her düşman geçince -1 can (damage parametresi kullanılmıyor artık)
            playerLives -= 1;
            Debug.Log($"⚠️ Düşman üsse ulaştı! -1 can. Kalan can: {playerLives}");

            // Can barını güncelle
            UpdateHealthBar();

            if (playerLives <= 0)
            {
                OnGameOver();
            }
        }

        /// <summary>
        /// Can barını günceller (her can kaybında)
        /// </summary>
        private void UpdateHealthBar()
        {
            // Eğer healthBar yoksa tekrar ara
            if (healthBar == null)
            {
                healthBar = FindAnyObjectByType<SpriteHealthBar>();
            }

            if (healthBar != null)
            {
                // SpriteHealthBar'ın TakeDamage fonksiyonunu kullanmak yerine
                // direkt currentHealth'i set edip güncelle
                healthBar.currentHealth = playerLives;
                healthBar.UpdateHealthSprite();
                Debug.Log($"✓ Can barı güncellendi: {playerLives}/{healthBar.maxHealth}");
            }
            else
            {
                Debug.LogWarning("⚠️ Can barı güncellenemedi - SpriteHealthBar bulunamadı!");
            }
        }

        /// <summary>
        /// Harita tamamlandığında
        /// </summary>
        private void OnMapCompleted()
        {
            Debug.Log($"\n🎉 {currentMap.mapName} TAMAMLANDI! 🎉");
            Debug.Log($"Toplam Para: {playerMoney} coin");
            Debug.Log($"Kalan Can: {playerLives}");
            isGameActive = false;
        }

        /// <summary>
        /// Oyun kaybedildiğinde
        /// </summary>
        private void OnGameOver()
        {
            Debug.Log("\n💀 OYUN BİTTİ 💀");
            Debug.Log($"Wave: {currentWave}/10");
            isGameActive = false;

            // UIManager'a Game Over ekranını göster
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameOver();
            }
        }

        /// <summary>
        /// Oyun konfigürasyonunu yazdırır
        /// </summary>
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

        /// <summary>
        /// Belirli bir haritanın detaylarını döndürür
        /// </summary>
        public MapData GetMap(int index)
        {
            if (index >= 0 && index < allMaps.Count)
                return allMaps[index];
            return null;
        }

        /// <summary>
        /// Toplam harita sayısını döndürür
        /// </summary>
        public int GetTotalMapCount()
        {
            return allMaps.Count;
        }

        // ==================== HERO SYSTEM ====================

        /// <summary>
        /// Spawns hero at base location
        /// </summary>
        private void SpawnHero()
        {
            if (heroPrefab == null)
            {
                Debug.LogWarning("Hero prefab not assigned in GameManager! Skipping hero spawn.");
                return;
            }

            // Determine spawn position (base/last waypoint)
            Vector3 spawnPos = GetHeroSpawnPosition();

            GameObject heroObj = Instantiate(heroPrefab, spawnPos, Quaternion.identity);
            currentHero = heroObj.GetComponent<TowerDefense.Hero.Hero>();

            Debug.Log($"✓ Hero spawned at base: {spawnPos}");
        }

        /// <summary>
        /// Gets hero spawn position (priority: manual spawn point > last waypoint > origin)
        /// </summary>
        private Vector3 GetHeroSpawnPosition()
        {
            // Priority 1: Manual spawn point
            if (heroSpawnPoint != null)
            {
                return heroSpawnPoint.position;
            }

            // Priority 2: Last waypoint (base)
            GameObject pathObj = GameObject.Find("EnemyPath");
            if (pathObj != null && pathObj.transform.childCount > 0)
            {
                Transform lastWaypoint = pathObj.transform.GetChild(pathObj.transform.childCount - 1);
                return lastWaypoint.position;
            }

            // Fallback: origin
            Debug.LogWarning("No hero spawn point or enemy path found, spawning at origin");
            return Vector3.zero;
        }

        /// <summary>
        /// Called when hero dies
        /// </summary>
        public void OnHeroKilled()
        {
            Debug.Log("⚔️ Hero has fallen!");

            if (enableHeroRespawn)
            {
                StartCoroutine(RespawnHeroAfterDelay());
            }
            else
            {
                // Treat as game over
                playerLives = 0;
                OnGameOver();
            }
        }

        /// <summary>
        /// Respawns hero after delay
        /// </summary>
        private System.Collections.IEnumerator RespawnHeroAfterDelay()
        {
            yield return new WaitForSeconds(heroRespawnDelay);
            SpawnHero();
            Debug.Log($"✓ Hero respawned after {heroRespawnDelay}s!");
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
