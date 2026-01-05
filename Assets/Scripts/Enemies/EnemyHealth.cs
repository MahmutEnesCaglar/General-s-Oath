using UnityEngine;
using System.Collections;
using TowerDefense.Core;

namespace TowerDefense.Enemy.Components
{
    /// <summary>
    /// Handles enemy health, damage, and death
    /// </summary>
    [RequireComponent(typeof(BaseEnemyRefactored))]
    public class EnemyHealth : MonoBehaviour
    {
        private BaseEnemyRefactored enemy;
        private SpriteRenderer[] allSprites;

        public int MaxHealth { get; private set; }
        public int CurrentHealth { get; private set; }
        public bool IsDead { get; private set; }

        private void Awake()
        {
            enemy = GetComponent<BaseEnemyRefactored>();
            allSprites = GetComponentsInChildren<SpriteRenderer>();
        }

        public void Initialize(int maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            IsDead = false;
        }

        public void TakeDamage(int damageAmount)
        {
            if (IsDead) return;

            CurrentHealth -= damageAmount;
            StartCoroutine(DamageFlash());

            Animator animator = enemy.GetAnimator();
            if (animator != null && CurrentHealth > 0)
            {
                animator.Play("hurt", -1, 0f);
            }

            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        private IEnumerator DamageFlash()
        {
            foreach (SpriteRenderer sr in allSprites)
                sr.color = Color.red;

            yield return new WaitForSeconds(0.1f);

            foreach (SpriteRenderer sr in allSprites)
                sr.color = Color.white;
        }

        private void Die()
        {
            if (IsDead) return;
            IsDead = true;

            enemy.OnDeath();

            // Notify GameManager for money reward
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnEnemyKilled(enemy.GetMoneyReward());
            }

            // Notify WaveManager for wave progression
            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.OnEnemyKilled(gameObject);
            }

            Animator animator = enemy.GetAnimator();
            if (animator != null)
            {
                animator.Play("die", -1, 0f);
            }

            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            Destroy(gameObject, 1.5f);
        }
    }
}