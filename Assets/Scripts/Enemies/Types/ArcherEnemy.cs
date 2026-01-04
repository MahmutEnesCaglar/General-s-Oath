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
    public class ArcherEnemy : BaseEnemy
    {
        protected override void InitializeStats()
        {
            // Combat Stats
            maxHealth = 50;
            currentHealth = maxHealth;
            damageToHero = 6;
            damageToBarrier = 6;
            damageToOthers = 6;
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

        protected override void OnSpawn()
        {
            base.OnSpawn();
        }

        // Override movement to stop at ranged attack distance
        protected override void MoveTowardsHero()
        {
            if (currentHeroTarget == null) return;
            if (isAttacking) return;

            // Check if in attack range
            float distanceToHero = Vector2.Distance(transform.position, currentHeroTarget.transform.position);

            // If within archer range, stop and attack
            if (distanceToHero <= archerRange)
            {
                AttackHero();
                return;
            }

            // Move closer to attack range
            Vector2 directionToHero = (currentHeroTarget.transform.position - transform.position).normalized;
            Vector2 movement = directionToHero * moveSpeed + CalculateSeparation() * separationForce;

            transform.position += (Vector3)movement * Time.deltaTime;
            FlipSprite(movement.x);
        }

        protected override void OnAttackPerformed()
        {
            base.OnAttackPerformed();
            // Future: Add arrow shooting effect/sound
        }

        protected override void OnDeath()
        {
            base.OnDeath();
        }
    }
}
