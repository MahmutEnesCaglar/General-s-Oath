using UnityEngine;
using System.Collections.Generic;

public class Tower : MonoBehaviour
{
    [Header("Temel Ayarlar")]
    public string towerName = "Kule";
    public float range = 5f;
    public float fireRate = 1f;
    public int damage = 10;

    [Header("Hedefleme ve Liste")]
    public List<GameObject> enemiesInRange = new List<GameObject>();
    public GameObject currentTarget; 
    protected float fireCooldown = 0f;

    private RotatableTowerSprite rotatableVisual;

    // Hata buradaydı: 'protected virtual' eklendi
    protected virtual void Start()
    {
        rotatableVisual = GetComponentInChildren<RotatableTowerSprite>();
    }

    protected virtual void Update()
    {
        fireCooldown -= Time.deltaTime;
        UpdateTarget();

        if (currentTarget != null)
        {
            // Eğer standart 8 yönlü sprite scripti varsa çalıştırır
            if (rotatableVisual != null)
                rotatableVisual.RotateTowards(currentTarget.transform.position);

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
        if (currentTarget == null && enemiesInRange.Count > 0)
            currentTarget = enemiesInRange[0];
    }

    protected virtual void Attack()
    {
        Debug.Log(towerName + " ateş ediyor!");
    }

    // --- FİZİK TETİKLEYİCİLERİ ---
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!enemiesInRange.Contains(other.gameObject))
            {
                enemiesInRange.Add(other.gameObject);
                Debug.Log("DÜŞMAN LİSTEYE EKLENDİ!");
            }
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
            if (currentTarget == other.gameObject) currentTarget = null;
            Debug.Log("Düşman menzilden çıktı.");
        }
    }
}