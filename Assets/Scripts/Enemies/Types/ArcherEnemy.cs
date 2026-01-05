using UnityEngine;
using System.Collections;

namespace TowerDefense.Enemy
{
    /// <summary>
    /// Archer Enemy - Uzaktan saldıran okçu düşman
    /// HP: 50, Damage to Hero: 6, Damage to Barrier: 6
    /// Speed: 1.5, Reward: 8$
    /// isRanged = true (archerRange 3.5f kullanır)
    /// </summary>
    public class ArcherEnemy : BaseEnemyRefactored
    {
        protected override void InitializeStats()
        {
            // Combat Stats
            maxHealth = 50;
            currentHealth = maxHealth;
            damageToHero = 6;
            damageToBarrier = 6;
            moveSpeed = 1f;
            moneyReward = 8;

            // Attack Settings
            attackCooldown = 2.0f;  // Yavaş ama uzun menzilli saldırı
            aggroRange = 4f;        // Uzun aggro range (archer için)
            archerRange = 3.5f;     // Uzaktan saldırı menzili

            // Properties
            isBoss = false;
            isRanged = true;  // ÖNEMLI: Archer ranged saldırı yapar
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
        }

        // Note: Archer-specific movement behavior (stopping at range) is now handled by EnemyCombat component
        // The isRanged flag and archerRange settings control the behavior automatically

        public override void OnAttackPerformed()
        {
            base.OnAttackPerformed();
            // Future: Add arrow shooting effect/sound
        }

        public override void OnDeath()
        {
            base.OnDeath();
        }
    }
}
