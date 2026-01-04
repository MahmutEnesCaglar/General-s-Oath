using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldMapManager : MonoBehaviour
{
    [Header("Butonlar")]
    public Button mainMenuButton;
    
    void Start()
    {
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }
    
    public void ReturnToMainMenu()
    {
        Debug.Log("Ana menüye dönülüyor...");
        
        // NULL check
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene("MainMenuSahne");
        else
        {
            Debug.LogWarning("SceneTransition yok, normal yani animasyonsuz yükleme yapılıyor...");
            SceneManager.LoadScene("MainMenuSahne");
        }
    }
}