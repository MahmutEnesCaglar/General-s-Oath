using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TowerDefense.Data;
using TowerDefense.Core;

namespace TowerDefense.Enemy
{
    /// <summary>
    /// Düşman spawn sistemini yönetir
    /// GameManager tarafından tetiklenir
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Ayarları")]
        public Transform spawnPoint;          // Düşmanların spawn olacağı nokta

        [Header("Düşman Prefab'ları")]
        public GameObject basicEnemyPrefab;
        public GameObject fastEnemyPrefab;
        public GameObject armoredEnemyPrefab;
        public GameObject archerEnemyPrefab;
        public GameObject flyingEnemyPrefab;
        public GameObject eliteEnemyPrefab;
        public GameObject miniBossPrefab;
        public GameObject grifonBossPrefab;
        public GameObject kirinBossPrefab;
        public GameObject ejderhaBossPrefab;

        [Header("Durum")]
        public bool isSpawning = false;
        private int totalEnemiesInWave = 0;
        private int enemiesSpawned = 0;
        private int enemiesAlive = 0;

        [Header("Referanslar")]
        private FinalBossConfigurator bossConfigurator;
        private Dictionary<string, EnemyType> enemyTypeData;

        private void Awake()
        {
            // Spawn point otomatik bul
            if (spawnPoint == null)
            {
                GameObject spawnObj = GameObject.Find("SpawnPoint");
                if (spawnObj != null)
                    spawnPoint = spawnObj.transform;
                else
                    Debug.LogError("SpawnPoint bulunamadı! Scene'de 'SpawnPoint' adlı bir GameObject oluşturun.");
            }

            // Boss configurator'dan düşman verilerini al
            if (GameManager.Instance != null && GameManager.Instance.bossConfigurator != null)
            {
                bossConfigurator = GameManager.Instance.bossConfigurator;
                LoadEnemyTypeData();
            }
        }

        /// <summary>
        /// Düşman tipi verilerini yükler
        /// </summary>
        private void LoadEnemyTypeData()
        {
            enemyTypeData = new Dictionary<string, EnemyType>();
            List<EnemyType> allEnemies = bossConfigurator.GetAllEnemyTypes();

            foreach (var enemy in allEnemies)
            {
                enemyTypeData[enemy.type] = enemy;
            }

            Debug.Log($"✓ {enemyTypeData.Count} düşman tipi yüklendi.");
        }

        /// <summary>
        /// Wave'i spawn eder
        /// </summary>
        public void SpawnWave(WaveData wave)
        {
            if (isSpawning)
            {
                Debug.LogWarning("Zaten bir wave spawn oluyor!");
                return;
            }

            StartCoroutine(SpawnWaveCoroutine(wave));
        }

        /// <summary>
        /// Wave spawn coroutine
        /// </summary>
        private IEnumerator SpawnWaveCoroutine(WaveData wave)
        {
            isSpawning = true;
            enemiesSpawned = 0;
            totalEnemiesInWave = 0;

            // Toplam düşman sayısını hesapla
            foreach (var spawnInfo in wave.enemies)
            {
                totalEnemiesInWave += spawnInfo.count;
            }

            enemiesAlive = totalEnemiesInWave;

            Debug.Log($"Wave {wave.waveNumber} başlıyor: {totalEnemiesInWave} düşman spawn olacak");

            // Her düşman grubunu spawn et
            foreach (var spawnInfo in wave.enemies)
            {
                for (int i = 0; i < spawnInfo.count; i++)
                {
                    SpawnEnemy(spawnInfo.enemyType);
                    enemiesSpawned++;

                    // Spawn gecikmesi
                    yield return new WaitForSeconds(spawnInfo.spawnDelay);
                }
            }

            isSpawning = false;
            Debug.Log($"Wave {wave.waveNumber} spawn tamamlandı. Toplam: {enemiesSpawned} düşman");
        }

        /// <summary>
        /// Tek bir düşman spawn eder
        /// </summary>
        private void SpawnEnemy(string enemyType)
        {
            // Prefab'ı seç
            GameObject prefab = GetEnemyPrefab(enemyType);

            if (prefab == null)
            {
                Debug.LogError($"Düşman prefab'ı bulunamadı: {enemyType}");
                return;
            }

            // Spawn point kontrolü
            if (spawnPoint == null)
            {
                Debug.LogError("Spawn point tanımlanmamış!");
                return;
            }

            // Düşmanı spawn et
            GameObject enemyObj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            enemyObj.name = $"{enemyType}_{enemiesSpawned}";

            // Enemy component'i al ve initialize et
            Enemy enemyScript = enemyObj.GetComponent<Enemy>();
            if (enemyScript != null && enemyTypeData.ContainsKey(enemyType))
            {
                enemyScript.InitializeFromEnemyType(enemyTypeData[enemyType]);

                // Waypoint'leri manuel ata (otomatik bulmayı override et)
                SetEnemyWaypoints(enemyScript);
            }

            // Düşman öldüğünde event dinle
            StartCoroutine(WaitForEnemyDeath(enemyObj));
        }

        /// <summary>
        /// Düşmana waypoint'leri atar
        /// SpawnPoint'ten SONRA başlayan yolu oluşturur
        /// </summary>
        private void SetEnemyWaypoints(Enemy enemyScript)
        {
            // EnemyPath'i bul
            GameObject pathParent = GameObject.Find("EnemyPath");
            if (pathParent == null)
            {
                Debug.LogError("EnemyPath objesi bulunamadı!");
                return;
            }

            // Tüm waypoint'leri al
            int childCount = pathParent.transform.childCount;
            Transform[] waypoints = new Transform[childCount];

            for (int i = 0; i < childCount; i++)
            {
                waypoints[i] = pathParent.transform.GetChild(i);
            }

            // Waypoint'leri enemy'ye ata
            enemyScript.waypoints = waypoints;

            Debug.Log($"{enemyScript.name}: {waypoints.Length} waypoint atandı (spawn pozisyonundan başlayacak)");
        }

        /// <summary>
        /// Düşmanın ölmesini bekler
        /// </summary>
        private IEnumerator WaitForEnemyDeath(GameObject enemy)
        {
            // Düşman yok olana kadar bekle
            while (enemy != null)
            {
                yield return null;
            }

            // Düşman öldü veya üsse ulaştı
            enemiesAlive--;

            // Tüm düşmanlar öldü mü?
            if (enemiesAlive <= 0)
            {
                OnWaveCompleted();
            }
        }

        /// <summary>
        /// Wave tamamlandığında
        /// </summary>
        private void OnWaveCompleted()
        {
            Debug.Log("✓ Wave tamamlandı! Tüm düşmanlar yok edildi.");

            // GameManager'a bildir (ileride auto-start next wave için)
            if (GameManager.Instance != null)
            {
                // GameManager.Instance.OnWaveCompleted();
            }
        }

        /// <summary>
        /// Düşman tipine göre prefab döndürür
        /// </summary>
        private GameObject GetEnemyPrefab(string enemyType)
        {
            switch (enemyType.ToLower())
            {
                case "basic":
                    return basicEnemyPrefab;
                case "fast":
                    return fastEnemyPrefab;
                case "armored":
                    return armoredEnemyPrefab;
                case "archer":
                    return archerEnemyPrefab;
                case "flying":
                    return flyingEnemyPrefab;
                case "elite":
                    return eliteEnemyPrefab;
                case "mini-boss":
                    return miniBossPrefab;
                case "final-boss-grifon":
                    return grifonBossPrefab;
                case "final-boss-kirin":
                    return kirinBossPrefab;
                case "final-boss-ejderha":
                    return ejderhaBossPrefab;
                default:
                    Debug.LogWarning($"Bilinmeyen düşman tipi: {enemyType}");
                    return basicEnemyPrefab; // Fallback
            }
        }

        /// <summary>
        /// Kaç düşman hayatta?
        /// </summary>
        public int GetAliveEnemyCount()
        {
            return enemiesAlive;
        }

        /// <summary>
        /// Wave hala spawn oluyor mu?
        /// </summary>
        public bool IsWaveActive()
        {
            return isSpawning || enemiesAlive > 0;
        }
    }
}
