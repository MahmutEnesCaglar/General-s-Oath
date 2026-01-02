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
        // SceneManager.LoadScene("MainMenuSahne"); // veya SceneManager.LoadScene(0);
        SceneTransition.Instance.LoadScene("MainMenuSahne");
    }
}