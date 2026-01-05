using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldMapManager : MonoBehaviour
{
    [Header("Yöneticiler")]
    public Button mainMenuButton;
    public MapSelectionManager mapSelectionManager;

    [Header("--- GRİFON (Bölüm 1) ---")]
    public Button grifonButton;
    public Image grifonImage;
    // Grifon hep açık, kilitli haline gerek yok
    public Sprite grifonCurrentSprite; // Şu an oynanan
    public Sprite grifonPassedSprite;  // Bitirilen

    [Header("--- KİRİN (Bölüm 2) ---")]
    public Button kirinButton;
    public Image kirinImage;
    [Space(5)]
    public Sprite kirinLockedSprite;  // KİLİTLİ (Gri veya üzerinde kilit çizili resim)
    public Sprite kirinCurrentSprite; // ŞİMDİKİ (Sıradaki hedef)
    public Sprite kirinPassedSprite;  // GEÇİLEN (Bitmiş)
    
    [Header("--- EJDERHA (Bölüm 3) ---")]
    public Button ejderhaButton;
    public Image ejderhaImage;
    [Space(5)]
    public Sprite ejderhaLockedSprite; 
    public Sprite ejderhaCurrentSprite;
    public Sprite ejderhaPassedSprite; 
    
    void Start()
    {
        if (mainMenuButton != null) 
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        
        // Buton Tıklamaları
        if (grifonButton != null && mapSelectionManager != null)
            grifonButton.onClick.AddListener(mapSelectionManager.OpenGrifonMap);
            
        if (kirinButton != null && mapSelectionManager != null)
            kirinButton.onClick.AddListener(mapSelectionManager.OpenKirinMap);
            
        if (ejderhaButton != null && mapSelectionManager != null)
            ejderhaButton.onClick.AddListener(mapSelectionManager.OpenEjderhaMap);
        
        // Görselleri Güncelle
        UpdateMapVisuals();
    }
    
    void UpdateMapVisuals()
    {
        // 1. GRİFON (Hep açık)
        int grifonStars = PlayerPrefs.GetInt("Map_Grifon_Stars", 0);
        // Kilitli sprite yok (null), kilitli değil (true)
        UpdateOneMapVisual(grifonImage, true, grifonStars, 
            grifonCurrentSprite, grifonPassedSprite, null);

        // 2. KİRİN
        int kirinStars = PlayerPrefs.GetInt("Map_Kirin_Stars", 0);
        bool isKirinUnlocked = PlayerPrefs.GetInt("Map_Kirin_Unlocked", 0) == 1;

        UpdateOneMapVisual(kirinImage, isKirinUnlocked, kirinStars, 
            kirinCurrentSprite, kirinPassedSprite, kirinLockedSprite);

        // 3. EJDERHA
        int ejderhaStars = PlayerPrefs.GetInt("Map_Ejderha_Stars", 0);
        bool isEjderhaUnlocked = PlayerPrefs.GetInt("Map_Ejderha_Unlocked", 0) == 1;

        UpdateOneMapVisual(ejderhaImage, isEjderhaUnlocked, ejderhaStars, 
            ejderhaCurrentSprite, ejderhaPassedSprite, ejderhaLockedSprite);
    }
    
    /// <summary>
    /// Sadece Sprite değiştirerek durumu yönetir. Ekstra ikon yoktur.
    /// </summary>
    void UpdateOneMapVisual(Image mapImg, bool isUnlocked, int starCount, 
                            Sprite currentSprite, Sprite passedSprite, Sprite lockedSprite)
    {
        if (mapImg == null) return;

        // --- DURUM 1: KİLİTLİ ---
        if (!isUnlocked)
        {
            // Eğer inspector'a kilitli resim koyduysan onu göster
            if (lockedSprite != null) 
            {
                mapImg.sprite = lockedSprite;
                mapImg.color = Color.white; // Resim zaten griyse rengi beyaz kalsın
            }
            else 
            {
                // Resim yoksa, mevcut resmi grileştir (Kodla karartma)
                mapImg.color = Color.gray; 
            }
            return;
        }

        // --- KİLİT AÇIK İSE ---
        mapImg.color = Color.white; // Rengi normale döndür

        // --- DURUM 2: GEÇİLEN (Yıldız > 0) ---
        if (starCount > 0)
        {
            if (passedSprite != null) mapImg.sprite = passedSprite;
        }
        // --- DURUM 3: ŞİMDİKİ (Yıldız == 0) ---
        else
        {
            if (currentSprite != null) mapImg.sprite = currentSprite;
        }
    }
    
    public void ReturnToMainMenu()
    {
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene("MainMenuSahne");
        else
            SceneManager.LoadScene("MainMenuSahne");
    }
}