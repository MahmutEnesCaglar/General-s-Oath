using UnityEngine;

namespace TowerDefense.Enemy
{
    /// <summary>
    /// Fast Enemy - Hızlı ama kırılgan düşman
    /// HP: 30, Damage to Hero: 3, Damage to Barrier: 3
    /// Speed: 4.0, Reward: 7$
    /// </summary>
    public class FastEnemy : BaseEnemyRefactored
    {
        protected override void InitializeStats()
        {
            // Combat Stats
            maxHealth = 30;
            currentHealth = maxHealth;
            damageToHero = 3;
            damageToBarrier = 3;
            damageFromHero = 25;    // Hero'dan aldığı hasar (kırılgan, daha çok hasar alır)
            damageFromTower = 15;   // Tower'dan aldığı hasar (kırılgan)
            moveSpeed = 2f;  // En hızlı düşman
            moneyReward = 7;

            // Attack Settings
            attackCooldown = 1.2f;  // Hızlı saldırı
            aggroRange = 2.5f;

            // Properties
            isBoss = false;
            isRanged = false;

            Debug.Log($"<color=cyan>FastEnemy Initialized:</color> HP:{maxHealth} DMG:{damageToHero} SPD:{moveSpeed}");
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            // FastEnemy spawn'landığında hızlı koşu efekti eklenebilir
        }

        public override void OnDeath()
        {
            base.OnDeath();
            // Hızlı düşman öldüğünde özel animasyon olabilir
        }
    }
}
