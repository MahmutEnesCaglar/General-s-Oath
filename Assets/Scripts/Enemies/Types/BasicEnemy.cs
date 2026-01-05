using UnityEngine;

namespace TowerDefense.Enemy
{
    /// <summary>
    /// Basic Enemy - En temel düşman tipi
    /// HP: 50, Damage to Hero: 5, Damage to Barrier: 5
    /// Speed: 2.0, Reward: 5$
    /// </summary>
    public class BasicEnemy : BaseEnemy
    {
        protected override void InitializeStats()
        {
            // Combat Stats
            maxHealth = 50;
            currentHealth = maxHealth;
            damageToHero = 5;
            damageToBarrier = 5;
            damageToOthers = 5;  // Gelecekte diğer düşmanlara saldırı için
            moveSpeed = 1.2f;
            moneyReward = 5;

            // Attack Settings
            attackCooldown = 1.5f;
            aggroRange = 2f;

            // Properties
            isBoss = false;
            isRanged = false;

            Debug.Log($"<color=cyan>BasicEnemy Initialized:</color> HP:{maxHealth} DMG:{damageToHero} SPD:{moveSpeed}");
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            // BasicEnemy spawn'landığında özel bir şey yapmaz
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            // BasicEnemy öldüğünde özel bir şey yapmaz
        }
    }
}
