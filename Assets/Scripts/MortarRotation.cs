using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct MortarDirectionData
{
    public string directionName;
    public Sprite sprite;
    public Vector3 scale;
}

public class MortarRotation : MonoBehaviour
{
    [Header("6 Görseli Buraya Diz")]
    public List<MortarDirectionData> mortarData = new List<MortarDirectionData>();

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Oyun başında varsayılan olarak index7 (Güney-Doğu) göster
        SetDefaultDirection("index7");
    }

    public void RotateTowards(Vector3 targetPosition)
    {
        if (mortarData == null || mortarData.Count == 0) return;

        Vector3 direction = targetPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        int segment8 = Mathf.RoundToInt(angle / 45f) % 8;

        string targetIndexName = "";

        // EKSİK İNDEXLERİ YÖNLENDİRME MANTIĞI
        switch (segment8)
        {
            case 0: targetIndexName = "index1"; break; // Sağ -> Sağ-Üst'e bak
            case 1: targetIndexName = "index1"; break; // Sağ-Üst
            case 2: targetIndexName = "index2"; break; // Üst
            case 3: targetIndexName = "index3"; break; // Sol-Üst
            case 4: targetIndexName = "index3"; break; // Sol -> Sol-Üst'e bak
            case 5: targetIndexName = "index5"; break; // Sol-Alt
            case 6: targetIndexName = "index6"; break; // Alt
            case 7: targetIndexName = "index7"; break; // Sağ-Alt
        }

        ApplyDataByName(targetIndexName);
    }

    private void ApplyDataByName(string name)
    {
        foreach (var data in mortarData)
        {
            if (data.directionName == name)
            {
                spriteRenderer.sprite = data.sprite;
                transform.localScale = data.scale;
                return;
            }
        }
    }

    private void SetDefaultDirection(string name)
    {
        ApplyDataByName(name);
    }
}