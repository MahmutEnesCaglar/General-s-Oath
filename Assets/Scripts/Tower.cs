using UnityEngine;
using System.Collections.Generic;

public class Tower : MonoBehaviour
{
    [Header("Temel Ayarlar")]
    public string towerName = "Kule";
    public float range = 5f;
    public float fireRate = 1f;
    public int damage = 10;

    [Header("İzometrik Ayar")]
    [Range(0.1f, 1f)]
    public float verticalRangeModifier = 0.5f; // Y eksenini daraltan çarpan

    [Header("Hedefleme ve Liste")]
    public List<GameObject> enemiesInRange = new List<GameObject>();
    public GameObject currentTarget; 
    protected float fireCooldown = 0f;

    [Header("Mermi Ayarları")]
    public GameObject projectilePrefab; 
    // firePoint artık ofset sistemiyle kullanılıyor ama referans olarak durabilir

    protected RotatableTowerSprite rotatableVisual; // 'protected' yaptık ki Archer/Mortar da görsün

    protected virtual void Start()
    {
        rotatableVisual = GetComponentInChildren<RotatableTowerSprite>();
        
        // Inspector'dan CircleCollider2D'yi elle ayarlamak yerine kodla oval yapabiliriz
        UpdateColliderScale();
    }

    protected virtual void Update()
    {
        fireCooldown -= Time.deltaTime;
        
        // 1. Listeyi temizle ve en yakın/uygun hedefi seç
        UpdateTarget();

        if (currentTarget != null)
        {
            // 2. Hedefe dön
            if (rotatableVisual != null)
                rotatableVisual.RotateTowards(currentTarget.transform.position);

            // 3. Ateş et
            if (fireCooldown <= 0)
            {
                Attack();
                fireCooldown = 1f / fireRate;
            }
        }
    }

    protected virtual void UpdateTarget()
    {
        enemiesInRange.RemoveAll(item => item == null);

        // Eğer şu an bir hedefimiz yoksa ve listede düşman varsa, ilkini seç
        if (currentTarget == null && enemiesInRange.Count > 0)
        {
            currentTarget = enemiesInRange[0];
        }
    }

    // Listedeki düşmanlar arasından izometrik olarak en yakın olanı bulur
    private GameObject GetClosestEnemy()
    {
        GameObject bestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemiesInRange)
        {
            float dist = GetIsometricDistance(transform.position, enemy.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                bestTarget = enemy;
            }
        }
        return bestTarget;
    }

    // İzometrik mesafe hesabı (Oval Menzil)
    protected float GetIsometricDistance(Vector3 posA, Vector3 posB)
    {
        float diffX = posA.x - posB.x;
        float diffY = (posA.y - posB.y) / verticalRangeModifier; 
        return Mathf.Sqrt(diffX * diffX + diffY * diffY);
    }

    protected virtual void Attack()
    {
        if (projectilePrefab != null && currentTarget != null && rotatableVisual != null)
        {
            Vector2 offset = rotatableVisual.GetCurrentFirePointOffset(rotatableVisual.currentSegmentIndex);
            Vector3 spawnPos = transform.position + (Vector3)offset;

            GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            Projectile projScript = projObj.GetComponent<Projectile>();
            
            if (projScript != null)
                projScript.Setup(currentTarget, damage);
        }
    }

    // Fizik tetikleyicileri (Senin eski sistemin)
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!enemiesInRange.Contains(other.gameObject))
                enemiesInRange.Add(other.gameObject);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
            if (currentTarget == other.gameObject) currentTarget = null;
        }
    }

    // Collider'ı görsel olarak oval yapmak için (Opsiyonel)
    private void UpdateColliderScale()
    {
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col != null) col.radius = range;
    }

    // Editörde menzili görmeni sağlar
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        // Oval menzili çizmek için matrisi değiştiriyoruz
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, verticalRangeModifier, 1));
        Gizmos.DrawWireSphere(Vector3.zero, range);
        Gizmos.matrix = oldMatrix;
    }
}