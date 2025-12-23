using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    private GameObject target;

    // Hasar parametresini koddan kaldırmadım ama kullanmıyoruz, 
    // böylece Tower.cs tarafını bozmamış oluruz.
    public void Setup(GameObject _target, int _damage)
    {
        target = _target;
    }

    void Update()
    {
        // Eğer hedef bir şekilde sahneden silinirse mermiyi de sil
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Hedefe doğru yönel ve ilerle
        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Hedefe ulaştı mı? (0.2 birim kala ulaştı sayıyoruz)
        if (Vector3.Distance(transform.position, target.transform.position) < 0.2f)
        {
            Destroy(gameObject); // Sadece mermiyi yok et
        }
    }
}