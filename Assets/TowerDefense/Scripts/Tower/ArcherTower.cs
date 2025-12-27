using TowerDefense.Tower;
using UnityEngine;

public class ArcherTower : Tower
{
    private ArcherRotation archerVisual;

    protected override void Start()
    {
        base.Start();
        archerVisual = GetComponentInChildren<ArcherRotation>();
    }

    protected override void Update()
    {
        base.Update();
        if (currentTarget != null && archerVisual != null)
        {
            archerVisual.RotateTowards(currentTarget.transform.position);
        }
    }
    protected override void Attack()
    {
        if (projectilePrefab != null && currentTarget != null)
        {
            // ArcherRotation'dan o anki yönün ofsetini al
            Vector2 offset = archerVisual.GetCurrentFirePointOffset(archerVisual.currentSegmentIndex);
            Vector3 spawnPos = transform.position + (Vector3)offset;

            GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            
            ArrowProjectile arrow = projObj.GetComponent<ArrowProjectile>();
            if (arrow != null)
            {
                arrow.Setup(currentTarget, damage);
            }
        }
    }
}