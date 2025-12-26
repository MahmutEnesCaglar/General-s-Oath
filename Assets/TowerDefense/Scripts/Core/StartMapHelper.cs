using UnityEngine;
using TowerDefense.Core;

public class StartMapHelper : MonoBehaviour
{
    void Start()
    {
        // İlk haritayı başlat (Grifon - index 0)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartMap(0);
            Debug.Log("StartMapHelper: Harita başlatıldı!");
        }
        else
        {
            Debug.LogError("StartMapHelper: GameManager bulunamadı!");
        }
    }
}
