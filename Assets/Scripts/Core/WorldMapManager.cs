using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldMapManager : MonoBehaviour
{
    [Header("Butonlar")]
    public Button mainMenuButton;
    
    [Header("Grifon Haritası Yıldızları")]
    public Image grifonStar1;
    public Image grifonStar2;
    public Image grifonStar3;
    
    [Header("Kirin Haritası Yıldızları")]
    public Image kirinStar1;
    public Image kirinStar2;
    public Image kirinStar3;
    public GameObject kirinLock; // Kilit ikonu
    
    [Header("Ejderha Haritası Yıldızları")]
    public Image ejderhaStar1;
    public Image ejderhaStar2;
    public Image ejderhaStar3;
    public GameObject ejderhaLock; // Kilit ikonu
    
    [Header("Yıldız Sprite'ları")]
    public Sprite starEmpty;
    public Sprite starFilled;
    
    void Start()
    {
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        
        // Yıldızları güncelle
        UpdateAllMapStars();
    }
    
    void UpdateAllMapStars()
    {
        // Grifon (her zaman açık)
        int grifonStars = PlayerPrefs.GetInt("Map_Grifon_Stars", 0);
        UpdateStars(grifonStar1, grifonStar2, grifonStar3, grifonStars);
        
        // Kirin
        int kirinStars = PlayerPrefs.GetInt("Map_Kirin_Stars", 0);
        bool kirinUnlocked = PlayerPrefs.GetInt("Map_Kirin_Unlocked", 0) == 1;
        UpdateStars(kirinStar1, kirinStar2, kirinStar3, kirinStars);
        if (kirinLock != null) 
            kirinLock.SetActive(!kirinUnlocked);
        
        // Ejderha
        int ejderhaStars = PlayerPrefs.GetInt("Map_Ejderha_Stars", 0);
        bool ejderhaUnlocked = PlayerPrefs.GetInt("Map_Ejderha_Unlocked", 0) == 1;
        UpdateStars(ejderhaStar1, ejderhaStar2, ejderhaStar3, ejderhaStars);
        if (ejderhaLock != null) 
            ejderhaLock.SetActive(!ejderhaUnlocked);
        
        Debug.Log($"🗺️ WorldMap Yıldızlar güncellendi:");
        Debug.Log($"  Grifon: {grifonStars} ⭐");
        Debug.Log($"  Kirin: {kirinStars} ⭐ (Kilitsiz: {kirinUnlocked})");
        Debug.Log($"  Ejderha: {ejderhaStars} ⭐ (Kilitsiz: {ejderhaUnlocked})");
    }
    
    void UpdateStars(Image star1, Image star2, Image star3, int starCount)
    {
        if (star1 != null)
            star1.sprite = starCount >= 1 ? starFilled : starEmpty;
        
        if (star2 != null)
            star2.sprite = starCount >= 2 ? starFilled : starEmpty;
        
        if (star3 != null)
            star3.sprite = starCount >= 3 ? starFilled : starEmpty;
    }
    
    public void ReturnToMainMenu()
    {
        Debug.Log("Ana menüye dönülüyor...");
        
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene("MainMenuSahne");
        else
        {
            Debug.LogWarning("SceneTransition yok, normal yani animasyonsuz yükleme yapılıyor...");
            SceneManager.LoadScene("MainMenuSahne");
        }
    }
}