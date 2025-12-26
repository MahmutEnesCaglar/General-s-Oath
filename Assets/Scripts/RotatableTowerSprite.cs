using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct DirectionalData
{
    public string directionName;
    public Sprite sprite;
    public Vector3 scale;
    public Vector2 firePointOffset; 
}

public class RotatableTowerSprite : MonoBehaviour
{
    [Header("Yön Verileri")]
    public List<DirectionalData> directionDataList = new List<DirectionalData>();

    private SpriteRenderer spriteRenderer;
    
    // Tower.cs'nin hangi yöne bakıldığını bilmesi için bu değişkeni ekledik
    [HideInInspector] public int currentSegmentIndex;

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

        // Hesaplanan indexi hem burada kullanıyoruz hem de dışarıya saklıyoruz
        currentSegmentIndex = Mathf.RoundToInt(angle / 45f) % 8;

        DirectionalData currentData = directionDataList[currentSegmentIndex];

        spriteRenderer.sprite = currentData.sprite;
        transform.localScale = currentData.scale;
    }

    // İsim hatası düzeltildi: directionDataList kullanıldı
    public Vector2 GetCurrentFirePointOffset(int index)
    {
        if (index >= 0 && index < directionDataList.Count)
            return directionDataList[index].firePointOffset;
            
        return Vector2.zero;
    }
}