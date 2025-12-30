using UnityEngine;
using UnityEngine.UI;

public class SimplePauseManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenu;
    public Image overlay;
    
    [Header("Karartma")]
    public float fadeSpeed = 5f;
    
    private bool isPaused = false;
    private float targetAlpha = 0f;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
        
        // Karartma animasyonu
        if (overlay != null)
        {
            Color color = overlay.color;
            color.a = Mathf.Lerp(color.a, targetAlpha, fadeSpeed * Time.unscaledDeltaTime);
            overlay.color = color;
        }
    }
    
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        if (pauseMenu != null)
            pauseMenu.SetActive(true);
        
        if (overlay != null)
        {
            overlay.gameObject.SetActive(true);
            targetAlpha = 0.7f; // %70 karartma
        }
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        
        targetAlpha = 0f;
        
        StartCoroutine(HideOverlayAfterFade());
    }
    
    System.Collections.IEnumerator HideOverlayAfterFade()
    {
        yield return new WaitForSecondsRealtime(1f / fadeSpeed);
        if (overlay != null)
            overlay.gameObject.SetActive(false);
    }
}