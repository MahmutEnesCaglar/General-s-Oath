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
    
    private string selectedMapScene = "";
    
    void Start()
    {
        if (selectionPopup != null)
            selectionPopup.SetActive(false);
        
        if (darkOverlay != null)
            darkOverlay.gameObject.SetActive(false);
        
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
                Debug.LogWarning("SceneTransition yok, normal yani animasyonsuz yükleme yapılıyor...");
            }

        }
    }
    /// <summary>
    /// Grifon haritası için wrapper
    /// </summary>
    public void OpenGrifonMap()
    {
        OpenMapSelection("GRİFON HARITASI", "Map_Grifon", 0);
    }

    /// <summary>
    /// Kirin haritası için wrapper
    /// </summary>
    public void OpenKirinMap()
    {
        OpenMapSelection("KİRİN HARITASI", "Map_Kirin", 0);
    }

    /// <summary>
    /// Ejderha haritası için wrapper
    /// </summary>
    public void OpenEjderhaMap()
    {
        OpenMapSelection("EJDERHA HARITASI", "Map_Ejderha", 0);
    }
    
    void ClosePopup()
    {
        if (selectionPopup != null)
            selectionPopup.SetActive(false);
        
        if (darkOverlay != null)
            darkOverlay.gameObject.SetActive(false);
    }
}