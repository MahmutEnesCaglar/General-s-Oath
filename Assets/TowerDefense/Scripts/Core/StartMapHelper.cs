using UnityEngine;
using TowerDefense.Core;

/// <summary>
/// Sadece oyunu başlatır - başka bir şey yapmaz
/// </summary>
public class StartMapHelper : MonoBehaviour
{
    void Start()
    {
        // Sadece oyunu başlat
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
            Debug.Log("Oyun başlatıldı! Start Wave butonuna basarak wave başlatın.");
        }
    }
}
