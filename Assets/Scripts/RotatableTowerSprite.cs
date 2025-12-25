using UnityEngine;
using System.Collections.Generic;

// Inspector'da görünebilmesi için System.Serializable eklemeliyiz
[System.Serializable]
public struct DirectionalData
{
    public string directionName; // Sadece düzen için (Örn: Sag, Sag-Ust)
    public Sprite sprite;
    public Vector3 scale; // Bu yöne özel ölçek
}

public class RotatableTowerSprite : MonoBehaviour
{
    [Header("Yön Verileri")]
    public List<DirectionalData> directionDataList = new List<DirectionalData>();

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void RotateTowards(Vector3 targetPosition)
    {
        if (directionDataList == null || directionDataList.Count == 0 || spriteRenderer == null)
            return;

        Vector3 direction = targetPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        int segmentIndex = Mathf.RoundToInt(angle / 45f) % 8;

        // SEÇİLEN VERİYİ AL
        DirectionalData currentData = directionDataList[segmentIndex];

        // HEM SPRITE'I HEM SCALE'I GÜNCELLE
        spriteRenderer.sprite = currentData.sprite;
        transform.localScale = currentData.scale;
    }
}