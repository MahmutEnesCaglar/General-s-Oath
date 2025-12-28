using UnityEngine;
using TowerDefense.Core;
using TowerDefense.Enemy;
using TowerDefense.Tower; // Bunu eklemezsen BuildManager'ı bulamaz

namespace TowerDefense.Hero
{
    /// <summary>
    /// Hero character controller - General Altay (Hero Knight)
    /// Handles movement, combat, health, and integration with abilities
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class Hero : MonoBehaviour
    {
        [Header("Stats")]
        public int maxHealth = 150;
        public int currentHealth;
        public float moveSpeed = 3.5f;
        public int meleeDamage = 25;
        public float attackRange = 2.5f; // Increased from 1.5 for better combat
        public float attackCooldown = 0.8f;

        [Header("Movement")]
        private Vector3 targetPosition;
        private bool isMoving = false;

        [Header("Combat")]
        private TowerDefense.Enemy.Enemy currentTarget;
        private float attackTimer = 0f;
        private int attackComboIndex = 0; // 0,1,2 for Attack1/2/3

        [Header("Components")]
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D rb;
        public HeroAbilities abilities;

        [Header("Health Bar")]
        public GameObject healthBarPrefab;  // Health bar prefab'ı (Unity'de atanacak)
        private HeroHealthBar healthBarInstance;

        [Header("State")]
        public bool isDead = false;
        private bool isAttacking = false;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            // Setup rigidbody
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;

            // Initialize health
            currentHealth = maxHealth;

            // Add abilities component
            abilities = gameObject.AddComponent<HeroAbilities>();
            abilities.hero = this;

            // Create health bar
            if (healthBarPrefab != null)
            {
                GameObject healthBarObj = Instantiate(healthBarPrefab);
                healthBarInstance = healthBarObj.GetComponent<HeroHealthBar>();
                if (healthBarInstance != null)
                {
                    healthBarInstance.Initialize(transform);
                    healthBarInstance.UpdateHealthBar(currentHealth, maxHealth);
                }
            }

            // Initial sorting order
            UpdateSortingOrder();
        }

        private void Update()
        {
            if (isDead) return;

            UpdateMovement();
            UpdateCombat();
            UpdateAnimations();
            UpdateSortingOrder();
        }

        /// <summary>
        /// Called by HeroInput when player clicks
        /// </summary>
        public void SetDestination(Vector3 worldPos)
        {
            targetPosition = worldPos;
            targetPosition.z = 0;
            isMoving = true;
        }

        /// <summary>
        /// Handles hero movement to target position
        /// </summary>
        private void UpdateMovement()
        {
            if (!isMoving || isDead) return;

            Vector3 direction = (targetPosition - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetPosition);

            // Reached destination
            if (distance < 0.1f)
            {
                isMoving = false;
                return;
            }

            // Move toward target
            transform.position += direction * moveSpeed * Time.deltaTime;

            FlipSprite(direction.x);
        }

        /// <summary>
        /// Combat update - find and attack enemies
        /// </summary>
        private void UpdateCombat()
        {
            // Don't attack while moving to a destination
            if (isMoving)
            {
                currentTarget = null; // Clear target while moving
                return;
            }

            // Decrement attack timer
            if (attackTimer > 0)
                attackTimer -= Time.deltaTime;

            // Find target if none
            if (currentTarget == null)
            {
                FindNearestEnemy();
            }
            // Check if current target still valid
            else
            {
                float distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);
                if (distanceToTarget > attackRange || currentTarget == null)
                {
                    currentTarget = null;
                }
            }

            // Attack if target in range and cooldown ready
            if (currentTarget != null && attackTimer <= 0)
            {
                Attack();
            }
        }

        /// <summary>
        /// Finds nearest enemy within attack range
        /// </summary>
        private void FindNearestEnemy()
        {
            TowerDefense.Enemy.Enemy[] allEnemies = FindObjectsByType<TowerDefense.Enemy.Enemy>(FindObjectsSortMode.None);
            TowerDefense.Enemy.Enemy closestEnemy = null;
            float closestDistance = Mathf.Infinity;

            foreach (var enemy in allEnemies)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance <= attackRange && distance < closestDistance)
                {
                    closestEnemy = enemy;
                    closestDistance = distance;
                }
            }

            currentTarget = closestEnemy;
        }

        /// <summary>
        /// Attacks current target
        /// </summary>
        private void Attack()
        {
            if (currentTarget == null) return;

            // Stop moving to attack
            isMoving = false;

            // Face target
            float directionToTarget = currentTarget.transform.position.x - transform.position.x;
            FlipSprite(directionToTarget);

            // Trigger attack animation (combo system)
            string attackTrigger = "Attack" + (attackComboIndex + 1);
            animator.SetTrigger(attackTrigger);
            attackComboIndex = (attackComboIndex + 1) % 3; // Cycle 1->2->3->1

            // Deal damage
            currentTarget.TakeDamage(meleeDamage);

            // Reset cooldown
            attackTimer = attackCooldown;
        }

        /// <summary>
        /// Flips sprite based on movement direction
        /// Hero Knight faces RIGHT by default
        /// </summary>
        private void FlipSprite(float directionX)
        {
            if (directionX < 0)
                spriteRenderer.flipX = true;   // Face left
            else if (directionX > 0)
                spriteRenderer.flipX = false;  // Face right
        }

        /// <summary>
        /// Updates animator state based on movement
        /// </summary>
        private void UpdateAnimations()
        {
            if (animator == null || isDead) return;

            if (isAttacking) return; // Attack animations handled by triggers

            // Check actual movement velocity
            bool isActuallyMoving = isMoving && moveSpeed > 0.1f;

            // Movement animations
            if (isActuallyMoving)
            {
                animator.SetInteger("AnimState", 1); // Run
                animator.SetBool("Grounded", true); // Ensure grounded
            }
            else
            {
                animator.SetInteger("AnimState", 0); // Idle
                animator.SetBool("Grounded", true); // Ensure grounded
            }
        }

        /// <summary>
        /// Updates sorting order for isometric depth
        /// </summary>
        private void UpdateSortingOrder()
        {
            spriteRenderer.sortingOrder = SortingOrderConfig.GetCharacterSortingOrder(transform.position.y);
        }

        /// <summary>
        /// Hero takes damage
        /// </summary>
        public void TakeDamage(int damageAmount)
        {
            if (isDead || abilities == null) return;

            // Apply block reduction if active
            damageAmount = abilities.ApplyBlockReduction(damageAmount);

            currentHealth -= damageAmount;

            // Update health bar
            if (healthBarInstance != null)
            {
                healthBarInstance.UpdateHealthBar(currentHealth, maxHealth);
            }

            // Damage flash
            StartCoroutine(DamageFlash());

            // Hurt animation (optional)
            if (animator != null)
            {
                animator.SetTrigger("Hurt");
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Visual damage feedback
        /// </summary>
        private System.Collections.IEnumerator DamageFlash()
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = Color.white;
        }

        /// <summary>
        /// Hero death
        /// </summary>
        private void Die()
        {
            if (isDead) return;

            isDead = true;

            // Hide health bar
            if (healthBarInstance != null)
            {
                healthBarInstance.SetVisible(false);
            }

            // Death animation
            if (animator != null)
            {
                animator.SetBool("noBlood", true);
                animator.SetTrigger("Death");
            }

            // Notify GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnHeroKilled();
            }

            // Destroy after animation
            Destroy(gameObject, 2f);
        }
        /// <summary>
        /// Hero'yu maksimum canının belli bir yüzdesi kadar iyileştirir.
        /// AbilityManager'daki Heal butonu tarafından çağrılır.
        /// </summary>
        /// <param name="percentage">0.5f = %50 can doldurur</param>
        public void HealPercentage(float percentage)
        {
            if (isDead) return;

            // İyileşme miktarını hesapla
            int healAmount = Mathf.RoundToInt(maxHealth * percentage);
            currentHealth += healAmount;

            // Can maksimumu geçmesin
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

            // UI Barını Güncelle
            if (healthBarInstance != null)
            {
                healthBarInstance.UpdateHealthBar(currentHealth, maxHealth);
            }

            // Görsel Efekt (Yeşil Yanıp Sönme)
            if (spriteRenderer != null)
            {
                StopAllCoroutines(); // Kırmızı yanıyorsa durdur
                StartCoroutine(HealFlash());
            }

            Debug.Log($"<color=green>HERO HEALED!</color> +{healAmount} HP. (Current: {currentHealth}/{maxHealth})");
        }

        /// <summary>
        /// İyileşme görsel efekti (Yeşil Flash)
        /// </summary>
        private System.Collections.IEnumerator HealFlash()
        {
            spriteRenderer.color = Color.green; // Yeşil yap
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = Color.white; // Normale dön
        }
    }
    
}
