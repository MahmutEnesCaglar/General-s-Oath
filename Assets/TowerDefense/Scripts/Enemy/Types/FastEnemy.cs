using UnityEngine;

namespace TowerDefense.Enemy
{
    /// <summary>
    /// Fast Enemy - Hızlı ama kırılgan düşman
    /// HP: 30, Damage to Hero: 3, Damage to Barrier: 3
    /// Speed: 4.0, Reward: 9$
    /// </summary>
    public class FastEnemy : BaseEnemy
    {
        protected override void InitializeStats()
        {
            // Combat Stats
            maxHealth = 30;
            currentHealth = maxHealth;
            damageToHero = 3;
            damageToBarrier = 3;
            damageToOthers = 3;
            moveSpeed = 4.0f;  // En hızlı düşman
            moneyReward = 9;

            // Attack Settings
            attackCooldown = 1.2f;  // Hızlı saldırı
            aggroRange = 2.5f;

            // Properties
            isBoss = false;
            isRanged = false;

            Debug.Log($"<color=cyan>FastEnemy Initialized:</color> HP:{maxHealth} DMG:{damageToHero} SPD:{moveSpeed}");
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            // FastEnemy spawn'landığında hızlı koşu efekti eklenebilir
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            // Hızlı düşman öldüğünde özel animasyon olabilir
        }
    }
}
