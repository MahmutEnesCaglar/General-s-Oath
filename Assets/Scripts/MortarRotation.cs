using UnityEngine;
using System.Collections.Generic;

public class MortarRotation : MonoBehaviour
{
    [Header("Yön Verileri")]
    public List<DirectionalData> directionDataList = new List<DirectionalData>();

    private SpriteRenderer spriteRenderer;
    
    // Kulemizin erişebilmesi için bu değişkeni ekledik
    [HideInInspector] public int currentSegmentIndex;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void RotateTowards(Vector3 targetPosition)
    {
        if (directionDataList == null || directionDataList.Count == 0) return;

        Vector3 direction = targetPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        // 6 sprite'lı özel mortar mantığın burada kalmalı
        // ... (Önceki yazdığımız 6-sprite seçim kodları) ...
        
        // Önemli: Seçilen indexi sakla
        // currentSegmentIndex = hesaplanan_index; 
        
        // Örnek (senin önceki koduna göre):
        int segmentIndex = Mathf.RoundToInt(angle / 60f) % 6; 
        currentSegmentIndex = segmentIndex;

        spriteRenderer.sprite = directionDataList[segmentIndex].sprite;
        transform.localScale = directionDataList[segmentIndex].scale;
    }

    // İŞTE EKSİK OLAN FONKSİYON:
    public Vector2 GetCurrentFirePointOffset(int index)
    {
        if (index >= 0 && index < directionDataList.Count)
        {
            return directionDataList[index].firePointOffset;
        }
        return Vector2.zero;
    }
}