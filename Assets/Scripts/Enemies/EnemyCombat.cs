using UnityEngine;
using System.Collections;
using TowerDefense.Buildings;

namespace TowerDefense.Enemy.Components
{
    /// <summary>
    /// Handles enemy combat, attacks, and grid positioning
    /// </summary>
    [RequireComponent(typeof(BaseEnemyRefactored))]
    public class EnemyCombat : MonoBehaviour
    {
        private BaseEnemyRefactored enemy;
        private EnemyAnimation animationComponent;
        private EnemyMovement movementComponent;

        // Combat stats
        private int damageToHero;
        private int damageToBarrier;
        private bool isRanged;

        // Attack settings
        private float aggroRange = 2f;
        private float attackCooldown = 1.5f;
        private float attackTimer = 0f;
        private bool isAttacking = false;
        private float attackAnimationDuration = 0.8f;
        private float archerRange = 3.5f;
        private float meleReachOffset = 0.3f;

        // Grid system
        private float gridInnerDepth = 0.85f;
        private float gridOuterMultiplier = 1.3f;
        private float maxPathDeviation = 2.5f;

        // Targets
        private TowerDefense.Hero.Hero currentHeroTarget;
        private Barrier currentBarrierTarget;
        private bool isBlockedByBarrier = false;

        // Grid positioning
        private int heroAttackSlot = -1;
        private static int nextAttackSlot = 0;
        private static int maxInnerSlots = 6;
        private bool isInOuterGrid = false;

        // Hero bounds cache
        private float heroBoundsMinX, heroBoundsMaxX;
        private float heroBoundsMinY, heroBoundsMaxY;

        public bool IsAttacking => isAttacking;
        public TowerDefense.Hero.Hero CurrentHeroTarget => currentHeroTarget;
        public bool IsBlockedByBarrier => isBlockedByBarrier;

        private void Awake()
        {
            enemy = GetComponent<BaseEnemyRefactored>();
            animationComponent = GetComponent<EnemyAnimation>();
            movementComponent = GetComponent<EnemyMovement>();
        }

        public void Initialize(int dmgHero, int dmgBarrier, bool ranged, float atkCooldown, float aggro, float archerRng, float meleeReach)
        {
            damageToHero = dmgHero;
            damageToBarrier = dmgBarrier;
            isRanged = ranged;
            attackCooldown = atkCooldown;
            aggroRange = aggro;
            archerRange = archerRng;
            meleReachOffset = meleeReach;
        }

        public void SetGridSettings(float innerDepth, float outerMult, float pathDev)
        {
            gridInnerDepth = innerDepth;
            gridOuterMultiplier = outerMult;
            maxPathDeviation = pathDev;
        }

        public void CheckForHero()
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

                // DEBUG: Boss için hero detection
                BaseEnemyRefactored baseEnemy = GetComponent<BaseEnemyRefactored>();
                if (baseEnemy != null && baseEnemy.GetType().Name.Contains("Boss"))
                {
                    Debug.Log($"<color=cyan>[BOSS DEBUG]</color> CheckForHero: Distance={distanceToHero:F2}, AggroRange={aggroRange}, InRange={distanceToHero <= aggroRange}");
                }

                if (distanceToHero <= aggroRange)
                {
                    currentHeroTarget = hero;

                    if (baseEnemy != null && baseEnemy.GetType().Name.Contains("Boss"))
                    {
                        Debug.Log($"<color=green>[BOSS DEBUG]</color> Hero LOCKED as target!");
                    }
                }
            }
        }

        public void MoveTowardsHero()
        {
            if (currentHeroTarget == null) return;
            if (isAttacking) return;

            // DEBUG: Boss için movement tracking
            BaseEnemyRefactored baseEnemy = GetComponent<BaseEnemyRefactored>();
            bool isBoss = baseEnemy != null && baseEnemy.GetType().Name.Contains("Boss");

            // Path deviation check
            float distanceFromPath = Vector2.Distance(transform.position, movementComponent.LastWaypointPosition);
            float deviationMultiplier = CalculateDeviationMultiplier();

            if (distanceFromPath > maxPathDeviation * deviationMultiplier)
            {
                if (isBoss) Debug.Log($"<color=yellow>[BOSS DEBUG]</color> Lost hero target due to path deviation! Distance={distanceFromPath:F2}, Max={maxPathDeviation * deviationMultiplier:F2}");

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
            Vector2 movement = directionToTarget * movementComponent.GetMoveSpeed() + movementComponent.CalculateSeparation() * movementComponent.GetSeparationForce();

            if (distanceToTarget > 0.15f)
            {
                if (isBoss) Debug.Log($"<color=cyan>[BOSS DEBUG]</color> Moving towards hero. Distance to grid pos: {distanceToTarget:F2}");

                transform.position += (Vector3)movement * Time.deltaTime;
                if (animationComponent != null)
                    animationComponent.FlipSprite(movement.x);
            }
            else
            {
                if (isBoss) Debug.Log($"<color=green>[BOSS DEBUG]</color> Reached attack position! Calling AttackHero()");
                AttackHero();
            }
        }

        public void AttackHero()
        {
            if (currentHeroTarget == null || isAttacking) return;

            // DEBUG: Boss için attack attempt tracking
            BaseEnemyRefactored baseEnemy = GetComponent<BaseEnemyRefactored>();
            bool isBoss = baseEnemy != null && baseEnemy.GetType().Name.Contains("Boss");

            bool canAttack = CanAttackHero();
            if (isBoss)
            {
                float dist = Vector2.Distance(transform.position, currentHeroTarget.transform.position);
                Debug.Log($"<color=magenta>[BOSS DEBUG]</color> AttackHero called! CanAttack={canAttack}, Distance={dist:F2}, MeleReachOffset={meleReachOffset}, AttackTimer={attackTimer:F2}");
            }

            if (!canAttack) return;

            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
                return;
            }

            if (isBoss) Debug.Log($"<color=red>[BOSS DEBUG]</color> Starting attack coroutine!");
            StartCoroutine(PerformHeroAttack());
        }

        private IEnumerator PerformHeroAttack()
        {
            isAttacking = true;
            if (animationComponent != null)
                animationComponent.PlayAttackAnimation();

            yield return new WaitForSeconds(attackAnimationDuration * 0.5f);

            if (currentHeroTarget != null && !currentHeroTarget.isDead && CanAttackHero())
            {
                currentHeroTarget.TakeDamage(damageToHero);
                enemy.OnAttackPerformed();
            }

            yield return new WaitForSeconds(attackAnimationDuration * 0.5f);

            attackTimer = attackCooldown;
            isAttacking = false;
        }

        private bool CanAttackHero()
        {
            if (currentHeroTarget == null) return false;

            Vector2 heroPos = currentHeroTarget.transform.position;
            Vector2 enemyPos = transform.position;
            float distance = Vector2.Distance(enemyPos, heroPos);

            // Boss için basit mesafe kontrolü - grid bounds'a bağlı değil
            BaseEnemyRefactored baseEnemy = GetComponent<BaseEnemyRefactored>();
            if (baseEnemy != null && baseEnemy.GetType().Name.Contains("Boss"))
            {
                bool result = distance <= meleReachOffset;
                Debug.Log($"<color=orange>[BOSS DEBUG]</color> CanAttackHero: Distance={distance:F2}, MeleReachOffset={meleReachOffset}, Result={result}");
                return result;
            }

            if (isRanged)
            {
                return distance <= archerRange;
            }

            // Melee check with grid bounds (normal enemies)
            CalculateHeroBounds();
            float dx = Mathf.Abs(heroPos.x - enemyPos.x);
            float dy = Mathf.Abs(heroPos.y - enemyPos.y);

            float rangeX = (heroBoundsMaxX - heroBoundsMinX) / 2f;
            float rangeY = (heroBoundsMaxY - heroBoundsMinY) / 2f;

            float allowedX = rangeX * gridInnerDepth + meleReachOffset;
            float allowedY = rangeY * gridInnerDepth + meleReachOffset;

            return dx <= allowedX && dy <= allowedY;
        }

        public void HandleBarrierLogic()
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

        private IEnumerator PerformBarrierAttack()
        {
            isAttacking = true;
            if (animationComponent != null)
                animationComponent.PlayAttackAnimation();

            yield return new WaitForSeconds(attackAnimationDuration * 0.5f);

            if (currentBarrierTarget != null)
                currentBarrierTarget.TakeDamage(damageToBarrier);

            yield return new WaitForSeconds(attackAnimationDuration * 0.5f);

            attackTimer = attackCooldown;
            isAttacking = false;
        }

        public void OnTriggerEnter2D(Collider2D other)
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

        private void CalculateHeroBounds()
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

        private Vector2 GetGridPosition(Vector2 heroCenter, int slot, float depthMultiplier)
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

        private float CalculateDeviationMultiplier()
        {
            if (currentHeroTarget == null || currentHeroTarget.isDead) return 1.0f;

            float heroHealthPercentage = (float)currentHeroTarget.currentHealth / currentHeroTarget.maxHealth;

            if (heroHealthPercentage < 0.3f) return 1.8f;
            if (heroHealthPercentage < 0.6f) return 1.4f;
            return 1.0f;
        }

        private int CountEnemiesTargetingHero()
        {
            if (currentHeroTarget == null) return 0;

            int count = 0;
            BaseEnemyRefactored[] allEnemies = FindObjectsByType<BaseEnemyRefactored>(FindObjectsSortMode.None);

            foreach (BaseEnemyRefactored otherEnemy in allEnemies)
            {
                if (otherEnemy != null)
                {
                    EnemyCombat otherCombat = otherEnemy.GetComponent<EnemyCombat>();
                    if (otherCombat != null && otherCombat.currentHeroTarget == currentHeroTarget)
                        count++;
                }
            }

            return count;
        }

        public float GetAttackRange()
        {
            return isRanged ? archerRange : meleReachOffset;
        }
    }
}