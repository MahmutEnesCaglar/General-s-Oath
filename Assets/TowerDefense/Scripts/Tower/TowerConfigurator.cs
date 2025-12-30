using UnityEngine;
using TowerDefense.Data;

namespace TowerDefense.Tower
{
    /// <summary>
    /// Tüm kule türlerinin konfigürasyonlarını oluşturur
    /// 
    /// KULE TİPLERİ:
    /// 1. Archer Tower (Okçu Kulesi) - Sadece yerdeki düşmanlara saldırır
    /// 2. Cannon (Bomba Kulesi) - Hem yere hem havaya saldırır
    /// 3. Mortar (Havan topu) - Alan hasarı verir
    /// </summary>
    public class TowerConfigurator : MonoBehaviour
    {
        /// <summary>
        /// GROUND TOWER (OKÇU KULESİ)
        /// Yerleştirme: 75 coin
        /// Seviye 2: 125 coin
        /// Seviye 3: 200 coin
        /// TOPLAM: 400 coin
        ///
        /// Özellikleri:
        /// - Sadece yerdeki düşmanlara saldırır
        /// - Yüksek saldırı hızı
        /// - Orta menzil
        /// </summary>
        public TowerStats CreateGroundTower()
        {
            TowerStats tower = new TowerStats(
                "Okçu Kulesi",
                "ground",
                canTargetAir: false,
                isAOE: false
            );

            // Seviye 1
            tower.levelStats[0] = new TowerLevelStats(
                level: 1,
                damage: 15,
                range: 3.5f,
                attackSpeed: 1.0f,  // Her 1 saniyede bir ateş eder
                upgradeCost: 75     // Yerleştirme maliyeti
            );

            // Seviye 2
            tower.levelStats[1] = new TowerLevelStats(
                level: 2,
                damage: 25,         // +10 hasar
                range: 4.5f,        // +1 menzil
                attackSpeed: 0.8f,  // Daha hızlı
                upgradeCost: 125    // Seviye 2'ye yükseltme maliyeti
            );

            // Seviye 3
            tower.levelStats[2] = new TowerLevelStats(
                level: 3,
                damage: 45,         // +20 hasar
                range: 6.0f,        // +1.5 menzil
                attackSpeed: 0.6f,  // En hızlı
                upgradeCost: 200    // Seviye 3'e yükseltme maliyeti
            );

            return tower;
        }

        /// <summary>
        /// UNIVERSAL TOWER (BÜYÜCÜ KULESİ / CANNON)
        /// Yerleştirme: 60 coin
        /// Seviye 2: 100 coin
        /// Seviye 3: 160 coin
        /// TOPLAM: 320 coin
        ///
        /// Özellikleri:
        /// - Hem yere hem havaya saldırır
        /// - Orta hasar
        /// - Geniş menzil
        /// - En stratejik kule
        /// </summary>
        public TowerStats CreateUniversalTower()
        {
            TowerStats tower = new TowerStats(
                "Büyücü Kulesi",
                "universal",
                canTargetAir: true,
                isAOE: false
            );

            // Seviye 1
            tower.levelStats[0] = new TowerLevelStats(
                level: 1,
                damage: 20,
                range: 4.0f,
                attackSpeed: 1.2f,
                upgradeCost: 60     // Yerleştirme maliyeti
            );

            // Seviye 2
            tower.levelStats[1] = new TowerLevelStats(
                level: 2,
                damage: 35,         // +15 hasar
                range: 5.5f,        // +1.5 menzil
                attackSpeed: 1.0f,
                upgradeCost: 100    // Seviye 2'ye yükseltme maliyeti
            );

            // Seviye 3
            tower.levelStats[2] = new TowerLevelStats(
                level: 3,
                damage: 60,         // +25 hasar
                range: 7.0f,        // +1.5 menzil
                attackSpeed: 0.8f,
                upgradeCost: 160    // Seviye 3'e yükseltme maliyeti
            );

            return tower;
        }

        /// <summary>
        /// AOE TOWER (BOMBA KULESİ / MORTAR)
        /// Yerleştirme: 125 coin
        /// Seviye 2: 200 coin
        /// Seviye 3: 300 coin
        /// TOPLAM: 625 coin
        ///
        /// Özellikleri:
        /// - Alan hasarı verir
        /// - Yavaş ateş hızı
        /// - Kalabalık düşman gruplarına etkili
        /// </summary>
        public TowerStats CreateAOETower()
        {
            TowerStats tower = new TowerStats(
                "Bomba Kulesi",
                "aoe",
                canTargetAir: false,
                isAOE: true,
                aoeRadius: 2.0f     // 2 birim yarıçapında patlama
            );

            // Seviye 1
            tower.levelStats[0] = new TowerLevelStats(
                level: 1,
                damage: 25,         // Alan hasarı
                range: 4.0f,
                attackSpeed: 2.0f,  // Yavaş
                upgradeCost: 125    // Yerleştirme maliyeti
            );

            // Seviye 2
            tower.levelStats[1] = new TowerLevelStats(
                level: 2,
                damage: 40,         // +15 hasar
                range: 5.0f,        // +1 menzil
                attackSpeed: 1.7f,
                upgradeCost: 200    // Seviye 2'ye yükseltme maliyeti
            );

            // Seviye 3
            tower.levelStats[2] = new TowerLevelStats(
                level: 3,
                damage: 70,         // +30 hasar
                range: 6.5f,        // +1.5 menzil
                attackSpeed: 1.4f,
                upgradeCost: 300    // Seviye 3'e yükseltme maliyeti
            );

            return tower;
        }

        /// <summary>
        /// Tüm kule bilgilerini yazdırır (Debug için)
        /// </summary>
        public void PrintAllTowerStats()
        {
            Debug.Log("=== KULE İSTATİSTİKLERİ ===\n");

            TowerStats groundTower = CreateGroundTower();
            PrintTowerInfo(groundTower);

            Debug.Log("\n" + new string('-', 50) + "\n");

            TowerStats universalTower = CreateUniversalTower();
            PrintTowerInfo(universalTower);

            Debug.Log("\n" + new string('-', 50) + "\n");

            TowerStats aoeTower = CreateAOETower();
            PrintTowerInfo(aoeTower);
        }

        /// <summary>
        /// Bir kulenin detaylı bilgilerini yazdırır
        /// </summary>
        private void PrintTowerInfo(TowerStats tower)
        {
            Debug.Log($"=== {tower.towerName} ({tower.towerType}) ===");
            Debug.Log($"Havaya Saldırır: {(tower.canTargetAir ? "Evet" : "Hayır")}");
            Debug.Log($"Alan Hasarı: {(tower.isAOE ? "Evet (Yarıçap: " + tower.aoeRadius + ")" : "Hayır")}");
            Debug.Log($"Toplam Maliyet: {tower.GetTotalUpgradeCost()} coin\n");

            for (int i = 0; i < 3; i++)
            {
                var stats = tower.levelStats[i];
                Debug.Log($"Seviye {stats.level}:");
                Debug.Log($"  Hasar: {stats.damage}");
                Debug.Log($"  Menzil: {stats.range}");
                Debug.Log($"  Ateş Hızı: {stats.attackSpeed}s");
                Debug.Log($"  {(i == 0 ? "Yerleştirme" : "Yükseltme")} Maliyeti: {stats.upgradeCost} coin\n");
            }
        }
    }
}
