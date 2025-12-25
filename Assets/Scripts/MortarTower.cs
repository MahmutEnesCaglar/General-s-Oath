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
        // Ateş etme mantığında bu radius'u kullanacağız
        Debug.Log($"{towerName} {explosionRadius} birimlik alana hasar veren mermi fırlatıyor!");
    }
}