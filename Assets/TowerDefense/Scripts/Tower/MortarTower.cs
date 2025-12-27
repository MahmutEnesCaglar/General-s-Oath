using UnityEngine; // Namespace düzeltmesi
using TowerDefense.Tower;

public class MortarTower : Tower
{
    [Header("Mortar (Havan) Özel Ayarları")]
    public float explosionRadius = 1.5f; // Inspector'dan ayarlayacağın değer

    private MortarRotation mortarVisual;

    protected override void Start()
    {
        base.Start();
        mortarVisual = GetComponentInChildren<MortarRotation>();
    }

    protected override void Update()
    {
        base.Update();
        // Hedefe dönme işlemi
        if (currentTarget != null && mortarVisual != null)
        {
            mortarVisual.RotateTowards(currentTarget.transform.position);
        }
    }

    protected override void Attack()
    {
        if (projectilePrefab != null && currentTarget != null)
        {
            // Namlu ucu ayarı
            Vector3 spawnPos = transform.position;
            if (mortarVisual != null)
            {
                Vector2 offset = mortarVisual.GetCurrentFirePointOffset(mortarVisual.currentSegmentIndex);
                spawnPos = transform.position + (Vector3)offset;
            }

            GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            
            MortarProjectile projScript = projObj.GetComponent<MortarProjectile>();
            if (projScript != null)
            {
                // GÜNCELLEME BURADA:
                // setup fonksiyonuna kendi 'explosionRadius' değerimizi de gönderiyoruz.
                projScript.Setup(currentTarget, damage, explosionRadius);
            }
        }
    }
}