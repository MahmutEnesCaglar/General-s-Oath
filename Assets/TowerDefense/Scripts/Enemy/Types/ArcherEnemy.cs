using UnityEngine;

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
            moveSpeed = 1.5f;
            moneyReward = 8;

            // Attack Settings
            attackCooldown = 2.0f;  // Yavaş ama uzun menzilli saldırı
            aggroRange = 4f;        // Uzun aggro range (archer için)
            archerRange = 3.5f;     // Uzaktan saldırı menzili

            // Properties
            isBoss = false;
            isRanged = true;  // ÖNEMLI: Archer ranged saldırı yapar

            Debug.Log($"<color=cyan>ArcherEnemy Initialized:</color> HP:{maxHealth} DMG:{damageToHero} SPD:{moveSpeed} RANGE:{archerRange}");
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            // Okçu spawn'landığında yay germe animasyonu olabilir
        }

        protected override void OnAttackPerformed()
        {
            base.OnAttackPerformed();
            // Okçu saldırısında ok fırlatma efekti/ses eklenebilir
            // Gelecekte: Projectile sistemi
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            // Okçu öldüğünde yayını düşürme efekti
        }

        // Gelecek için: Ok fırlatma sistemi
        // protected override void PerformRangedAttack()
        // {
        //     GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        //     arrow.GetComponent<Projectile>().Initialize(currentHeroTarget, damageToHero);
        // }
    }
}
