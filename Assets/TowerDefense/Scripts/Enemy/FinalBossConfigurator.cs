using UnityEngine;
using System.Collections.Generic;
using TowerDefense.Enemy;

namespace TowerDefense.Core
{
    /// <summary>
    /// Her haritanın final boss'unu tanımlar
    /// Final boss'lar efsanevi yaratıklardır ve çok güçlüdürler
    /// </summary>
    public class FinalBossConfigurator : MonoBehaviour
    {
        /// <summary>
        /// Tüm düşman tiplerini (final boss'lar dahil) döndürür
        /// </summary>
        public List<EnemyType> GetAllEnemyTypes()
        {
            List<EnemyType> enemyTypes = new List<EnemyType>
            {
                // Normal düşmanlar
                new EnemyType("basic", 75, 5, 10, 5, isFlying: false),
                new EnemyType("fast", 50, 7, 14, 5, isFlying: false),
                new EnemyType("armored", 110, 8, 7, 7, isFlying: false),
                new EnemyType("archer", 80, 9, 11, 6, isFlying: false),
                new EnemyType("flying", 60, 8, 12, 8, isFlying: true),      // Havada uçan düşman
                new EnemyType("elite", 150, 10, 12, 10, isFlying: false),
                new EnemyType("mini-boss", 200, 11, 9, 15, isFlying: false),

                // Final Boss'lar
                new EnemyType("final-boss-grifon", 500, 20, 8, 50, isFlying: true, isBoss: true),
                new EnemyType("final-boss-kirin", 750, 25, 9, 60, isFlying: false, isBoss: true),
                new EnemyType("final-boss-ejderha", 1000, 30, 10, 80, isFlying: true, isBoss: true)
            };

            return enemyTypes;
        }

        /// <summary>
        /// FINAL BOSS 1 - GRİFON
        /// Harita: Grifon'un Dağları
        /// 
        /// Özellikler:
        /// - HP: 500 (Orta güçlü)
        /// - Hasar: 20 (Yüksek)
        /// - Hız: 8 (Orta)
        /// - Ödül: 50 coin
        /// - Havada uçar (Universal tower gerekli!)
        /// 
        /// Strateji:
        /// - Mutlaka Universal Tower veya birden fazla Ground Tower gerekir
        /// - Oyuncunun bu noktada iyi bir savunma kurmuş olması beklenir
        /// </summary>
        public EnemyType CreateGrifonBoss()
        {
            return new EnemyType(
                type: "final-boss-grifon",
                hp: 500,
                damage: 20,
                speed: 8,
                money: 50,
                isFlying: true,
                isBoss: true
            );
        }

        /// <summary>
        /// FINAL BOSS 2 - KİRİN
        /// Harita: Kirin'in Bahçeleri
        /// 
        /// Özellikler:
        /// - HP: 750 (Güçlü)
        /// - Hasar: 25 (Çok yüksek)
        /// - Hız: 9 (Hızlı)
        /// - Ödül: 60 coin
        /// - Yerde yürür
        /// 
        /// Strateji:
        /// - Yüksek HP nedeniyle uzun süre dayanır
        /// - Birden fazla yükseltilmiş kule gerekir
        /// - AOE tower büyük hasar verebilir
        /// </summary>
        public EnemyType CreateKirinBoss()
        {
            return new EnemyType(
                type: "final-boss-kirin",
                hp: 750,
                damage: 25,
                speed: 9,
                money: 60,
                isFlying: false,
                isBoss: true
            );
        }

        /// <summary>
        /// FINAL BOSS 3 - EJDERHA
        /// Harita: Ejderha'nın Kalesi
        /// 
        /// Özellikler:
        /// - HP: 1000 (Çok güçlü!)
        /// - Hasar: 30 (Aşırı yüksek)
        /// - Hız: 10 (Çok hızlı)
        /// - Ödül: 80 coin
        /// - Havada uçar
        /// 
        /// Strateji:
        /// - Oyunun en zor boss'u
        /// - Tüm kuleler maksimum seviyede olmalı
        /// - Universal tower zorunlu (havada uçuyor)
        /// - Mükemmel kule yerleşimi gerekir
        /// </summary>
        public EnemyType CreateEjderhaBoss()
        {
            return new EnemyType(
                type: "final-boss-ejderha",
                hp: 1000,
                damage: 30,
                speed: 10,
                money: 80,
                isFlying: true,
                isBoss: true
            );
        }

        /// <summary>
        /// Tüm final boss bilgilerini yazdırır
        /// </summary>
        public void PrintAllBossInfo()
        {
            Debug.Log("=== FİNAL BOSS BİLGİLERİ ===\n");

            EnemyType grifonBoss = CreateGrifonBoss();
            PrintBossInfo("Yırtıcı Grifon", grifonBoss, 1);

            Debug.Log("\n" + new string('-', 50) + "\n");

            EnemyType kirinBoss = CreateKirinBoss();
            PrintBossInfo("Mistik Kirin", kirinBoss, 2);

            Debug.Log("\n" + new string('-', 50) + "\n");

            EnemyType ejderhaBoss = CreateEjderhaBoss();
            PrintBossInfo("Ateş Ejderhası", ejderhaBoss, 3);
        }

        /// <summary>
        /// Bir boss'un detaylı bilgilerini yazdırır
        /// </summary>
        private void PrintBossInfo(string name, EnemyType boss, int mapNumber)
        {
            Debug.Log($"=== {name} (Harita {mapNumber}) ===");
            Debug.Log($"Tip: {boss.type}");
            Debug.Log($"HP: {boss.hp}");
            Debug.Log($"Hasar: {boss.damage}");
            Debug.Log($"Hız: {boss.speed}");
            Debug.Log($"Ödül: {boss.money} coin");
            Debug.Log($"Uçuyor: {(boss.isFlying ? "Evet" : "Hayır")}");
        }
    }
}
