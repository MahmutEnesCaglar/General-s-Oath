using UnityEngine;
using System.Collections;
using TowerDefense.Core;
using TowerDefense.Buildings;

namespace TowerDefense.Enemy
{
    /// <summary>
    /// Tüm düşman tiplerinin base class'ı
    /// Ortak combat, movement ve grid logic burada
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public abstract class BaseEnemy : MonoBehaviour
    {
        [Header("Runtime Stats - Derived class'larda ayarlanır")]
        public int maxHealth;
        public int currentHealth;
        protected int damageToHero;      // Hero'ya verilen hasar
        protected int damageToBarrier;   // Barrier'a verilen hasar
        protected int damageToOthers;    // Diğer düşmanlara verilen hasar (gelecek için)
        protected float moveSpeed;
        protected int moneyReward;
        protected bool isBoss;
        protected bool isRanged;

        [Header("Saldırı Sistemi")]
        public float aggroRange = 2f;
        public float attackCooldown = 1.5f;
        protected float attackTimer = 0f;
        protected bool isAttacking = false;
        protected float attackAnimationDuration = 0.8f;

        [Header("Combat Grid System")]
        [Range(0.7f, 0.95f)]
        public float gridInnerDepth = 0.85f;
        [Range(1.1f, 1.5f)]
        public float gridOuterMultiplier = 1.3f;
        public float meleReachOffset = 0.3f;
        public float archerRange = 3.5f;

        [Header("Collision Avoidance")]
        public float separationRadius = 0.4f;
        public float separationForce = 5f;
        public float maxPathDeviation = 2.5f;

        // Private fields
        protected Transform[] waypoints;
        protected int currentWaypointIndex = 0;
        protected bool hasReachedBase = false;
        protected bool isDead = false;  // Birden fazla ölüm çağrısını önlemek için

        protected TowerDefense.Hero.Hero currentHeroTarget;
        protected Barrier currentBarrierTarget;
        protected bool isBlockedByBarrier = false;

        protected int heroAttackSlot = -1;
        protected static int nextAttackSlot = 0;
        protected static int maxInnerSlots = 6;
        protected bool isInOuterGrid = false;
        protected Vector3 lastWaypointPosition;

        // Hero bounds cache
        protected float heroBoundsMinX, heroBoundsMaxX;
        protected float heroBoundsMinY, heroBoundsMaxY;

        // Components
        protected SpriteRenderer spriteRenderer;
        protected Animator animator;
        protected Vector3 lastPosition;

        // ============= INITIALIZATION =============

        protected virtual void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            animator = GetComponentInChildren<Animator>();

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;

            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col == null) col = gameObject.AddComponent<CircleCollider2D>();
            col.radius = 0.3f;
            col.isTrigger = false;
        }

        protected virtual void Start()
        {
            InitializeStats();  // Her derived class kendi stats'ını ayarlar
            FindWaypoints();
            UpdateSortingOrder();
            lastPosition = transform.position;

            if (waypoints != null && waypoints.Length > 0)
                lastWaypointPosition = waypoints[0].position;
            else
                lastWaypointPosition = transform.position;

            OnSpawn(); // Spawn callback
        }

        /// <summary>
        /// Her düşman tipinin kendi stats'larını ayarladığı abstract metod
        /// Derived class'larda override edilmeli
        /// </summary>
        protected abstract void InitializeStats();

        // ============= UPDATE LOOP =============

        protected virtual void Update()
        {
            if (hasReachedBase) return;

            // Bariyer öncelikli
            if (isBlockedByBarrier)
            {
                HandleBarrierLogic();
                UpdateAnimator();
                UpdateSortingOrder();
                return;
            }

            // Hero kontrolü
            CheckForHero();

            if (currentHeroTarget != null && !currentHeroTarget.isDead)
            {
                MoveTowardsHero();
            }
            else
            {
                if (currentHeroTarget != null)
                {
                    heroAttackSlot = -1;
                    currentHeroTarget = null;
                }
                MoveTowardsWaypoint();
            }

            UpdateSortingOrder();
            UpdateAnimator();
        }

        protected virtual void LateUpdate()
        {
            lastPosition = transform.position;
        }

        // ============= ABSTRACT/VIRTUAL METHODS =============

        /// <summary>
        /// Düşman spawn olduğunda çağrılır
        /// </summary>
        protected virtual void OnSpawn() { }

        /// <summary>
        /// Düşman öldüğünde çağrılır
        /// </summary>
        protected virtual void OnDeath() { }

        /// <summary>
        /// Saldırı yaparken çağrılır (özel efektler için)
        /// </summary>
        protected virtual void OnAttackPerformed() { }

        /// <summary>
        /// Attack range döndürür (archer için archerRange, melee için meleReachOffset)
        /// </summary>
        public virtual float GetAttackRange()
        {
            return isRanged ? archerRange : meleReachOffset;
        }

        // ============= MOVEMENT =============

        protected virtual void FindWaypoints()
        {
            GameObject pathParent = GameObject.Find("EnemyPath");
            if (pathParent != null)
            {
                int childCount = pathParent.transform.childCount;
                waypoints = new Transform[childCount];
                for (int i = 0; i < childCount; i++)
                    waypoints[i] = pathParent.transform.GetChild(i);
            }
        }

        /// <summary>
        /// Waypoint'leri dışarıdan atar (WaveManager tarafından kullanılır)
        /// </summary>
        public void SetWaypoints(Transform[] newWaypoints)
        {
            waypoints = newWaypoints;
            currentWaypointIndex = 0; // İlk waypoint'ten başla

            if (waypoints != null && waypoints.Length > 0)
            {
                lastWaypointPosition = waypoints[0].position;
            }
        }

        protected virtual void MoveTowardsWaypoint()
        {
            if (waypoints == null || waypoints.Length == 0) return;
            if (currentWaypointIndex >= waypoints.Length)
            {
                ReachBase();
                return;
            }

            Transform targetWaypoint = waypoints[currentWaypointIndex];
            lastWaypointPosition = targetWaypoint.position;

            Vector2 direction = (targetWaypoint.position - transform.position).normalized;
            Vector2 movement = direction * moveSpeed + CalculateSeparation() * separationForce;

            transform.position += (Vector3)movement * Time.deltaTime;

            float distanceToWaypoint = Vector2.Distance(transform.position, targetWaypoint.position);
            if (distanceToWaypoint < 0.1f)
                currentWaypointIndex++;

            FlipSprite(movement.x);
        }

        protected virtual void ReachBase()
        {
            if (hasReachedBase) return;
            hasReachedBase = true;
            if (GameManager.Instance != null)
                GameManager.Instance.OnEnemyReachedBase(1);
            Destroy(gameObject);
        }

        // ============= HERO TARGETING & COMBAT =============

        protected virtual void CheckForHero()
        {
            if (currentHeroTarget != null && !currentHeroTarget.isDead)
            {
                float distanceToHero = Vector2.Distance(transform.position, currentHeroTarget.transform.position);
                if (distanceToHero > aggroRange * 1.5f)
                {
                    currentHeroTarget = null;
                    heroAttackSlot = -1;
                }
                return;
            }

            TowerDefense.Hero.Hero hero = FindAnyObjectByType<TowerDefense.Hero.Hero>();
            if (hero != null && !hero.isDead)
            {
                float distanceToHero = Vector2.Distance(transform.position, hero.transform.position);
                if (distanceToHero <= aggroRange)
                {
                    currentHeroTarget = hero;
                }
            }
        }

        protected virtual void MoveTowardsHero()
        {
            if (currentHeroTarget == null) return;
            if (isAttacking) return;

            // Path deviation check
            float distanceFromPath = Vector2.Distance(transform.position, lastWaypointPosition);
            float deviationMultiplier = CalculateDeviationMultiplier();

            if (distanceFromPath > maxPathDeviation * deviationMultiplier)
            {
                currentHeroTarget = null;
                heroAttackSlot = -1;
                isInOuterGrid = false;
                return;
            }

            // Grid slot assignment
            if (heroAttackSlot == -1)
            {
                int totalEnemies = CountEnemiesTargetingHero();
                heroAttackSlot = nextAttackSlot;
                nextAttackSlot = (nextAttackSlot + 1) % maxInnerSlots;
                isInOuterGrid = totalEnemies > maxInnerSlots;
            }

            // Calculate grid position
            Vector2 heroPos = currentHeroTarget.transform.position;
            float depthMultiplier = isInOuterGrid ? gridOuterMultiplier : gridInnerDepth;
            Vector2 targetPosition = GetGridPosition(heroPos, heroAttackSlot, depthMultiplier);

            // Move towards target
            Vector2 directionToTarget = (targetPosition - (Vector2)transform.position).normalized;
            float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
            Vector2 movement = directionToTarget * moveSpeed + CalculateSeparation() * separationForce;

            if (distanceToTarget > 0.15f)
            {
                transform.position += (Vector3)movement * Time.deltaTime;
                FlipSprite(movement.x);
            }
            else
            {
                AttackHero();
            }
        }

        protected virtual void AttackHero()
        {
            if (currentHeroTarget == null || isAttacking) return;
            if (!CanAttackHero()) return;

            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
                return;
            }

            StartCoroutine(PerformHeroAttack());
        }

        protected virtual IEnumerator PerformHeroAttack()
        {
            isAttacking = true;
            if (animator != null) animator.Play("attack");

            yield return new WaitForSeconds(attackAnimationDuration * 0.5f);

            if (currentHeroTarget != null && !currentHeroTarget.isDead && CanAttackHero())
            {
                currentHeroTarget.TakeDamage(damageToHero);  // Hero'ya özel hasar
                OnAttackPerformed();
            }

            yield return new WaitForSeconds(attackAnimationDuration * 0.5f);

            attackTimer = attackCooldown;
            isAttacking = false;
        }

        protected virtual bool CanAttackHero()
        {
            if (currentHeroTarget == null) return false;

            Vector2 heroPos = currentHeroTarget.transform.position;
            Vector2 enemyPos = transform.position;
            float distance = Vector2.Distance(enemyPos, heroPos);

            // Ranged check (archer)
            if (isRanged)
            {
                return distance <= archerRange;
            }

            // Melee check with grid bounds
            CalculateHeroBounds();
            float dx = Mathf.Abs(heroPos.x - enemyPos.x);
            float dy = Mathf.Abs(heroPos.y - enemyPos.y);

            float rangeX = (heroBoundsMaxX - heroBoundsMinX) / 2f;
            float rangeY = (heroBoundsMaxY - heroBoundsMinY) / 2f;

            float allowedX = rangeX * gridInnerDepth + meleReachOffset;
            float allowedY = rangeY * gridInnerDepth + meleReachOffset;

            return dx <= allowedX && dy <= allowedY;
        }

        // ============= GRID SYSTEM =============

        protected virtual void CalculateHeroBounds()
        {
            if (currentHeroTarget == null) return;

            Vector2 heroPos = currentHeroTarget.transform.position;
            Vector2 tl = heroPos + currentHeroTarget.attackRangeTopLeft;
            Vector2 tr = heroPos + currentHeroTarget.attackRangeTopRight;
            Vector2 bl = heroPos + currentHeroTarget.attackRangeBottomLeft;
            Vector2 br = heroPos + currentHeroTarget.attackRangeBottomRight;

            heroBoundsMinX = Mathf.Min(tl.x, tr.x, bl.x, br.x);
            heroBoundsMaxX = Mathf.Max(tl.x, tr.x, bl.x, br.x);
            heroBoundsMinY = Mathf.Min(tl.y, tr.y, bl.y, br.y);
            heroBoundsMaxY = Mathf.Max(tl.y, tr.y, bl.y, br.y);
        }

        protected virtual Vector2 GetGridPosition(Vector2 heroCenter, int slot, float depthMultiplier)
        {
            CalculateHeroBounds();

            float rangeX = heroBoundsMaxX - heroBoundsMinX;
            float rangeY = heroBoundsMaxY - heroBoundsMinY;

            float adjustedRangeX = rangeX * depthMultiplier;
            float adjustedRangeY = rangeY * depthMultiplier;

            switch (slot)
            {
                case 0: return heroCenter + new Vector2(-adjustedRangeX / 2f, adjustedRangeY / 2f);
                case 1: return heroCenter + new Vector2(0, adjustedRangeY / 2f);
                case 2: return heroCenter + new Vector2(adjustedRangeX / 2f, adjustedRangeY / 2f);
                case 3: return heroCenter + new Vector2(adjustedRangeX / 2f, adjustedRangeY / 4f);
                case 4: return heroCenter + new Vector2(-adjustedRangeX / 2f, adjustedRangeY / 4f);
                case 5: return heroCenter + new Vector2(0, adjustedRangeY / 4f);
                default: return heroCenter;
            }
        }

        protected virtual float CalculateDeviationMultiplier()
        {
            if (currentHeroTarget == null || currentHeroTarget.isDead) return 1.0f;

            float heroHealthPercentage = (float)currentHeroTarget.currentHealth / currentHeroTarget.maxHealth;

            if (heroHealthPercentage < 0.3f) return 1.8f;
            if (heroHealthPercentage < 0.6f) return 1.4f;
            return 1.0f;
        }

        protected virtual int CountEnemiesTargetingHero()
        {
            if (currentHeroTarget == null) return 0;

            int count = 0;
            BaseEnemy[] allEnemies = FindObjectsByType<BaseEnemy>(FindObjectsSortMode.None);

            foreach (BaseEnemy enemy in allEnemies)
            {
                if (enemy != null && enemy.currentHeroTarget == currentHeroTarget)
                    count++;
            }

            return count;
        }

        // ============= BARRIER LOGIC =============

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Barrier"))
            {
                Barrier barrier = other.GetComponentInParent<Barrier>();
                if (barrier != null && barrier.currentHealth > 0)
                {
                    currentBarrierTarget = barrier;
                    isBlockedByBarrier = true;
                }
            }
        }

        protected virtual void HandleBarrierLogic()
        {
            if (currentBarrierTarget == null || currentBarrierTarget.currentHealth <= 0)
            {
                isBlockedByBarrier = false;
                currentBarrierTarget = null;
                return;
            }

            if (isAttacking) return;

            if (attackTimer > 0)
                attackTimer -= Time.deltaTime;
            else
                StartCoroutine(PerformBarrierAttack());
        }

        protected virtual IEnumerator PerformBarrierAttack()
        {
            isAttacking = true;
            if (animator != null) animator.Play("attack");

            yield return new WaitForSeconds(attackAnimationDuration * 0.5f);

            if (currentBarrierTarget != null)
                currentBarrierTarget.TakeDamage(damageToBarrier);  // Barrier'a özel hasar

            yield return new WaitForSeconds(attackAnimationDuration * 0.5f);

            attackTimer = attackCooldown;
            isAttacking = false;
        }

        // ============= HEALTH & DAMAGE =============

        public virtual void TakeDamage(int damageAmount)
        {
            // Zaten öldüyse hasar alma
            if (isDead) return;

            currentHealth -= damageAmount;
            StartCoroutine(DamageFlash());
            if (animator != null && currentHealth > 0) animator.Play("hurt");

            // Can 0 veya altına düştüyse öl
            if (currentHealth <= 0 && !isDead)
            {
                Die();
            }
        }

        protected virtual IEnumerator DamageFlash()
        {
            SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in allSprites) sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            foreach (SpriteRenderer sr in allSprites) sr.color = Color.white;
        }

        protected virtual void Die()
        {
            // Zaten öldüyse tekrar işlem yapma (double-death prevention)
            if (isDead) return;

            // Hemen flag'i set et ki birden fazla çağrı engellenir
            isDead = true;

            Debug.Log($"<color=red>[BaseEnemy] {gameObject.name} öldü! Reward: {moneyReward}</color>");

            OnDeath();

            // Notify GameManager for money reward
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnEnemyKilled(moneyReward);
            }

            // Notify WaveManager for wave progression
            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.OnEnemyKilled(gameObject);
            }

            // Death animation - Güvenli oynatma
            if (animator != null)
            {
                try
                {
                    // "die" state'i yoksa hata vermeden geç
                    animator.Play("die");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[BaseEnemy] Death animation bulunamadı: {e.Message}");
                }
            }

            // Collider'ı kapat (başka düşmanlar veya kuleler bu düşmanı hedef almasın)
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            // 1.5 saniye sonra objeyi yok et
            Destroy(gameObject, 1.5f);
        }

        // ============= UTILITY =============

        protected virtual Vector2 CalculateSeparation()
        {
            Vector2 separationVector = Vector2.zero;
            int nearbyEnemyCount = 0;
            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, separationRadius);

            foreach (Collider2D col in nearbyColliders)
            {
                if (col.gameObject != gameObject && col.GetComponent<BaseEnemy>() != null)
                {
                    Vector2 awayFromEnemy = (Vector2)transform.position - (Vector2)col.transform.position;
                    float distance = awayFromEnemy.magnitude;
                    if (distance > 0.01f)
                    {
                        separationVector += awayFromEnemy.normalized / distance;
                        nearbyEnemyCount++;
                    }
                }
            }

            if (nearbyEnemyCount > 0) separationVector /= nearbyEnemyCount;
            return separationVector;
        }

        protected virtual void FlipSprite(float directionX)
        {
            SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in allSprites)
            {
                if (directionX > 0) sr.flipX = true;
                else if (directionX < 0) sr.flipX = false;
            }
        }

        protected virtual void UpdateAnimator()
        {
            if (animator == null || isAttacking) return;

            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            bool isMoving = distanceMoved > 0.001f;

            if (isMoving)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (!stateInfo.IsName("walk")) animator.CrossFade("walk", 0.1f);
            }
            else
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (!stateInfo.IsName("idle")) animator.CrossFade("idle", 0.1f);
            }
        }

        protected virtual void UpdateSortingOrder()
        {
            SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>();
            int baseSortingOrder = SortingOrderConfig.GetCharacterSortingOrder(transform.position.y);

            foreach (SpriteRenderer sr in allSprites)
                sr.sortingOrder = baseSortingOrder;
        }
    }
}

