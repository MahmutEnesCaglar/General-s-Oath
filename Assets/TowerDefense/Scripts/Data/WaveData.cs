using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Data
{
    /// <summary>
    /// Bir wave'deki düşman spawn bilgilerini tutar
    /// </summary>
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public string enemyType;      // Düşman tipi
        public int count;             // Kaç adet spawn olacak
        public float spawnDelay;      // Düşmanlar arası spawn süresi (saniye)
    }

    /// <summary>
    /// Tek bir wave'in tüm bilgilerini tutar
    /// </summary>
    [System.Serializable]
    public class WaveData
    {
        public int waveNumber;                          // Wave numarası (1-10)
        public List<EnemySpawnInfo> enemies;            // Bu wave'de gelecek düşmanlar
        public float delayBeforeNextWave;               // Sonraki wave'e kadar bekleme süresi
        public int totalMoneyReward;                    // Bu wave'den kazanılabilecek toplam para

        public WaveData(int waveNumber, float delayBeforeNextWave = 5f)
        {
            this.waveNumber = waveNumber;
            this.delayBeforeNextWave = delayBeforeNextWave;
            this.enemies = new List<EnemySpawnInfo>();
            this.totalMoneyReward = 0;
        }

        /// <summary>
        /// Wave'e düşman ekler
        /// </summary>
        public void AddEnemy(string type, int count, float spawnDelay = 1f)
        {
            enemies.Add(new EnemySpawnInfo 
            { 
                enemyType = type, 
                count = count, 
                spawnDelay = spawnDelay 
            });
        }

        /// <summary>
        /// Bu wave'den kazanılabilecek toplam parayı hesaplar
        /// </summary>
        public void CalculateTotalReward(Dictionary<string, int> enemyMoneyValues)
        {
            totalMoneyReward = 0;
            foreach (var spawnInfo in enemies)
            {
                if (enemyMoneyValues.ContainsKey(spawnInfo.enemyType))
                {
                    totalMoneyReward += enemyMoneyValues[spawnInfo.enemyType] * spawnInfo.count;
                }
            }
        }
    }
}
