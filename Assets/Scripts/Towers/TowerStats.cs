using UnityEngine;

namespace TowerDefense.Data
{
    /// <summary>
    /// Bir kule seviyesinin istatistiklerini tutar
    /// </summary>
    [System.Serializable]
    public class TowerLevelStats
    {
        public int level;           // Seviye (1-3)
        public int damage;          // Hasar
        public float range;         // Menzil
        public float attackSpeed;   // Saldırı hızı (saniye)
        public int upgradeCost;     // Bu seviyeye yükseltme maliyeti (level 1 için placement cost)

        public TowerLevelStats(int level, int damage, float range, float attackSpeed, int upgradeCost)
        {
            this.level = level;
            this.damage = damage;
            this.range = range;
            this.attackSpeed = attackSpeed;
            this.upgradeCost = upgradeCost;
        }
    }

    /// <summary>
    /// Bir kule türünün tüm seviye istatistiklerini tutar
    /// </summary>
    [System.Serializable]
    public class TowerStats
    {
        public string towerName;                    // Kule adı
        public string towerType;                    // Tip: "ground", "universal", "aoe"
        public TowerLevelStats[] levelStats;        // 3 seviye istatistikleri
        public bool canTargetAir;                   // Havaya saldırabilir mi?
        public bool isAOE;                          // Alan hasarı mı?
        public float aoeRadius;                     // Alan yarıçapı (AOE için)

        public TowerStats(string name, string type, bool canTargetAir, bool isAOE, float aoeRadius = 0f)
        {
            this.towerName = name;
            this.towerType = type;
            this.canTargetAir = canTargetAir;
            this.isAOE = isAOE;
            this.aoeRadius = aoeRadius;
            this.levelStats = new TowerLevelStats[3];
        }

        /// <summary>
        /// Belirli bir seviye için istatistikleri döndürür
        /// </summary>
        public TowerLevelStats GetStatsForLevel(int level)
        {
            if (level < 1 || level > 3)
            {
                Debug.LogError($"Geçersiz kule seviyesi: {level}");
                return levelStats[0];
            }
            return levelStats[level - 1];
        }

        /// <summary>
        /// Toplam yükseltme maliyetini hesaplar (max seviyeye kadar)
        /// </summary>
        public int GetTotalUpgradeCost()
        {
            int total = 0;
            foreach (var stat in levelStats)
            {
                total += stat.upgradeCost;
            }
            return total;
        }
    }
}
