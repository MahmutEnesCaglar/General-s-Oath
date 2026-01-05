using UnityEngine;

namespace TowerDefense.Enemy
{
    /// <summary>
    /// Armored Enemy - Ağır zırhlı, yavaş ama dayanıklı düşman
    /// HP: 110, Damage to Hero: 8, Damage to Barrier: 8
    /// Speed: 1.0, Reward: 10$
    /// </summary>
    public class ArmoredEnemy : BaseEnemyRefactored
    {
        protected override void InitializeStats()
        {
            // Combat Stats
            maxHealth = 110;  // En yüksek HP (Boss hariç)
            currentHealth = maxHealth;
            damageToHero = 8;
            damageToBarrier = 8;
            moveSpeed = 0.7f;  // En yavaş düşman
            moneyReward = 10;

            // Attack Settings
            attackCooldown = 2.0f;  // Yavaş saldırı
            aggroRange = 1.5f;      // Kısa aggro range

            // Properties
            isBoss = false;
            isRanged = false;

            Debug.Log($"<color=cyan>ArmoredEnemy Initialized:</color> HP:{maxHealth} DMG:{damageToHero} SPD:{moveSpeed}");
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            // ArmoredEnemy spawn'landığında ağır adım sesi çalınabilir
        }

        public override void OnDeath()
        {
            base.OnDeath();
            // Zırhlı düşman öldüğünde farklı ölüm efekti olabilir
        }

        // Gelecek için: Zırh damage reduction sistemi
        // public override void TakeDamage(int damageAmount)
        // {
        //     int reducedDamage = Mathf.Max(1, damageAmount - armorValue);
        //     base.TakeDamage(reducedDamage);
        // }
    }
}
