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

        [Header("Attack Range (4 Köşe Sistemi)")]
        [Tooltip("Saldırı menzili 4 köşe noktasıyla tanımlanır - her köşeyi ayrı ayarlayabilirsin!")]
        public Vector2 attackRangeTopLeft = new Vector2(-1.8f, 1.5f);      // Sol Üst
        public Vector2 attackRangeTopRight = new Vector2(1.8f, 1.5f);      // Sağ Üst
        public Vector2 attackRangeBottomLeft = new Vector2(-1.8f, -1.5f);  // Sol Alt
        public Vector2 attackRangeBottomRight = new Vector2(1.8f, -1.5f);  // Sağ Alt

        public float attackCooldown = 0.8f;

        [Header("Movement")]
        private Vector3 targetPosition;
        private bool isMoving = false;

        [Header("Combat")]
        private BaseEnemy currentTarget;
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
            // Don't attack while moving, but maintain target if still in range
            if (isMoving)
            {
                // Only clear target if we're far from combat
                if (currentTarget != null)
                {
                    // Box-based range check
                    if (!IsEnemyInAttackRange(currentTarget, 1.5f))
                    {
                        currentTarget = null;
                    }
                }
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
                if (!IsEnemyInAttackRange(currentTarget) || currentTarget == null)
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
        /// Düşmanın 4 köşe ile tanımlanan attack range içinde olup olmadığını kontrol eder
        /// </summary>
        private bool IsEnemyInAttackRange(BaseEnemy enemy, float multiplier = 1.0f)
        {
            if (enemy == null) return false;

            // 4 köşeyi hero pozisyonuna göre hesapla
            Vector2 heroPos = transform.position;
            Vector2 topLeft = heroPos + attackRangeTopLeft * multiplier;
            Vector2 topRight = heroPos + attackRangeTopRight * multiplier;
            Vector2 bottomLeft = heroPos + attackRangeBottomLeft * multiplier;
            Vector2 bottomRight = heroPos + attackRangeBottomRight * multiplier;

            // Min-Max bounding box hesapla
            float minX = Mathf.Min(topLeft.x, topRight.x, bottomLeft.x, bottomRight.x);
            float maxX = Mathf.Max(topLeft.x, topRight.x, bottomLeft.x, bottomRight.x);
            float minY = Mathf.Min(topLeft.y, topRight.y, bottomLeft.y, bottomRight.y);
            float maxY = Mathf.Max(topLeft.y, topRight.y, bottomLeft.y, bottomRight.y);

            Vector2 enemyPos = enemy.transform.position;

            // Düşman bounding box içinde mi?
            return enemyPos.x >= minX && enemyPos.x <= maxX &&
                   enemyPos.y >= minY && enemyPos.y <= maxY;
        }

        /// <summary>
        /// Finds nearest enemy within attack range (box-based)
        /// </summary>
        private void FindNearestEnemy()
        {
            BaseEnemy[] allEnemies = FindObjectsByType<BaseEnemy>(FindObjectsSortMode.None);
            BaseEnemy closestEnemy = null;
            float closestDistance = Mathf.Infinity;

            foreach (var enemy in allEnemies)
            {
                // Box-based range check
                if (IsEnemyInAttackRange(enemy))
                {
                    float distance = Vector2.Distance(transform.position, enemy.transform.position);
                    if (distance < closestDistance)
                    {
                        closestEnemy = enemy;
                        closestDistance = distance;
                    }
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

        // ============= DEBUG GÖRSELLEŞTIRME =============

        private void OnDrawGizmos()
        {
            if (isDead) return;

            // Attack Range - Kırmızı 4 köşeli şekil (her zaman görünür)
            Gizmos.color = new Color(1, 0, 0, 0.2f);
            DrawAttackRangeBox(transform.position, 1.0f, true);
        }

        private void OnDrawGizmosSelected()
        {
            if (isDead) return;

            // Hero seçili olduğunda detaylı bilgiler

            // 1. Attack Range - Kırmızı 4 köşeli şekil
            Gizmos.color = Color.red;
            DrawAttackRangeBox(transform.position, 1.0f, false);

            // 2. 4 köşe noktalarını göster
            Vector2 heroPos = transform.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(heroPos + attackRangeTopLeft, 0.15f);
            Gizmos.DrawWireSphere(heroPos + attackRangeTopRight, 0.15f);
            Gizmos.DrawWireSphere(heroPos + attackRangeBottomLeft, 0.15f);
            Gizmos.DrawWireSphere(heroPos + attackRangeBottomRight, 0.15f);

            // 3. Mevcut hedef varsa göster
            if (currentTarget != null)
            {
                // Hedefe çizgi
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, currentTarget.transform.position);

                // Hedef pozisyonu
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(currentTarget.transform.position, 0.3f);

                // Range içinde mi kontrol et
                bool inRange = IsEnemyInAttackRange(currentTarget);

                #if UNITY_EDITOR
                UnityEditor.Handles.Label(
                    transform.position + Vector3.up * 0.8f,
                    $"Target In Range: {(inRange ? "YES" : "NO")}\n" +
                    $"HP: {currentHealth}/{maxHealth}\n" +
                    $"4 Corners: TL={attackRangeTopLeft}, BR={attackRangeBottomRight}"
                );
                #endif
            }
            else
            {
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(
                    transform.position + Vector3.up * 0.8f,
                    $"No Target\n" +
                    $"HP: {currentHealth}/{maxHealth}\n" +
                    $"Moving: {isMoving}\n" +
                    $"4 Corners System Active"
                );
                #endif
            }

            // 4. Hareket hedefi varsa göster
            if (isMoving)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, targetPosition);
                Gizmos.DrawWireSphere(targetPosition, 0.2f);
            }
        }

        // Helper: 4 köşe ile attack range çiz
        private void DrawAttackRangeBox(Vector2 center, float multiplier, bool filled)
        {
            Vector3 topLeft = center + attackRangeTopLeft * multiplier;
            Vector3 topRight = center + attackRangeTopRight * multiplier;
            Vector3 bottomLeft = center + attackRangeBottomLeft * multiplier;
            Vector3 bottomRight = center + attackRangeBottomRight * multiplier;

            // Kenarları çiz
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);

            // İsteğe bağlı: dolu şekil (çapraz çizgiler)
            if (filled)
            {
                Gizmos.DrawLine(topLeft, bottomRight);
                Gizmos.DrawLine(topRight, bottomLeft);
            }
        }
    }

}
