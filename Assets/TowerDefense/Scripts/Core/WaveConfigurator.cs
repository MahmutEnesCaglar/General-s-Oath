using UnityEngine;
using System.Collections.Generic;
using TowerDefense.Data;
using TowerDefense.Enemy;

namespace TowerDefense.Core
{
    /// <summary>
    /// Tüm haritaların wave konfigürasyonlarını oluşturur ve yönetir
    /// EKONOMI DENGESİ:
    /// - Başlangıç parası: 100 coin
    /// - Her harita tamamlandığında oyuncu yeterli para kazanarak sonraki haritaya hazır olur
    /// </summary>
    public class WaveConfigurator : MonoBehaviour
    {
        // Düşman para değerleri
        private Dictionary<string, int> enemyMoney = new Dictionary<string, int>
        {
            { "basic", 5 },
            { "fast", 5 },
            { "armored", 7 },
            { "archer", 6 },
            { "flying", 8 },
            { "elite", 10 },
            { "mini-boss", 15 },
            { "final-boss-grifon", 50 },
            { "final-boss-kirin", 60 },
            { "final-boss-ejderha", 80 }
        };

        /// <summary>
        /// HANGİ HARİTA NE KADAR PARA KAZANDIRIR?
        /// 
        /// Harita 1 (Grifon): ~280 coin (Başlangıç: 100 + Kazanılan: 280 = 380 toplam)
        /// Harita 2 (Kirin): ~420 coin (Başlangıç: 100 + Kazanılan: 420 = 520 toplam)
        /// Harita 3 (Ejderha): ~580 coin (Başlangıç: 100 + Kazanılan: 580 = 680 toplam)
        /// 
        /// KULE MALİYETLERİ:
        /// - Ground Tower: 30 (place) + 40 (lvl2) + 80 (lvl3) = 150 toplam
        /// - Universal Tower: 50 (place) + 60 (lvl2) + 120 (lvl3) = 230 toplam
        /// - AOE Tower: 40 (place) + 50 (lvl2) + 100 (lvl3) = 190 toplam
        /// 
        /// Oyuncu strateji seçmeli - hepsini max upgrade edemez!
        /// </summary>

        #region HARITA 1 - GRIFON (KOLAY)
        public MapData CreateGrifonMap()
        {
            MapData map = new MapData(
                "Grifon'un Dağları", 
                "General Altay", 
                "Grifon", 
                1
            );
            map.startingMoney = 100;
            map.finalBossName = "Yırtıcı Grifon";

            // WAVE 1 - Başlangıç + TEST (25 coin)
            WaveData wave1 = new WaveData(1, 8f);
            wave1.AddEnemy("basic", 3, 1.5f);  // 3 basic = 15 coin
            wave1.AddEnemy("fast", 2, 1.2f);   // 2 fast = 10 coin (TEST için eklendi)
            wave1.CalculateTotalReward(enemyMoney);
            map.AddWave(wave1);

            // WAVE 2 - Biraz daha düşman (30 coin)
            WaveData wave2 = new WaveData(2, 8f);
            wave2.AddEnemy("basic", 6, 1.3f);  // 6 basic = 30 coin
            wave2.CalculateTotalReward(enemyMoney);
            map.AddWave(wave2);

            // WAVE 3 - İlk hızlı düşman (35 coin)
            WaveData wave3 = new WaveData(3, 10f);
            wave3.AddEnemy("basic", 4, 1.5f);  // 4 basic = 20 coin
            wave3.AddEnemy("fast", 3, 1.2f);   // 3 fast = 15 coin
            wave3.CalculateTotalReward(enemyMoney);
            map.AddWave(wave3);

            // WAVE 4 - Zırhlı giriş (36 coin)
            WaveData wave4 = new WaveData(4, 10f);
            wave4.AddEnemy("basic", 3, 1.5f);  // 3 basic = 15 coin
            wave4.AddEnemy("armored", 3, 2f);  // 3 armored = 21 coin
            wave4.CalculateTotalReward(enemyMoney);
            map.AddWave(wave4);

            // WAVE 5 - Karışık (41 coin)
            WaveData wave5 = new WaveData(5, 12f);
            wave5.AddEnemy("basic", 3, 1.5f);  // 15 coin
            wave5.AddEnemy("fast", 2, 1.2f);   // 10 coin
            wave5.AddEnemy("archer", 2, 1.8f); // 12 coin
            wave5.AddEnemy("armored", 1, 2f);  // 7 coin
            wave5.CalculateTotalReward(enemyMoney);
            map.AddWave(wave5);

            // WAVE 6 - Okçu ağırlıklı (48 coin)
            WaveData wave6 = new WaveData(6, 12f);
            wave6.AddEnemy("archer", 4, 1.5f); // 24 coin
            wave6.AddEnemy("basic", 4, 1.3f);  // 20 coin
            wave6.AddEnemy("armored", 1, 2f);  // 7 coin - Ek tank
            wave6.CalculateTotalReward(enemyMoney);
            map.AddWave(wave6);

            // WAVE 7 - Elite giriş (52 coin)
            WaveData wave7 = new WaveData(7, 12f);
            wave7.AddEnemy("fast", 3, 1.2f);   // 15 coin
            wave7.AddEnemy("armored", 2, 2f);  // 14 coin
            wave7.AddEnemy("elite", 2, 2.5f);  // 20 coin
            wave7.AddEnemy("archer", 1, 1.5f); // 6 coin - Ek destek
            wave7.CalculateTotalReward(enemyMoney);
            map.AddWave(wave7);

            // WAVE 8 - Yoğunlaşma (60 coin)
            WaveData wave8 = new WaveData(8, 15f);
            wave8.AddEnemy("elite", 3, 2f);    // 30 coin
            wave8.AddEnemy("armored", 3, 1.8f);// 21 coin
            wave8.AddEnemy("fast", 2, 1.2f);   // 10 coin - Ek hızlı
            wave8.CalculateTotalReward(enemyMoney);
            map.AddWave(wave8);

            // WAVE 9 - Mini-boss wave (53 coin)
            WaveData wave9 = new WaveData(9, 15f);
            wave9.AddEnemy("mini-boss", 1, 3f);// 15 coin
            wave9.AddEnemy("elite", 2, 2f);    // 20 coin
            wave9.AddEnemy("archer", 3, 1.5f); // 18 coin
            wave9.CalculateTotalReward(enemyMoney);
            map.AddWave(wave9);

            // WAVE 10 - FINAL BOSS (50 coin)
            WaveData wave10 = new WaveData(10, 0f);
            wave10.AddEnemy("final-boss-grifon", 1, 0f); // 50 coin
            wave10.CalculateTotalReward(enemyMoney);
            map.AddWave(wave10);

            map.CalculateTotalMoney();
            return map;
        }
        #endregion

        #region HARITA 2 - KIRIN (ORTA)
        public MapData CreateKirinMap()
        {
            MapData map = new MapData(
                "Kirin'in Bahçeleri", 
                "General Aylin", 
                "Kirin", 
                2
            );
            map.startingMoney = 100;
            map.finalBossName = "Mistik Kirin";

            // WAVE 1 (35 coin)
            WaveData wave1 = new WaveData(1, 8f);
            wave1.AddEnemy("basic", 4, 1.5f);  // 20 coin
            wave1.AddEnemy("fast", 3, 1.2f);   // 15 coin
            wave1.CalculateTotalReward(enemyMoney);
            map.AddWave(wave1);

            // WAVE 2 (42 coin)
            WaveData wave2 = new WaveData(2, 8f);
            wave2.AddEnemy("armored", 3, 2f);  // 21 coin
            wave2.AddEnemy("basic", 4, 1.5f);  // 20 coin
            wave2.CalculateTotalReward(enemyMoney);
            map.AddWave(wave2);

            // WAVE 3 - Havadan saldırı (48 coin)
            WaveData wave3 = new WaveData(3, 10f);
            wave3.AddEnemy("flying", 3, 1.5f); // 24 coin (Havada uçan düşman)
            wave3.AddEnemy("archer", 4, 1.3f); // 24 coin
            wave3.CalculateTotalReward(enemyMoney);
            map.AddWave(wave3);

            // WAVE 4 (55 coin)
            WaveData wave4 = new WaveData(4, 10f);
            wave4.AddEnemy("elite", 2, 2.5f);  // 20 coin
            wave4.AddEnemy("fast", 4, 1.2f);   // 20 coin
            wave4.AddEnemy("armored", 2, 2f);  // 14 coin
            wave4.CalculateTotalReward(enemyMoney);
            map.AddWave(wave4);

            // WAVE 5 - Hava-yer kombinasyonu (60 coin)
            WaveData wave5 = new WaveData(5, 12f);
            wave5.AddEnemy("flying", 4, 1.5f); // 32 coin
            wave5.AddEnemy("elite", 2, 2f);    // 20 coin
            wave5.AddEnemy("archer", 1, 1.5f); // 6 coin
            wave5.CalculateTotalReward(enemyMoney);
            map.AddWave(wave5);

            // WAVE 6 (65 coin)
            WaveData wave6 = new WaveData(6, 12f);
            wave6.AddEnemy("elite", 4, 2f);    // 40 coin
            wave6.AddEnemy("armored", 3, 1.8f);// 21 coin
            wave6.AddEnemy("fast", 1, 1.2f);   // 5 coin
            wave6.CalculateTotalReward(enemyMoney);
            map.AddWave(wave6);

            // WAVE 7 - Mini-boss ile birlikte (58 coin)
            WaveData wave7 = new WaveData(7, 12f);
            wave7.AddEnemy("mini-boss", 1, 3f);// 15 coin
            wave7.AddEnemy("flying", 3, 1.5f); // 24 coin
            wave7.AddEnemy("elite", 2, 2f);    // 20 coin
            wave7.CalculateTotalReward(enemyMoney);
            map.AddWave(wave7);

            // WAVE 8 (72 coin)
            WaveData wave8 = new WaveData(8, 15f);
            wave8.AddEnemy("elite", 5, 1.8f);  // 50 coin
            wave8.AddEnemy("archer", 3, 1.5f); // 18 coin
            wave8.AddEnemy("armored", 1, 2f);  // 7 coin
            wave8.CalculateTotalReward(enemyMoney);
            map.AddWave(wave8);

            // WAVE 9 - Çift mini-boss (75 coin)
            WaveData wave9 = new WaveData(9, 15f);
            wave9.AddEnemy("mini-boss", 2, 3f);// 30 coin
            wave9.AddEnemy("elite", 3, 2f);    // 30 coin
            wave9.AddEnemy("flying", 2, 1.5f); // 16 coin
            wave9.CalculateTotalReward(enemyMoney);
            map.AddWave(wave9);

            // WAVE 10 - FINAL BOSS (60 coin)
            WaveData wave10 = new WaveData(10, 0f);
            wave10.AddEnemy("final-boss-kirin", 1, 0f); // 60 coin
            wave10.CalculateTotalReward(enemyMoney);
            map.AddWave(wave10);

            map.CalculateTotalMoney();
            return map;
        }
        #endregion

        #region HARITA 3 - EJDERHA (ZOR)
        public MapData CreateEjderhaMap()
        {
            MapData map = new MapData(
                "Ejderha'nın Kalesi", 
                "General Kaan", 
                "Ejderha", 
                3
            );
            map.startingMoney = 100;
            map.finalBossName = "Ateş Ejderhası";

            // WAVE 1 (48 coin)
            WaveData wave1 = new WaveData(1, 8f);
            wave1.AddEnemy("armored", 3, 2f);  // 21 coin
            wave1.AddEnemy("fast", 4, 1.2f);   // 20 coin
            wave1.AddEnemy("archer", 1, 1.5f); // 6 coin
            wave1.CalculateTotalReward(enemyMoney);
            map.AddWave(wave1);

            // WAVE 2 (60 coin)
            WaveData wave2 = new WaveData(2, 8f);
            wave2.AddEnemy("elite", 3, 2f);    // 30 coin
            wave2.AddEnemy("flying", 3, 1.5f); // 24 coin
            wave2.AddEnemy("armored", 1, 2f);  // 7 coin
            wave2.CalculateTotalReward(enemyMoney);
            map.AddWave(wave2);

            // WAVE 3 (68 coin)
            WaveData wave3 = new WaveData(3, 10f);
            wave3.AddEnemy("elite", 4, 2f);    // 40 coin
            wave3.AddEnemy("archer", 4, 1.5f); // 24 coin
            wave3.AddEnemy("armored", 1, 2f);  // 7 coin
            wave3.CalculateTotalReward(enemyMoney);
            map.AddWave(wave3);

            // WAVE 4 - Mini-boss erken giriş (75 coin)
            WaveData wave4 = new WaveData(4, 10f);
            wave4.AddEnemy("mini-boss", 1, 3f);// 15 coin
            wave4.AddEnemy("elite", 3, 2f);    // 30 coin
            wave4.AddEnemy("flying", 4, 1.5f); // 32 coin
            wave4.CalculateTotalReward(enemyMoney);
            map.AddWave(wave4);

            // WAVE 5 (80 coin)
            WaveData wave5 = new WaveData(5, 12f);
            wave5.AddEnemy("elite", 6, 1.8f);  // 60 coin
            wave5.AddEnemy("flying", 2, 1.5f); // 16 coin
            wave5.AddEnemy("armored", 1, 2f);  // 7 coin
            wave5.CalculateTotalReward(enemyMoney);
            map.AddWave(wave5);

            // WAVE 6 - Hava saldırısı (88 coin)
            WaveData wave6 = new WaveData(6, 12f);
            wave6.AddEnemy("flying", 7, 1.3f); // 56 coin
            wave6.AddEnemy("elite", 3, 2f);    // 30 coin
            wave6.AddEnemy("archer", 1, 1.5f); // 6 coin
            wave6.CalculateTotalReward(enemyMoney);
            map.AddWave(wave6);

            // WAVE 7 - Çift mini-boss (85 coin)
            WaveData wave7 = new WaveData(7, 12f);
            wave7.AddEnemy("mini-boss", 2, 3f);// 30 coin
            wave7.AddEnemy("elite", 4, 2f);    // 40 coin
            wave7.AddEnemy("flying", 2, 1.5f); // 16 coin
            wave7.CalculateTotalReward(enemyMoney);
            map.AddWave(wave7);

            // WAVE 8 (95 coin)
            WaveData wave8 = new WaveData(8, 15f);
            wave8.AddEnemy("elite", 7, 1.5f);  // 70 coin
            wave8.AddEnemy("armored", 3, 2f);  // 21 coin
            wave8.AddEnemy("archer", 1, 1.5f); // 6 coin
            wave8.CalculateTotalReward(enemyMoney);
            map.AddWave(wave8);

            // WAVE 9 - Üçlü mini-boss (101 coin)
            WaveData wave9 = new WaveData(9, 15f);
            wave9.AddEnemy("mini-boss", 3, 2.5f);// 45 coin
            wave9.AddEnemy("elite", 4, 2f);      // 40 coin
            wave9.AddEnemy("flying", 2, 1.5f);   // 16 coin
            wave9.CalculateTotalReward(enemyMoney);
            map.AddWave(wave9);

            // WAVE 10 - FINAL BOSS (80 coin)
            WaveData wave10 = new WaveData(10, 0f);
            wave10.AddEnemy("final-boss-ejderha", 1, 0f); // 80 coin
            wave10.CalculateTotalReward(enemyMoney);
            map.AddWave(wave10);

            map.CalculateTotalMoney();
            return map;
        }
        #endregion

        /// <summary>
        /// Tüm haritaları oluştur ve bilgilerini yazdır
        /// </summary>
        public void InitializeAllMaps()
        {
            Debug.Log("=== TOWER DEFENSE - WAVE KONFİGÜRASYONLARI ===\n");

            MapData grifonMap = CreateGrifonMap();
            grifonMap.PrintMapInfo();
            PrintWaveDetails(grifonMap);

            Debug.Log("\n" + new string('=', 50) + "\n");

            MapData kirinMap = CreateKirinMap();
            kirinMap.PrintMapInfo();
            PrintWaveDetails(kirinMap);

            Debug.Log("\n" + new string('=', 50) + "\n");

            MapData ejderhaMap = CreateEjderhaMap();
            ejderhaMap.PrintMapInfo();
            PrintWaveDetails(ejderhaMap);
        }

        /// <summary>
        /// Bir haritanın wave detaylarını yazdırır
        /// </summary>
        private void PrintWaveDetails(MapData map)
        {
            Debug.Log("\n--- Wave Detayları ---");
            foreach (var wave in map.waves)
            {
                Debug.Log($"Wave {wave.waveNumber}: {wave.totalMoneyReward} coin");
                foreach (var enemy in wave.enemies)
                {
                    Debug.Log($"  - {enemy.count}x {enemy.enemyType}");
                }
            }
        }
    }
}
