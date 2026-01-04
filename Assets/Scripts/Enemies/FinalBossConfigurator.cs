using UnityEngine;

namespace TowerDefense.Core
{
    /// <summary>
    /// Final boss configuration and debug information
    /// Simplified version - actual boss stats are defined in BossEnemy.cs
    /// </summary>
    public class FinalBossConfigurator : MonoBehaviour
    {
        /// <summary>
        /// Prints all final boss information (for debugging)
        /// </summary>
        public void PrintAllBossInfo()
        {
            Debug.Log("=== FINAL BOSS BİLGİLERİ ===\n");

            Debug.Log("=== Evil Wizard (Grifon Haritası) ===");
            Debug.Log("Tip: final-boss-grifon");
            Debug.Log("HP: 500");
            Debug.Log("Hasar: 20");
            Debug.Log("Hız: 8");
            Debug.Log("Ödül: 50 coin");
            Debug.Log("Uçuyor: Evet");

            Debug.Log("\n" + new string('-', 50) + "\n");

            Debug.Log("=== Bringer of Death (Kirin Haritası) ===");
            Debug.Log("Tip: final-boss-kirin");
            Debug.Log("HP: 750");
            Debug.Log("Hasar: 25");
            Debug.Log("Hız: 9");
            Debug.Log("Ödül: 60 coin");
            Debug.Log("Uçuyor: Hayır");

            Debug.Log("\n" + new string('-', 50) + "\n");

            Debug.Log("=== The Devourer (Ejderha Haritası) ===");
            Debug.Log("Tip: final-boss-ejderha");
            Debug.Log("HP: 1000");
            Debug.Log("Hasar: 30");
            Debug.Log("Hız: 10");
            Debug.Log("Ödül: 80 coin");
            Debug.Log("Uçuyor: Evet");
        }
    }
}
