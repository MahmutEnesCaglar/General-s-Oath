using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.Data
{
    /// <summary>
    /// Bir haritanın tüm bilgilerini tutar
    /// </summary>
    [System.Serializable]
    public class MapData
    {
        public string mapName;                    // Harita adı
        public string generalName;                // General adı
        public string mythicalCreature;           // Efsanevi yaratık (Grifon, Kirin, Ejderha)
        public int mapDifficulty;                 // Zorluk (1-3)
        public List<WaveData> waves;              // 10 wave verisi
        public string finalBossName;              // Final boss adı
        public int startingMoney;                 // Başlangıç parası
        public int totalPossibleMoney;            // Tüm düşmanları öldürerek kazanılabilecek toplam para

        public MapData(string mapName, string generalName, string mythicalCreature, int difficulty)
        {
            this.mapName = mapName;
            this.generalName = generalName;
            this.mythicalCreature = mythicalCreature;
            this.mapDifficulty = difficulty;
            this.waves = new List<WaveData>();
            this.startingMoney = 100; // Varsayılan
        }

        /// <summary>
        /// Haritaya wave ekler
        /// </summary>
        public void AddWave(WaveData wave)
        {
            waves.Add(wave);
        }

        /// <summary>
        /// Tüm haritadan kazanılabilecek toplam parayı hesaplar
        /// </summary>
        public void CalculateTotalMoney()
        {
            totalPossibleMoney = startingMoney;
            foreach (var wave in waves)
            {
                totalPossibleMoney += wave.totalMoneyReward;
            }
        }

        /// <summary>
        /// Harita bilgilerini yazdırır (Debug için)
        /// </summary>
        public void PrintMapInfo()
        {
            Debug.Log($"=== {mapName} ===");
            Debug.Log($"General: {generalName} ({mythicalCreature})");
            Debug.Log($"Zorluk: {mapDifficulty}/3");
            Debug.Log($"Başlangıç Parası: {startingMoney}");
            Debug.Log($"Toplam Kazanılabilir Para: {totalPossibleMoney}");
            Debug.Log($"Wave Sayısı: {waves.Count}");
        }
    }
}
