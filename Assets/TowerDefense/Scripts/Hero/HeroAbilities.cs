using UnityEngine;
using TowerDefense.Enemy;

namespace TowerDefense.Hero
{
    /// <summary>
    /// Manages hero special abilities: Roll attack and Block
    /// Handles cooldowns, damage calculations, and animation triggers
    /// </summary>
    public class HeroAbilities : MonoBehaviour
    {
        [Header("Special Ability - Roll Attack")]
        public float specialAbilityCooldown = 5f;
        public int specialAbilityDamage = 50;
        public float rollDistance = 3f;
        public float rollDuration = 0.57f; // 8 frames @ 14 FPS from Hero Knight demo
        private float specialAbilityTimer = 0f;
        private bool isRolling = false;
        private float rollCurrentTime = 0f;

        [Header("Block System")]
        public float blockDamageReduction = 0.5f; // 50% damage reduction
        public bool isBlocking = false;
        private bool wasBlockingLastFrame = false;

        [Header("References")]
        public Hero hero;
        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();

            if (animator == null)
            {
                Debug.LogError("HeroAbilities: Animator not found!");
            }
        }

        private void Update()
        {
            // Cooldown tick
            if (specialAbilityTimer > 0)
            {
                specialAbilityTimer -= Time.deltaTime;
            }

            // Roll duration check
            if (isRolling)
            {
                rollCurrentTime += Time.deltaTime;
                if (rollCurrentTime >= rollDuration)
                {
                    isRolling = false;
                    DealRollDamage();
                }
            }
        }

        /// <summary>
        /// Activates special ability (Roll attack)
        /// Q key triggers this from HeroInput
        /// </summary>
        public void ActivateSpecialAbility()
        {
            if (specialAbilityTimer > 0)
            {
                Debug.Log($"Special ability on cooldown! {specialAbilityTimer:F1}s remaining");
                return;
            }

            if (hero == null || hero.isDead)
            {
                return;
            }

            if (isRolling)
            {
                Debug.Log("Already rolling!");
                return;
            }

            // Start roll
            isRolling = true;
            rollCurrentTime = 0f;
            specialAbilityTimer = specialAbilityCooldown;

            // Trigger roll animation
            if (animator != null)
            {
                animator.SetTrigger("Roll");
            }

            // Move hero forward during roll
            SpriteRenderer spriteRenderer = hero.GetComponent<SpriteRenderer>();
            Vector3 rollDirection = (spriteRenderer != null && spriteRenderer.flipX) ? Vector3.left : Vector3.right;
            hero.transform.position += rollDirection * rollDistance;

            Debug.Log($"Hero uses Roll attack! Cooldown: {specialAbilityCooldown}s");
        }

        /// <summary>
        /// Deals AOE damage at roll endpoint
        /// </summary>
        private void DealRollDamage()
        {
            // AOE damage at current position
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2f);
            int enemiesHit = 0;

            foreach (var col in hits)
            {
                BaseEnemy enemy = col.GetComponent<BaseEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(specialAbilityDamage);
                    enemiesHit++;
                }
            }

            if (enemiesHit > 0)
            {
                Debug.Log($"Roll attack hit {enemiesHit} enemies for {specialAbilityDamage} damage each!");
            }
            else
            {
                Debug.Log("Roll attack hit no enemies");
            }
        }

        /// <summary>
        /// Activates or deactivates block
        /// RMB or B key controls this from HeroInput
        /// </summary>
        public void ActivateBlock(bool active)
        {
            isBlocking = active;

            if (animator == null) return;

            // Block started
            if (active && !wasBlockingLastFrame)
            {
                animator.SetBool("IdleBlock", true);
                animator.SetTrigger("Block");
                Debug.Log("Hero is blocking! Damage reduced by 50%");
            }
            // Block ended
            else if (!active && wasBlockingLastFrame)
            {
                animator.SetBool("IdleBlock", false);
                Debug.Log("Hero stopped blocking");
            }

            wasBlockingLastFrame = active;
        }

        /// <summary>
        /// Applies block damage reduction if active
        /// Called from Hero.TakeDamage()
        /// </summary>
        public int ApplyBlockReduction(int incomingDamage)
        {
            if (isBlocking)
            {
                int reducedDamage = Mathf.RoundToInt(incomingDamage * (1f - blockDamageReduction));
                Debug.Log($"Block active! Damage reduced: {incomingDamage} -> {reducedDamage}");
                return reducedDamage;
            }
            return incomingDamage;
        }

        /// <summary>
        /// Checks if special ability is ready to use
        /// </summary>
        public bool IsSpecialAbilityReady()
        {
            return specialAbilityTimer <= 0;
        }

        /// <summary>
        /// Gets cooldown progress (0 to 1, where 1 = ready)
        /// For UI radial cooldown display
        /// </summary>
        public float GetSpecialAbilityCooldownPercent()
        {
            return 1f - (specialAbilityTimer / specialAbilityCooldown);
        }

        /// <summary>
        /// Gets remaining cooldown time in seconds
        /// </summary>
        public float GetRemainingCooldown()
        {
            return Mathf.Max(0f, specialAbilityTimer);
        }
    }
}
