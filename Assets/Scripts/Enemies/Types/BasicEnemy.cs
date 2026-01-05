using UnityEngine;

namespace TowerDefense.Enemy
{
    /// <summary>
    /// Basic Enemy - En temel düşman tipi
    /// HP: 50, Damage to Hero: 5, Damage to Barrier: 5
    /// Speed: 2.0, Reward: 5$
    /// </summary>
    public class BasicEnemy : BaseEnemyRefactored
    {
        protected override void InitializeStats()
        {
            // Combat Stats
            maxHealth = 50;
            currentHealth = maxHealth;
            damageToHero = 5;
            damageToBarrier = 5;
            damageFromHero = 20;    // Hero'dan aldığı hasar (normal)
            damageFromTower = 10;   // Tower'dan aldığı hasar (normal)
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

        public override void OnSpawn()
        {
            base.OnSpawn();
            // BasicEnemy spawn'landığında özel bir şey yapmaz
        }

        public override void OnDeath()
        {
            base.OnDeath();
            // BasicEnemy öldüğünde özel bir şey yapmaz
        }
    }
}
