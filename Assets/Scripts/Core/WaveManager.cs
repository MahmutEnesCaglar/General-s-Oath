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

            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Basic, 5) }));
            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Basic, 10) }));
            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Basic, 12), new WaveEnemy(EnemyTypeEnum.Fast, 2) }));
            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Basic, 10), new WaveEnemy(EnemyTypeEnum.Fast, 4) }));
            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Basic, 8), new WaveEnemy(EnemyTypeEnum.Fast, 6), new WaveEnemy(EnemyTypeEnum.Armored, 2) }));
            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Basic, 6), new WaveEnemy(EnemyTypeEnum.Fast, 6), new WaveEnemy(EnemyTypeEnum.Armored, 4) }));
            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Fast, 4), new WaveEnemy(EnemyTypeEnum.Armored, 4), new WaveEnemy(EnemyTypeEnum.Elite, 6) }));
            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Fast, 8), new WaveEnemy(EnemyTypeEnum.Armored, 6), new WaveEnemy(EnemyTypeEnum.Archer, 2) }));
            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Armored, 6), new WaveEnemy(EnemyTypeEnum.Archer, 4), new WaveEnemy(EnemyTypeEnum.Elite, 7) }));
            waves.Add(new Wave(new WaveEnemy[] { new WaveEnemy(EnemyTypeEnum.Elite, 10) })); // Boss TriggerBossDialog() içinde spawn edilecek


            Debug.Log($"<color=cyan>WaveManager Initialized! Total Waves: {waves.Count}</color>");
        }

        public void StartWaveManually()
        {
            Debug.Log($"[WaveManager] StartWaveManually() çağrıldı! isWaveActive={isWaveActive}, isSpawning={isSpawning}");
            
            if (!isWaveActive && !isSpawning)
            {
                Debug.Log("[WaveManager] Yeni wave başlatılıyor...");
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
            // 1. Önce listeden silmeye çalış
            if (activeEnemies.Contains(enemy))
            {
                activeEnemies.Remove(enemy);
                Debug.Log($"🔻 Düşman Eksildi! Kalan Düşman: {activeEnemies.Count}");
            }
            else
            {
                Debug.LogWarning("⚠️ Silinmeye çalışılan düşman listede bulunamadı (Zaten silinmiş olabilir).");
            }

            // 2. Wave 10 Elite ve Boss Mantığı
            if (currentWaveIndex == 10 && !wave10DialogShown)
            {
                // GetComponent null hatası vermesin diye kontrol ediyoruz
                if (enemy != null) 
                {
                    BaseEnemy baseEnemy = enemy.GetComponent<BaseEnemy>();
                    // Geçen düşman Elite ise sayacı düşmeli mi? 
                    // EVET, çünkü o da sahneden gitti. Boss'un gelmesi için ölmesi veya gitmesi fark etmez.
                    if (baseEnemy != null && baseEnemy is EliteEnemy)
                    {
                        wave10EliteCount--;
                        Debug.Log($"⚔️ Elite Gitti! Kalan Elite: {wave10EliteCount}/10");

                        if (wave10EliteCount <= 0 && !wave10DialogShown)
                        {
                            wave10DialogShown = true;
                            StartCoroutine(TriggerBossDialog());
                        }
                    }
                }
            }

            // 3. Bölüm Bitti mi Kontrolü
            CheckWaveComplete();
        }

        private void CheckWaveComplete()
        {
            Debug.Log($"[CheckWaveComplete] isSpawning={isSpawning}, currentWave={currentWaveIndex}, activeEnemies={activeEnemies.Count}, wave10Boss={wave10BossSpawned}");
            
            // Spawn hala devam ediyorsa bitirme
            if (isSpawning) 
            {
                Debug.Log("⏳ Wave bitmedi: Hala spawn yapılıyor.");
                return;
            }

            // Listede hala düşman varsa bitirme (Boss dahil)
            if (activeEnemies.Count > 0)
            {
                Debug.Log($"⏳ Wave bitmedi: Sahnede {activeEnemies.Count} düşman var.");
                return;
            }

            // --- BURAYA GELDİYSE WAVE BİTMİŞ DEMEKTİR ---
            
            Debug.Log($"✅ WAVE {currentWaveIndex} TAMAMLANDI!");
            isWaveActive = false;

            // Wave 10 bittiyse (Boss öldü) -> VICTORY
            if (currentWaveIndex == 10)
            {
                Debug.Log("🏆 TÜM WAVE'LER BİTTİ! VICTORY ÇAĞRILIYOR...");
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnAllWavesCompleted();
                }
                return;
            }
            
            // Başka wave'ler varsa buton bekle
            if (currentWaveIndex < waves.Count)
            {
                Debug.Log("➡️ Wave bitti, sıradaki wave için buton bekleniyor.");
                if (VictoryDefeatManager.Instance != null)
                {
                    VictoryDefeatManager.Instance.UpdateUI();
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