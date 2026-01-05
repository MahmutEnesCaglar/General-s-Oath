using UnityEngine;

namespace TowerDefense.Enemy
{
    /// <summary>
    /// Elite Enemy - Güçlü ve dengeli düşman
    /// HP: 90, Damage to Hero: 10, Damage to Barrier: 10
    /// Speed: 2.5, Reward: 15$
    /// Wave 7, 9, 10'da gelir. Wave 10'daki 10 Elite öldükten sonra Boss spawn'lanır
    /// </summary>
    public class EliteEnemy : BaseEnemyRefactored
    {
        protected override void InitializeStats()
        {
            // Combat Stats
            maxHealth = 90;
            currentHealth = maxHealth;
            damageToHero = 10;     // Yüksek hasar
            damageToBarrier = 10;
            damageFromHero = 18;   // Hero'dan aldığı hasar (biraz dirençli)
            damageFromTower = 9;   // Tower'dan aldığı hasar (biraz dirençli)
            moveSpeed = 1.5f;      // Hızlı
            moneyReward = 15;

            // Attack Settings
            attackCooldown = 1.0f;  // En hızlı saldırı
            aggroRange = 2.5f;

            // Properties
            isBoss = false;
            isRanged = false;

            Debug.Log($"<color=orange>EliteEnemy Initialized:</color> HP:{maxHealth} DMG:{damageToHero} SPD:{moveSpeed}");
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            // Elite spawn'landığında güçlü bir giriş efekti olabilir
            Debug.Log($"<color=orange>ELITE ENEMY SPAWNED!</color> HP: {currentHealth}");
        }

        public override void OnDeath()
        {
            base.OnDeath();
            // Elite öldüğünde WaveManager kontrol eder
            // Wave 10'da son Elite ise Boss dialog'u tetiklenir
            Debug.Log($"<color=orange>ELITE ENEMY DEFEATED!</color>");
        }

        public override void OnAttackPerformed()
        {
            base.OnAttackPerformed();
            // Elite saldırısı daha gösterişli olabilir
        }
    }
}
