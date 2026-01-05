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

        private void InitializeWaves()
        {
            waves.Clear();

            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Basic, 8) }));
            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Basic, 10) }));
            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Basic, 12), new WaveEnemy(EnemyTypeEnum.Fast, 2) }));
            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Basic, 10), new WaveEnemy(EnemyTypeEnum.Fast, 4) }));
            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Basic, 8), new WaveEnemy(EnemyTypeEnum.Fast, 6), new WaveEnemy(EnemyTypeEnum.Armored, 2) }));
            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Basic, 6), new WaveEnemy(EnemyTypeEnum.Fast, 6), new WaveEnemy(EnemyTypeEnum.Armored, 4) }));
            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Fast, 4), new WaveEnemy(EnemyTypeEnum.Armored, 4), new WaveEnemy(EnemyTypeEnum.Elite, 6) }));
            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Fast, 8), new WaveEnemy(EnemyTypeEnum.Armored, 6), new WaveEnemy(EnemyTypeEnum.Archer, 2) }));
            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Armored, 6), new WaveEnemy(EnemyTypeEnum.Archer, 4), new WaveEnemy(EnemyTypeEnum.Elite, 7) }));
            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Elite, 10) }));


            Debug.Log($"<color=cyan>WaveManager Initialized! Total Waves: {waves.Count}</color>");
        }

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

        private IEnumerator StartNextWave()
        {
            // Tüm dalgalar bittiyse
            if (currentWaveIndex >= waves.Count)
            {
                Debug.Log($"<color=green>ALL WAVES COMPLETED! VICTORY!</color>");
                
                // ← YENİ: GameManager'a bildir
                if (GameManager.Instance != null)
                    GameManager.Instance.OnAllWavesCompleted();
                
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

        private void SpawnEnemy(EnemyTypeEnum enemyType)
        {
            GameObject prefab = GetEnemyPrefab(enemyType);

            if (prefab == null)
            {
                Debug.LogError($"Enemy prefab not found for type: {enemyType}");
                return;
            }

            if (waypoints == null || waypoints.Length == 0)
            {
                Debug.LogError("Waypoints not assigned to WaveManager!");
                return;
            }

            Vector3 spawnPosition = waypoints[0].position;
            GameObject enemyObj = Instantiate(prefab, spawnPosition, Quaternion.identity);

            BaseEnemy enemy = enemyObj.GetComponent<BaseEnemy>();
            if (enemy != null && waypoints != null && waypoints.Length > 0)
            {
                enemy.SetWaypoints(waypoints);
            }

            activeEnemies.Add(enemyObj);

            Debug.Log($"<color=cyan>Spawned: {enemyType}</color> (Active Enemies: {activeEnemies.Count})");
        }

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

        public void OnEnemyKilled(GameObject enemy)
        {
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

                    if (wave10EliteCount <= 0 && !wave10DialogShown)
                    {
                        wave10DialogShown = true;
                        StartCoroutine(TriggerBossDialog());
                    }
                }
            }

            CheckWaveComplete();
        }

        private void CheckWaveComplete()
        {
            // Spawn devam ediyorsa bekle
            if (isSpawning) return;
            
            // Wave 10 Boss bekliyorsa bekle
            if (currentWaveIndex == 10 && !wave10BossSpawned) return;

            // Tüm düşmanlar öldü/geçti
            if (activeEnemies.Count == 0)
            {
                isWaveActive = false;

                // Wave 10 Boss öldü → Victory!
                if (currentWaveIndex == 10 && wave10BossSpawned)
                {
                    Debug.Log($"<color=green>═══════════════════════════════════</color>");
                    Debug.Log($"<color=green>VICTORY! ANKA SIMURG DEFEATED!</color>");
                    Debug.Log($"<color=green>THE REALM IS SAVED!</color>");
                    Debug.Log($"<color=green>═══════════════════════════════════</color>");
                    
                    if (GameManager.Instance != null)
                        GameManager.Instance.OnAllWavesCompleted();
                    
                    return;
                }

                Debug.Log($"<color=yellow>Wave {currentWaveIndex} tamamlandı! Sonraki wave için butona basın.</color>");
                
                // ← YENİ: Son wave tamamlandıysa Victory!
                if (currentWaveIndex >= waves.Count)
                {
                    Debug.Log("<color=green>ALL WAVES COMPLETED! VICTORY!</color>");
                    
                    if (GameManager.Instance != null)
                        GameManager.Instance.OnAllWavesCompleted();
                }
            }
        }

        private IEnumerator TriggerBossDialog()
        {
            Debug.Log($"<color=orange>All Elites Defeated! Anka Simurg Awakens...</color>");

            if (dialogManager != null)
            {
                dialogManager.ShowBossDialog();
                yield return new WaitForSeconds(5f);
            }
            else
            {
                Debug.LogWarning("DialogManager not assigned! Skipping dialog...");
                yield return new WaitForSeconds(2f);
            }

            SpawnBoss();
        }

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
            activeEnemies.RemoveAll(enemy => enemy == null);
        }
    }

    [System.Serializable]
    public class Wave
    {
        public WaveEnemy[] enemies;
        public Wave(WaveEnemy[] enemies) { this.enemies = enemies; }
    }

    [System.Serializable]
    public class WaveEnemy
    {
        public EnemyTypeEnum type;
        public int count;
        public WaveEnemy(EnemyTypeEnum type, int count) { this.type = type; this.count = count; }
    }

    public enum EnemyTypeEnum
    {
        Basic, Fast, Armored, Archer, Elite, Boss
    }
}