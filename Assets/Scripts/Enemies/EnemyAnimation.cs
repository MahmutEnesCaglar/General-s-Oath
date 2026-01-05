using UnityEngine;

namespace TowerDefense.Enemy.Components
{
    /// <summary>
    /// Handles enemy animation state management
    /// </summary>
    [RequireComponent(typeof(BaseEnemyRefactored))]
    public class EnemyAnimation : MonoBehaviour
    {
        private Animator animator;
        private Vector3 lastPosition;
        private bool isAttacking;

        public Animator Animator => animator;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            lastPosition = transform.position;
        }

        public void UpdateAnimation(bool isAttackingFlag)
        {
            if (animator == null) return;

            isAttacking = isAttackingFlag;
            if (isAttacking) return;

            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            bool isMoving = distanceMoved > 0.001f;

            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

            if (isMoving)
            {
                if (!currentState.IsName("walk"))
                    animator.Play("walk", -1, 0f);
            }
            else
            {
                if (!currentState.IsName("idle"))
                    animator.Play("idle", -1, 0f);
            }
        }

        public void PlayAttackAnimation()
        {
            if (animator != null)
                animator.Play("attack", -1, 0f);
        }

        public void PlayHurtAnimation()
        {
            if (animator != null)
                animator.Play("hurt", -1, 0f);
        }

        public void PlayDieAnimation()
        {
            if (animator != null)
                animator.Play("die", -1, 0f);
        }

        public void UpdateLastPosition()
        {
            lastPosition = transform.position;
        }

        public void FlipSprite(float directionX)
        {
            SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in allSprites)
            {
                if (directionX > 0) sr.flipX = true;
                else if (directionX < 0) sr.flipX = false;
            }
        }
    }
}
