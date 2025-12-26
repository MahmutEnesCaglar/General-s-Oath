using UnityEngine;

public class MortarProjectile : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 targetPosition;
    
    [Header("Uçuş Ayarları")]
    public float duration = 1.5f; // Hız yerine "kaç saniyede varsın" mantığı daha stabil
    public float arcHeight = 3f;  // Yayın yüksekliği
    
    private float elapsedTime = 0f;

    public void Setup(GameObject _target, int _damage)
    {
        startPosition = transform.position;
        
        if (_target != null)
        {
            targetPosition = _target.transform.position;
        }
        else
        {
            // Hedef yoksa mermiyi hemen imha et ki havada asılı kalmasın
            Destroy(gameObject);
        }
        
        elapsedTime = 0f;
    }

    void Update()
    {
        // Zamanı ilerlet
        elapsedTime += Time.deltaTime;
        float progress = elapsedTime / duration; // 0 ile 1 arası oran

        if (progress <= 1.0f)
        {
            // Yatayda ilerleme (X ve Y düzlemi)
            Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, progress);

            // Dikeyde kavis (Parabol) ekleme
            // Sinüs fonksiyonu 0'dan başlar, 1'e çıkar ve 0'da biter
            float arc = Mathf.Sin(progress * Mathf.PI) * arcHeight;

            // Yeni pozisyonu uygula
            transform.position = new Vector3(currentPos.x, currentPos.y + arc, currentPos.z);
        }
        else
        {
            // Süre dolduysa hedefe varmışızdır
            HitTarget();
        }
    }

    void HitTarget()
    {
        Debug.Log("Havan mermisi patladı!");
        Destroy(gameObject);
    }
}