using UnityEngine;
using TowerDefense.Core;

namespace TowerDefense.Enemy.Components
{
    /// <summary>
    /// Handles enemy movement, waypoint navigation, and collision avoidance
    /// </summary>
    [RequireComponent(typeof(BaseEnemyRefactored))]
    public class EnemyMovement : MonoBehaviour
    {
        private BaseEnemyRefactored enemy;
        private EnemyAnimation animationComponent;

        // Movement settings
        private float moveSpeed;
        private float separationRadius = 0.4f;
        private float separationForce = 5f;

        // Waypoint navigation
        private Transform[] waypoints;
        private int currentWaypointIndex = 0;
        private bool hasReachedBase = false;

        public bool HasReachedBase => hasReachedBase;
        public Vector3 LastWaypointPosition { get; private set; }

        private void Awake()
        {
            enemy = GetComponent<BaseEnemyRefactored>();
            animationComponent = GetComponent<EnemyAnimation>();
        }

        public void Initialize(float speed, float sepRadius, float sepForce)
        {
            moveSpeed = speed;
            separationRadius = sepRadius;
            separationForce = sepForce;
        }

        public void SetWaypoints(Transform[] newWaypoints)
        {
            waypoints = newWaypoints;
            currentWaypointIndex = 0;

            if (waypoints != null && waypoints.Length > 0)
            {
                LastWaypointPosition = waypoints[0].position;
            }
            else
            {
                LastWaypointPosition = transform.position;
            }
        }

        public void FindWaypoints()
        {
            GameObject pathParent = GameObject.Find("EnemyPath");
            if (pathParent != null)
            {
                int childCount = pathParent.transform.childCount;
                waypoints = new Transform[childCount];
                for (int i = 0; i < childCount; i++)
                    waypoints[i] = pathParent.transform.GetChild(i);
            }

            if (waypoints != null && waypoints.Length > 0)
            {
                LastWaypointPosition = waypoints[0].position;
            }
        }

        public void MoveTowardsWaypoint()
        {
            if (waypoints == null || waypoints.Length == 0 || hasReachedBase) return;
            if (currentWaypointIndex >= waypoints.Length)
            {
                ReachBase();
                return;
            }

            Transform targetWaypoint = waypoints[currentWaypointIndex];
            LastWaypointPosition = targetWaypoint.position;

            Vector2 direction = (targetWaypoint.position - transform.position).normalized;
            Vector2 movement = direction * moveSpeed + CalculateSeparation() * separationForce;

            transform.position += (Vector3)movement * Time.deltaTime;

            float distanceToWaypoint = Vector2.Distance(transform.position, targetWaypoint.position);
            if (distanceToWaypoint < 0.1f)
                currentWaypointIndex++;

            if (animationComponent != null)
                animationComponent.FlipSprite(movement.x);
        }

        private void ReachBase()
        {
            if (hasReachedBase) return;
            hasReachedBase = true;

            if (GameManager.Instance != null)
                GameManager.Instance.OnEnemyReachedBase(1);

            Destroy(gameObject);
        }

        public Vector2 CalculateSeparation()
        {
            Vector2 separationVector = Vector2.zero;
            int nearbyEnemyCount = 0;
            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, separationRadius);

            foreach (Collider2D col in nearbyColliders)
            {
                if (col.gameObject != gameObject && col.GetComponent<BaseEnemyRefactored>() != null)
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

        public float GetMoveSpeed() => moveSpeed;
        public float GetSeparationForce() => separationForce;
    }
}