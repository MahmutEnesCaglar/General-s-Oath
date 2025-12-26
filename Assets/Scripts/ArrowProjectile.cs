using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public float speed = 15f;
    private GameObject target;

    public void Setup(GameObject _target, int _damage)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // 1. Hedefe Doğru Hareket Et
        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // 2. Okun Ucunu Hedefe Çevir (Rotasyon)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Eğer ok resmin sağa bakıyorsa (default), direkt açıyı uygula. 
        // Eğer yukarı bakıyorsa angle - 90f yapman gerekebilir.
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 3. Mesafe Kontrolü
        if (Vector2.Distance(transform.position, target.transform.position) < 0.2f)
        {
            Destroy(gameObject);
        }
    }
}