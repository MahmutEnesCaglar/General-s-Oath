using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MapSelectionManager : MonoBehaviour
{
    [Header("Popup")]
    public GameObject selectionPopup;
    public Image darkOverlay;
    
    [Header("Map Info")]
    public TMP_Text mapTitle;
    public Image star1;
    public Image star2;
    public Image star3;
    
    [Header("Yıldız Sprite'ları")]
    public Sprite starEmpty;
    public Sprite starFilled;
    
    [Header("Butonlar")]
    public Button startButton;
    public Button closeButton;
    
    [Header("Kilit Popup (Opsiyonel)")]
    public GameObject lockedPopup; // "Bu harita kilitli!" mesajı için
    
    private string selectedMapScene = "";
    
    void Start()
    {
        if (selectionPopup != null)
            selectionPopup.SetActive(false);
        
        if (darkOverlay != null)
            darkOverlay.gameObject.SetActive(false);
        
        if (lockedPopup != null)
            lockedPopup.SetActive(false);
        
        if (startButton != null)
            startButton.onClick.AddListener(StartMap);
        
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePopup);
    }
    
    /// <summary>
    /// Harita butonlarından çağrılır
    /// </summary>
    public void OpenMapSelection(string mapName, string sceneName, int stars)
    {
        selectedMapScene = sceneName;
        
        // Map başlığı
        if (mapTitle != null)
            mapTitle.text = mapName;
        
        // Yıldızları güncelle
        UpdateStars(stars);
        
        // Popup aç
        if (selectionPopup != null)
            selectionPopup.SetActive(true);
        
        if (darkOverlay != null)
            darkOverlay.gameObject.SetActive(true);
    }
    
    void UpdateStars(int starCount)
    {
        if (star1 != null)
            star1.sprite = starCount >= 1 ? starFilled : starEmpty;
        
        if (star2 != null)
            star2.sprite = starCount >= 2 ? starFilled : starEmpty;
        
        if (star3 != null)
            star3.sprite = starCount >= 3 ? starFilled : starEmpty;
    }
    
    void StartMap()
    {
        if (!string.IsNullOrEmpty(selectedMapScene))
        {
            Debug.Log($"Harita başlatılıyor: {selectedMapScene}");
            if (SceneTransition.Instance != null)
                SceneTransition.Instance.LoadScene(selectedMapScene);
            else
            {
                SceneManager.LoadScene(selectedMapScene);
                Debug.LogWarning("SceneTransition yok, animasyonsuz yükleme yapılıyor...");
            }
        }
    }
    
    /// <summary>
    /// Grifon haritası için wrapper (Her zaman açık)
    /// </summary>
    public void OpenGrifonMap()
    {
        int savedStars = GetSavedStars("Map_Grifon");
        OpenMapSelection("GRİFON HARITASI", "Map_Grifon", savedStars);
    }

    /// <summary>
    /// Kirin haritası için wrapper (Grifon tamamlanınca açılır)
    /// </summary>
    public void OpenKirinMap()
    {
        // Kilit kontrolü
        if (PlayerPrefs.GetInt("Map_Kirin_Unlocked", 0) == 1)
        {
            int savedStars = GetSavedStars("Map_Kirin");
            OpenMapSelection("KİRİN HARITASI", "Map_Kirin", savedStars);
        }
        else
        {
            ShowLockedMessage("Kirin haritası kilitli! Grifon'u tamamlayın.");
        }
    }

    /// <summary>
    /// Ejderha haritası için wrapper (Kirin tamamlanınca açılır)
    /// </summary>
    public void OpenEjderhaMap()
    {
        // Kilit kontrolü
        if (PlayerPrefs.GetInt("Map_Ejderha_Unlocked", 0) == 1)
        {
            int savedStars = GetSavedStars("Map_Ejderha");
            OpenMapSelection("EJDERHA HARITASI", "Map_Ejderha", savedStars);
        }
        else
        {
            ShowLockedMessage("Ejderha haritası kilitli! Kirin'i tamamlayın.");
        }
    }
    
    /// <summary>
    /// Kaydedilmiş yıldız sayısını getirir
    /// </summary>
    int GetSavedStars(string sceneName)
    {
        return PlayerPrefs.GetInt(sceneName + "_Stars", 0);
    }
    
    /// <summary>
    /// Kilitli harita mesajı göster
    /// </summary>
    void ShowLockedMessage(string message)
    {
        Debug.Log($"⚠️ {message}");
        
        // Opsiyonel: Kilit popup göster
        if (lockedPopup != null)
        {
            lockedPopup.SetActive(true);
            
            // Popup içindeki text'i güncelle
            TMP_Text lockedText = lockedPopup.GetComponentInChildren<TMP_Text>();
            if (lockedText != null)
                lockedText.text = message;
            
            // 2 saniye sonra kapat
            StartCoroutine(CloseLockedPopupAfterDelay());
        }
    }
    
    System.Collections.IEnumerator CloseLockedPopupAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        
        if (lockedPopup != null)
            lockedPopup.SetActive(false);
    }
    
    void ClosePopup()
    {
        if (selectionPopup != null)
            selectionPopup.SetActive(false);
        
        if (darkOverlay != null)
            darkOverlay.gameObject.SetActive(false);
    }
}