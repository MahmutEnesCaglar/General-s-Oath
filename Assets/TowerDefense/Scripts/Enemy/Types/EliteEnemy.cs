using UnityEngine;

namespace TowerDefense.Enemy
{
    /// <summary>
    /// Elite Enemy - Güçlü ve dengeli düşman
    /// HP: 90, Damage to Hero: 10, Damage to Barrier: 10
    /// Speed: 2.5, Reward: 22$
    /// Wave 7, 9, 10'da gelir. Wave 10'daki 10 Elite öldükten sonra Boss spawn'lanır
    /// </summary>
    public class EliteEnemy : BaseEnemy
    {
        protected override void InitializeStats()
        {
            // Combat Stats
            maxHealth = 90;
            currentHealth = maxHealth;
            damageToHero = 10;     // Yüksek hasar
            damageToBarrier = 10;
            damageToOthers = 10;
            moveSpeed = 2.5f;      // Hızlı
            moneyReward = 22;

            // Attack Settings
            attackCooldown = 1.0f;  // En hızlı saldırı
            aggroRange = 2.5f;

            // Properties
            isBoss = false;
            isRanged = false;

            Debug.Log($"<color=orange>EliteEnemy Initialized:</color> HP:{maxHealth} DMG:{damageToHero} SPD:{moveSpeed}");
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            // Elite spawn'landığında güçlü bir giriş efekti olabilir
            Debug.Log($"<color=orange>ELITE ENEMY SPAWNED!</color> HP: {currentHealth}");
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            // Elite öldüğünde WaveManager kontrol eder
            // Wave 10'da son Elite ise Boss dialog'u tetiklenir
            Debug.Log($"<color=orange>ELITE ENEMY DEFEATED!</color>");
        }

        protected override void OnAttackPerformed()
        {
            base.OnAttackPerformed();
            // Elite saldırısı daha gösterişli olabilir
        }
    }
}
