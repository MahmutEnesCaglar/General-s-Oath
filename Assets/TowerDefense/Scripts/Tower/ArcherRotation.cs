using UnityEngine;
using System.Collections.Generic;
using TowerDefense.Tower; // Tower scriptini bulabilmesi için şart

[System.Serializable]
public struct ArcherDirectionData
{
    public string directionName; 
    public Sprite sprite;
    public Vector3 scale;
    public Vector2 firePointOffset; // 1. BURASI EKLENDİ
}

public class ArcherRotation : MonoBehaviour
{
    [Header("4 Ara Yön Verisi")]
    public List<ArcherDirectionData> archerDataList = new List<ArcherDirectionData>();

    private SpriteRenderer spriteRenderer;
    
    // 2. BURASI EKLENDİ: Tower_Archer'ın erişebilmesi için
    [HideInInspector] public int currentSegmentIndex;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void RotateTowards(Vector3 targetPosition)
    {
        if (archerDataList == null || archerDataList.Count == 0) return;

        Vector3 direction = targetPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        int segmentIndex = 0;
        
        // 4 Çeyrek mantığı
        if (angle >= 0 && angle < 90) segmentIndex = 0;      // KD (index1)
        else if (angle >= 90 && angle < 180) segmentIndex = 1; // KB (index3)
        else if (angle >= 180 && angle < 270) segmentIndex = 2; // GB (index4)
        else segmentIndex = 3;                                // GD (index0)

        currentSegmentIndex = segmentIndex; // 3. BURASI EKLENDİ

        spriteRenderer.sprite = archerDataList[segmentIndex].sprite;
        transform.localScale = archerDataList[segmentIndex].scale;
    }

    // 4. EKSİK OLAN FONKSİYON BURASI:
    public Vector2 GetCurrentFirePointOffset(int index)
    {
        if (index >= 0 && index < archerDataList.Count)
        {
            return archerDataList[index].firePointOffset;
        }
        return Vector2.zero;
    }
}