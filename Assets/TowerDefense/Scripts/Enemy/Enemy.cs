using UnityEngine;
using System.Collections;
using TowerDefense.Core;
using TowerDefense.Buildings; // Barrier scriptine erişim için gerekli

namespace TowerDefense.Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Enemy : MonoBehaviour
    {
        [Header("Düşman Tipi")]
        public string enemyType = "basic";

        [Header("İstatistikler")]
        public int maxHealth = 100;
        public int currentHealth;
        public int damage = 5;           // Üsse verdiği hasar
        public float moveSpeed = 2f;
        public int moneyReward = 5;
        public bool isFlying = false;
        public bool isBoss = false;

        [Header("Saldırı Sistemi (Hero & Bariyer)")]
        public float aggroRange = 3f;    
        public int damageToHero = 5;     
        public float attackCooldown = 1.5f; // Genel saldırı hızı
        private float attackTimer = 0f;
        private bool isAttacking = false; 
        private float attackAnimationDuration = 0.8f;

        // --- BARİYER DEĞİŞKENLERİ ---
        private Barrier currentBarrierTarget; 
        private bool isBlockedByBarrier = false;

        // --- HERO DEĞİŞKENLERİ ---
        private TowerDefense.Hero.Hero currentHeroTarget;
        
        [Header("Collision Avoidance")]
        public float separationRadius = 0.05f;
        public float separationForce = 2f;
        public float heroAttackRadius = 4f;
        private int heroAttackSlot = -1;
        private static int nextAttackSlot = 0;

        [Header("Hareket")]
        [HideInInspector]
        public Transform[] waypoints;
        private int currentWaypointIndex = 0;
        private bool hasReachedBase = false;

        [Header("Görsel")]
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private Vector3 lastPosition;

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            animator = GetComponentInChildren<Animator>();

            if (animator != null) animator.speed = 1f;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col == null) col = gameObject.AddComponent<CircleCollider2D>();
            col.radius = 0.3f;
            col.isTrigger = false; // Fiziksel çarpışma için false, ama bariyer Trigger olacak
        }

        private void Start()
        {
            currentHealth = maxHealth;

            if (animator != null) animator.Play("idle");

            if (waypoints == null || waypoints.Length == 0)
            {
                FindWaypoints();
            }

            UpdateSortingOrder();
            lastPosition = transform.position;
        }

        private void Update()
        {
            if (hasReachedBase) return;

            // 1. ÖNCE BARİYER KONTROLÜ
            // Eğer bir bariyer tarafından engellendiysek, hareket etme, sadece bariyere odaklan
            if (isBlockedByBarrier)
            {
                HandleBarrierLogic();
                UpdateAnimator();     // Dururken idle'a geçmesi için
                UpdateSortingOrder(); // Bariyerin önünde mi arkasında mı güncelle
                return; // <-- Hareketi burada kesiyoruz
            }

            // 2. HERO KONTROLÜ
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

        // --- BARİYER MANTIĞI ---

        /// <summary>
        /// Bariyer ile çarpışmayı algılar (Bariyer prefab'ında IsTrigger seçili olmalı!)
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Eğer "Barrier" tag'li bir şeye çarparsak
            if (other.CompareTag("Barrier"))
            {
                // Script bazen parent objede olabilir, garantiye alıyoruz
                Barrier barrier = other.GetComponentInParent<Barrier>();
                
                if (barrier != null && barrier.currentHealth > 0)
                {
                    currentBarrierTarget = barrier;
                    isBlockedByBarrier = true;
                    // Hızını hemen kesmek istersen:
                    // lastPosition = transform.position; 
                }
            }
        }

        /// <summary>
        /// Bariyer ile ilgili saldırı döngüsünü yönetir
        /// </summary>
        private void HandleBarrierLogic()
        {
            // Hedef bariyer yok olduysa veya yıkıldıysa
            if (currentBarrierTarget == null || currentBarrierTarget.currentHealth <= 0 || currentBarrierTarget.gameObject == null)
            {
                isBlockedByBarrier = false;
                currentBarrierTarget = null;
                return; // Bir sonraki Update'de harekete devam edecek
            }

            // Saldırı animasyonu oynuyorsa bekle
            if (isAttacking) return;

            // Cooldown sayacı
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }
            else
            {
                // Saldırıya başla
                StartCoroutine(PerformBarrierAttack());
            }
        }

        /// <summary>
        /// Bariyere saldırı coroutine'i
        /// </summary>
        private IEnumerator PerformBarrierAttack()
        {
            isAttacking = true;

            if (animator != null) animator.Play("attack");

            // Animasyonun vuruş anına denk gelmesi için bekle
            yield return new WaitForSeconds(attackAnimationDuration * 0.5f);

            // Bariyer hala orada mı kontrol et ve hasar ver
            if (currentBarrierTarget != null)
            {
                // Hasar ver
                currentBarrierTarget.TakeDamage(damage); 
            }

            // Animasyonun kalanını bekle
            yield return new WaitForSeconds(attackAnimationDuration * 0.5f);

            attackTimer = attackCooldown;
            isAttacking = false;
        }

        // --- MEVCUT HERO & HAREKET KODLARI ---

        private void CheckForHero()
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

        private void MoveTowardsHero()
        {
            if (currentHeroTarget == null) return;
            if (isAttacking) return;

            if (heroAttackSlot == -1)
            {
                heroAttackSlot = nextAttackSlot;
                nextAttackSlot = (nextAttackSlot + 1) % 8;
            }

            Vector2 heroPos = currentHeroTarget.transform.position;
            float angle = heroAttackSlot * 45f * Mathf.Deg2Rad;
            Vector2 targetPosition = heroPos + new Vector2(
                Mathf.Cos(angle) * heroAttackRadius,
                Mathf.Sin(angle) * heroAttackRadius
            );

            Vector2 directionToTarget = (targetPosition - (Vector2)transform.position).normalized;
            float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
            Vector2 movement = directionToTarget * moveSpeed + CalculateSeparation() * separationForce;

            if (distanceToTarget > 0.3f)
            {
                transform.position += (Vector3)movement * Time.deltaTime;
                FlipSprite(movement.x);
            }
            else
            {
                AttackHero();
            }
        }

        private void MoveTowardsWaypoint()
        {
            if (waypoints == null || waypoints.Length == 0) return;
            if (currentWaypointIndex >= waypoints.Length)
            {
                ReachBase();
                return;
            }

            Transform targetWaypoint = waypoints[currentWaypointIndex];
            Vector2 direction = (targetWaypoint.position - transform.position).normalized;
            Vector2 movement = direction * moveSpeed + CalculateSeparation() * separationForce;

            transform.position += (Vector3)movement * Time.deltaTime;

            float distanceToWaypoint = Vector2.Distance(transform.position, targetWaypoint.position);
            if (distanceToWaypoint < 0.1f)
            {
                currentWaypointIndex++;
            }

            FlipSprite(movement.x);
        }

        private void AttackHero()
        {
            if (currentHeroTarget == null || isAttacking) return;

            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
                return;
            }

            StartCoroutine(PerformHeroAttack());
        }

        private IEnumerator PerformHeroAttack()
        {
            isAttacking = true;
            if (animator != null) animator.Play("attack");

            yield return new WaitForSeconds(attackAnimationDuration * 0.5f);

            if (currentHeroTarget != null && !currentHeroTarget.isDead)
            {
                currentHeroTarget.TakeDamage(damageToHero);
            }

            yield return new WaitForSeconds(attackAnimationDuration * 0.5f);

            attackTimer = attackCooldown;
            isAttacking = false;
        }

        private Vector2 CalculateSeparation()
        {
            Vector2 separationVector = Vector2.zero;
            int nearbyEnemyCount = 0;
            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, separationRadius);

            foreach (Collider2D col in nearbyColliders)
            {
                if (col.gameObject != gameObject && col.GetComponent<Enemy>() != null)
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

        private void FlipSprite(float directionX)
        {
            SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in allSprites)
            {
                if (directionX > 0) sr.flipX = true;
                else if (directionX < 0) sr.flipX = false;
            }
        }

        private void UpdateAnimator()
        {
            if (animator == null || isAttacking) return;

            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            bool isMoving = distanceMoved > 0.001f;

            // Eğer bariyer tarafından durdurulduysa isMoving false olur, otomatik Idle'a geçer
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

        private void LateUpdate()
        {
            lastPosition = transform.position;
        }

        private void UpdateSortingOrder()
        {
            // Bariyer veya diğer objelerle derinlik algısı için
            SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>();
            // SortingOrderConfig sınıfını kullandığını varsayıyorum
            int baseSortingOrder = SortingOrderConfig.GetCharacterSortingOrder(transform.position.y);

            foreach (SpriteRenderer sr in allSprites)
            {
                sr.sortingOrder = baseSortingOrder;
            }
        }

        private void FindWaypoints()
        {
            GameObject pathParent = GameObject.Find("EnemyPath");
            if (pathParent != null)
            {
                int childCount = pathParent.transform.childCount;
                waypoints = new Transform[childCount];
                for (int i = 0; i < childCount; i++) waypoints[i] = pathParent.transform.GetChild(i);
            }
        }

        public void TakeDamage(int damageAmount)
        {
            currentHealth -= damageAmount;
            StartCoroutine(DamageFlash());
            if (animator != null && currentHealth > 0) animator.Play("hurt");
            if (currentHealth <= 0) Die();
        }

        private IEnumerator DamageFlash()
        {
            SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in allSprites) sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            foreach (SpriteRenderer sr in allSprites) sr.color = Color.white;
        }

        private void Die()
        {
            if (GameManager.Instance != null) GameManager.Instance.OnEnemyKilled(moneyReward);
            if (animator != null) animator.Play("die");
            
            // Collider'ı kapat ki öldükten sonra yolu tıkamasın
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
            
            Destroy(gameObject, 1.5f);
        }

        private void ReachBase()
        {
            if (hasReachedBase) return;
            hasReachedBase = true;
            if (GameManager.Instance != null) GameManager.Instance.OnEnemyReachedBase(1);
            Destroy(gameObject);
        }

        public void InitializeFromEnemyType(EnemyType enemyTypeData)
        {
            enemyType = enemyTypeData.type;
            maxHealth = enemyTypeData.hp;
            currentHealth = maxHealth;
            damage = enemyTypeData.damage;
            damageToHero = enemyTypeData.damage;
            moveSpeed = enemyTypeData.speed * 0.1f;
            moneyReward = enemyTypeData.money;
            isFlying = enemyTypeData.isFlying;
            isBoss = enemyTypeData.isBoss;

            if (isBoss)
            {
                transform.localScale = Vector3.one * 1.5f;
                aggroRange = 5f;
            }
            attackCooldown = 1.5f / (moveSpeed / 2f);
        }
    }
}