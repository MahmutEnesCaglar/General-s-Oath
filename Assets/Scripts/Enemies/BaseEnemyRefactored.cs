using UnityEngine;
using TowerDefense.Enemy.Components;
using TowerDefense.Core;

namespace TowerDefense.Enemy
{
    /// <summary>
    /// Base class for all enemy types - Refactored to use components
    /// Now acts as a coordinator between EnemyHealth, EnemyAnimation, EnemyMovement, and EnemyCombat
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(EnemyHealth))]
    [RequireComponent(typeof(EnemyAnimation))]
    [RequireComponent(typeof(EnemyMovement))]
    [RequireComponent(typeof(EnemyCombat))]
    public abstract class BaseEnemyRefactored : MonoBehaviour
    {
        [Header("Runtime Stats - Set by derived classes")]
        public int maxHealth;
        public int currentHealth;

        [Header("Combat Settings")]
        public float aggroRange = 2f;
        public float attackCooldown = 1.5f;

        [Header("Grid System")]
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

        // Stats set by derived classes
        protected int damageToHero;
        protected int damageToBarrier;
        protected float moveSpeed;
        protected int moneyReward;
        protected bool isBoss;
        protected bool isRanged;

        // Components
        protected EnemyHealth healthComponent;
        protected EnemyAnimation animationComponent;
        protected EnemyMovement movementComponent;
        protected EnemyCombat combatComponent;
        protected SpriteRenderer spriteRenderer;

        // ============= INITIALIZATION =============

        protected virtual void Awake()
        {
            // Get components
            healthComponent = GetComponent<EnemyHealth>();
            animationComponent = GetComponent<EnemyAnimation>();
            movementComponent = GetComponent<EnemyMovement>();
            combatComponent = GetComponent<EnemyCombat>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            // Setup Rigidbody2D
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;

            // Setup Collider
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col == null) col = gameObject.AddComponent<CircleCollider2D>();
            col.radius = 0.3f;
            col.isTrigger = false;
        }

        protected virtual void Start()
        {
            InitializeStats();
            InitializeComponents();
            movementComponent.FindWaypoints();
            UpdateSortingOrder();
            OnSpawn();
        }

        protected virtual void Update()
        {
            if (movementComponent.HasReachedBase) return;

            // Combat takes priority over movement
            if (combatComponent.IsBlockedByBarrier)
            {
                combatComponent.HandleBarrierLogic();
                animationComponent.UpdateAnimation(combatComponent.IsAttacking);
                UpdateSortingOrder();
                return;
            }

            // Check for hero
            combatComponent.CheckForHero();

            // Move towards hero or waypoint
            if (combatComponent.CurrentHeroTarget != null && !combatComponent.CurrentHeroTarget.isDead)
            {
                combatComponent.MoveTowardsHero();
            }
            else
            {
                movementComponent.MoveTowardsWaypoint();
            }

            UpdateSortingOrder();
            animationComponent.UpdateAnimation(combatComponent.IsAttacking);
        }

        protected virtual void LateUpdate()
        {
            animationComponent.UpdateLastPosition();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            combatComponent.OnTriggerEnter2D(other);
        }

        // ============= COMPONENT INITIALIZATION =============

        private void InitializeComponents()
        {
            healthComponent.Initialize(maxHealth);
            movementComponent.Initialize(moveSpeed, separationRadius, separationForce);
            combatComponent.Initialize(damageToHero, damageToBarrier, isRanged, attackCooldown, aggroRange, archerRange, meleReachOffset);
            combatComponent.SetGridSettings(gridInnerDepth, gridOuterMultiplier, maxPathDeviation);
        }

        // ============= ABSTRACT/VIRTUAL METHODS =============

        /// <summary>
        /// Each enemy type must implement this to set their stats
        /// </summary>
        protected abstract void InitializeStats();

        /// <summary>
        /// Called when enemy spawns
        /// </summary>
        public virtual void OnSpawn() { }

        /// <summary>
        /// Called when enemy dies
        /// </summary>
        public virtual void OnDeath() { }

        /// <summary>
        /// Called when enemy performs an attack
        /// </summary>
        public virtual void OnAttackPerformed() { }

        // ============= PUBLIC API =============

        public void SetWaypoints(Transform[] waypoints)
        {
            movementComponent.SetWaypoints(waypoints);
        }

        public void TakeDamage(int damage)
        {
            healthComponent.TakeDamage(damage);
        }

        public float GetAttackRange()
        {
            return combatComponent.GetAttackRange();
        }

        public Animator GetAnimator()
        {
            return animationComponent?.Animator;
        }

        public int GetMoneyReward()
        {
            return moneyReward;
        }

        // ============= SORTING ORDER =============

        protected virtual void UpdateSortingOrder()
        {
            SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>();
            int baseSortingOrder = SortingOrderConfig.GetCharacterSortingOrder(transform.position.y);

            foreach (SpriteRenderer sr in allSprites)
                sr.sortingOrder = baseSortingOrder;
        }
    }
}