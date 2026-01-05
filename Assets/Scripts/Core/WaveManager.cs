using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TowerDefense.Enemy;

namespace TowerDefense.Core
{
    /// <summary>
    /// WaveManager - 10 Dalgalı düşman sistemi yönetir
    /// Wave 10: 10 Elite → Dialog (Anka Simurg) → Boss Spawn
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager Instance { get; private set; }

        [Header("Wave Settings")]
        [Tooltip("Dalgalar arası bekleme süresi (saniye)")]
        public float timeBetweenWaves = 5f;

        [Tooltip("Her düşman spawn arasındaki süre (saniye)")]
        public float timeBetweenSpawns = 0.5f;

        [Header("Spawn Settings")]
        [Tooltip("Düşmanların takip edeceği waypoint sistemi - İLK waypoint spawn noktasıdır")]
        public Transform[] waypoints;

        [Header("Enemy Prefabs")]
        public GameObject basicEnemyPrefab;
        public GameObject fastEnemyPrefab;
        public GameObject armoredEnemyPrefab;
        public GameObject archerEnemyPrefab;
        public GameObject eliteEnemyPrefab;
        public GameObject bossEnemyPrefab;

        [Header("Wave State")]
        public int currentWaveIndex = 0; // 0 = Wave 1
        public bool isSpawning = false;
        public bool isWaveActive = false;


        [Header("Wave 10 Special")]
        [Tooltip("Wave 10'da kalan Elite sayısı")]
        private int wave10EliteCount = 0;
        private bool wave10DialogShown = false;
        private bool wave10BossSpawned = false;

        [Header("Dialog System (Wave 10)")]
        public DialogManager dialogManager; // Dialog sistemi referansı

        // Wave tanımları
        private List<Wave> waves = new List<Wave>();

        // Aktif düşmanları takip et
        private List<GameObject> activeEnemies = new List<GameObject>();

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

            InitializeWaves();
        }

        private void Start()
        {
            Debug.Log("<color=yellow>Wave başlatmak için Start Wave butonuna basın!</color>");
        }

        /// <summary>
        /// 10 Dalganın tanımlarını oluşturur
        /// </summary>
        private void InitializeWaves()
        {
            waves.Clear();

            // Wave 1: 8 Basic
            waves.Add(new Wave(new WaveEnemy[]
            {
                new WaveEnemy(EnemyTypeEnum.Basic, 1),
                new WaveEnemy(EnemyTypeEnum.Boss, 1) // Test amaçlı Boss ekledim

            }));

            // Wave 2: 10 Basic
            waves.Add(new Wave(new WaveEnemy[]
            {
                new WaveEnemy(EnemyTypeEnum.Basic, 10)
            }));

            // Wave 3: 12 Basic + 2 Fast
            waves.Add(new Wave(new WaveEnemy[]
            {
                new WaveEnemy(EnemyTypeEnum.Basic, 12),
                new WaveEnemy(EnemyTypeEnum.Fast, 2)
            }));

            // Wave 4: 10 Basic + 4 Fast
            waves.Add(new Wave(new WaveEnemy[]
            {
                new WaveEnemy(EnemyTypeEnum.Basic, 10),
                new WaveEnemy(EnemyTypeEnum.Fast, 4)
            }));

            // Wave 5: 8 Basic + 6 Fast + 2 Armored
            waves.Add(new Wave(new WaveEnemy[]
            {
                new WaveEnemy(EnemyTypeEnum.Basic, 8),
                new WaveEnemy(EnemyTypeEnum.Fast, 6),
                new WaveEnemy(EnemyTypeEnum.Armored, 2)
            }));

            // Wave 6: 6 Basic + 6 Fast + 4 Armored
            waves.Add(new Wave(new WaveEnemy[]
            {
                new WaveEnemy(EnemyTypeEnum.Basic, 6),
                new WaveEnemy(EnemyTypeEnum.Fast, 6),
                new WaveEnemy(EnemyTypeEnum.Armored, 4)
            }));

            // Wave 7: 4 Fast + 4 Armored + 6 Elite (kullanıcı 8'den 6'ya düşürdü)
            waves.Add(new Wave(new WaveEnemy[]
            {
                new WaveEnemy(EnemyTypeEnum.Fast, 4),
                new WaveEnemy(EnemyTypeEnum.Armored, 4),
                new WaveEnemy(EnemyTypeEnum.Elite, 6)
            }));

            // Wave 8: 8 Fast + 6 Armored + 2 Archer
            waves.Add(new Wave(new WaveEnemy[]
            {
                new WaveEnemy(EnemyTypeEnum.Fast, 8),
                new WaveEnemy(EnemyTypeEnum.Armored, 6),
                new WaveEnemy(EnemyTypeEnum.Archer, 2)
            }));

            // Wave 9: 6 Armored + 4 Archer + 7 Elite (kullanıcı 10'dan 7'ye düşürdü)
            waves.Add(new Wave(new WaveEnemy[]
            {
                new WaveEnemy(EnemyTypeEnum.Armored, 6),
                new WaveEnemy(EnemyTypeEnum.Archer, 4),
                new WaveEnemy(EnemyTypeEnum.Elite, 7)
            }));

            // Wave 10: 10 Elite → (tümü öldükten sonra) → Dialog → Boss
            waves.Add(new Wave(new WaveEnemy[]
            {
                new WaveEnemy(EnemyTypeEnum.Elite, 10)
            }));

            Debug.Log($"<color=cyan>WaveManager Initialized! Total Waves: {waves.Count}</color>");
        }

        /// <summary>
        /// PUBLIC: Butona basıldığında wave başlatmak için
        /// </summary>
        public void StartWaveManually()
        {
            if (!isWaveActive && !isSpawning)
            {
                StartCoroutine(StartNextWave());
            }
            else
            {
                Debug.LogWarning("Wave zaten aktif veya spawn devam ediyor!");
            }
        }

        /// <summary>
        /// Bir sonraki dalgayı başlatır (Buton ile tetiklenir)
        /// </summary>
        private IEnumerator StartNextWave()
        {
            // Manuel başlatma - bekleme yok

            // Tüm dalgalar bittiyse
            if (currentWaveIndex >= waves.Count)
            {
                Debug.Log($"<color=green>ALL WAVES COMPLETED! VICTORY!</color>");
                yield break;
            }

            Wave currentWave = waves[currentWaveIndex];
            currentWaveIndex++;

            Debug.Log($"<color=cyan>═══════════════════════════════════</color>");
            Debug.Log($"<color=cyan>WAVE {currentWaveIndex} STARTING!</color>");
            Debug.Log($"<color=cyan>═══════════════════════════════════</color>");

            // Wave 10 özel hazırlık
            if (currentWaveIndex == 10)
            {
                wave10EliteCount = 10;
                wave10DialogShown = false;
                wave10BossSpawned = false;
                Debug.Log($"<color=orange>FINAL WAVE! Defeat 10 Elites to face the Boss!</color>");
            }

            isWaveActive = true;
            yield return StartCoroutine(SpawnWave(currentWave));
        }

        /// <summary>
        /// Bir dalganın düşmanlarını spawn eder
        /// </summary>
        private IEnumerator SpawnWave(Wave wave)
        {
            isSpawning = true;

            foreach (var enemyGroup in wave.enemies)
            {
                for (int i = 0; i < enemyGroup.count; i++)
                {
                    SpawnEnemy(enemyGroup.type);
                    yield return new WaitForSeconds(timeBetweenSpawns);
                }
            }

            isSpawning = false;
        }

        /// <summary>
        /// Belirtilen tipte düşman spawn eder
        /// </summary>
        private void SpawnEnemy(EnemyTypeEnum enemyType)
        {
            GameObject prefab = GetEnemyPrefab(enemyType);

            if (prefab == null)
            {
                Debug.LogError($"Enemy prefab not found for type: {enemyType}");
                return;
            }

            // İlk waypoint'te spawn et (waypoints[0] = Waypoint_1)
            if (waypoints == null || waypoints.Length == 0)
            {
                Debug.LogError("Waypoints not assigned to WaveManager!");
                return;
            }

            Vector3 spawnPosition = waypoints[0].position;

            // Düşmanı ilk waypoint'te spawn et
            GameObject enemyObj = Instantiate(prefab, spawnPosition, Quaternion.identity);

            // Waypoint sistemini düşmana ata
            BaseEnemy enemy = enemyObj.GetComponent<BaseEnemy>();
            if (enemy != null && waypoints != null && waypoints.Length > 0)
            {
                enemy.SetWaypoints(waypoints);
            }

            // Aktif düşman listesine ekle
            activeEnemies.Add(enemyObj);

            Debug.Log($"<color=cyan>Spawned: {enemyType}</color> (Active Enemies: {activeEnemies.Count})");
        }

        /// <summary>
        /// Enemy type enum'ına göre prefab döndürür
        /// </summary>
        private GameObject GetEnemyPrefab(EnemyTypeEnum type)
        {
            switch (type)
            {
                case EnemyTypeEnum.Basic: return basicEnemyPrefab;
                case EnemyTypeEnum.Fast: return fastEnemyPrefab;
                case EnemyTypeEnum.Armored: return armoredEnemyPrefab;
                case EnemyTypeEnum.Archer: return archerEnemyPrefab;
                case EnemyTypeEnum.Elite: return eliteEnemyPrefab;
                case EnemyTypeEnum.Boss: return bossEnemyPrefab;
                default: return null;
            }
        }

        /// <summary>
        /// Düşman öldüğünde bu fonksiyon çağrılır (BaseEnemy OnDeath'tan)
        /// </summary>
        public void OnEnemyKilled(GameObject enemy)
        {
            // Listeden kaldır
            if (activeEnemies.Contains(enemy))
            {
                activeEnemies.Remove(enemy);
            }

            // Wave 10 Elite takibi
            if (currentWaveIndex == 10 && !wave10DialogShown)
            {
                BaseEnemy baseEnemy = enemy.GetComponent<BaseEnemy>();
                if (baseEnemy != null && baseEnemy is EliteEnemy)
                {
                    wave10EliteCount--;
                    Debug.Log($"<color=orange>Elite Defeated! Remaining: {wave10EliteCount}/10</color>");

                    // Tüm Elite'ler öldü mü?
                    if (wave10EliteCount <= 0 && !wave10DialogShown)
                    {
                        wave10DialogShown = true;
                        StartCoroutine(TriggerBossDialog());
                    }
                }
            }

            // Dalga tamamlandı mı?
            CheckWaveComplete();
        }

        /// <summary>
        /// Dalga tamamlanıp tamamlanmadığını kontrol eder
        /// </summary>
        private void CheckWaveComplete()
        {
            // Spawn devam ediyorsa bekle
            if (isSpawning) return;

            // Wave 10 özel durum: Boss spawn'lanmadıysa bekle
            if (currentWaveIndex == 10 && !wave10BossSpawned) return;

            // Tüm düşmanlar öldü mü?
            if (activeEnemies.Count == 0)
            {
                isWaveActive = false;

                // Wave 10 Boss öldüyse oyun biter
                if (currentWaveIndex == 10 && wave10BossSpawned)
                {
                    Debug.Log($"<color=green>═══════════════════════════════════</color>");
                    Debug.Log($"<color=green>VICTORY! ANKA SIMURG DEFEATED!</color>");
                    Debug.Log($"<color=green>THE REALM IS SAVED!</color>");
                    Debug.Log($"<color=green>═══════════════════════════════════</color>");
                    return; // Oyun bitti
                }

                // Wave tamamlandı - butona basılmasını bekle
                Debug.Log($"<color=yellow>Wave {currentWaveIndex} tamamlandı! Sonraki wave için butona basın.</color>");
            }
        }

        /// <summary>
        /// Wave 10: Tüm Elite'ler öldükten sonra Boss dialog'u gösterir
        /// </summary>
        private IEnumerator TriggerBossDialog()
        {
            Debug.Log($"<color=orange>All Elites Defeated! Anka Simurg Awakens...</color>");

            // Dialog sistemini tetikle
            if (dialogManager != null)
            {
                dialogManager.ShowBossDialog(); // Dialog Manager'da implement edilecek

                // Dialog bitene kadar bekle (örnek: 5 saniye)
                yield return new WaitForSeconds(5f);
            }
            else
            {
                Debug.LogWarning("DialogManager not assigned! Skipping dialog...");
                yield return new WaitForSeconds(2f);
            }

            // Boss'u spawn et
            SpawnBoss();
        }

        /// <summary>
        /// Boss'u spawn eder (Wave 10)
        /// </summary>
        private void SpawnBoss()
        {
            Debug.Log($"<color=red>═══════════════════════════════════</color>");
            Debug.Log($"<color=red>BOSS INCOMING: ANKA SIMURG!</color>");
            Debug.Log($"<color=red>═══════════════════════════════════</color>");

            SpawnEnemy(EnemyTypeEnum.Boss);
            wave10BossSpawned = true;
        }

        private void Update()
        {
            // Aktif düşman listesini temizle (null referansları kaldır)
            activeEnemies.RemoveAll(enemy => enemy == null);
        }
    }

    // ============= WAVE SİSTEMİ DATA STRUCTURES =============

    /// <summary>
    /// Bir dalganın tanımı (hangi düşman tiplerinden kaçar tane)
    /// </summary>
    [System.Serializable]
    public class Wave
    {
        public WaveEnemy[] enemies;

        public Wave(WaveEnemy[] enemies)
        {
            this.enemies = enemies;
        }
    }

    /// <summary>
    /// Bir dalga içindeki düşman grubu (tip + adet)
    /// </summary>
    [System.Serializable]
    public class WaveEnemy
    {
        public EnemyTypeEnum type;
        public int count;

        public WaveEnemy(EnemyTypeEnum type, int count)
        {
            this.type = type;
            this.count = count;
        }
    }

    /// <summary>
    /// Düşman tipleri enum
    /// </summary>
    public enum EnemyTypeEnum
    {
        Basic,
        Fast,
        Armored,
        Archer,
        Elite,
        Boss
    }
}