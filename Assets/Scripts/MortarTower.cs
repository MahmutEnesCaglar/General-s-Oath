using UnityEngine;

public class MortarTower : Tower
{
    [Header("Mortar (Havan) Özel Ayarları")]
    public float explosionRadius = 1.5f; // İşte beklediğin alan hasarı parametresi!

    private MortarRotation mortarVisual;

    protected override void Start()
    {
        base.Start();
        mortarVisual = GetComponentInChildren<MortarRotation>();
    }

    protected override void Update()
    {
        base.Update();

        if (currentTarget != null && mortarVisual != null)
        {
            mortarVisual.RotateTowards(currentTarget.transform.position);
        }
    }

    protected override void Attack()
    {
        if (projectilePrefab != null && currentTarget != null)
        {
            // MortarRotation'dan o anki yönün offsetini al
            Vector2 offset = mortarVisual.GetCurrentFirePointOffset(mortarVisual.currentSegmentIndex);
            Vector3 spawnPos = transform.position + (Vector3)offset;

            GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            
            // Dikkat: Burada MortarProjectile scriptini çağırıyoruz
            MortarProjectile projScript = projObj.GetComponent<MortarProjectile>();
            if (projScript != null)
            {
                projScript.Setup(currentTarget, damage);
            }
        }
    }
}