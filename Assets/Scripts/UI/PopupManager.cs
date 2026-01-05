using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    [Header("Popup'lar")]
    public GameObject optionsPopup;
    public GameObject creditsPopup;
    
    [Header("Ana Menü")]
    public GameObject menuContainer;
    
    [Header("Animasyon")]
    public float slideDuration = 0.5f;
    
    private bool isAnimating = false;

    void Start()
    {
        // Popup'ları kapat
        if (optionsPopup != null)
            optionsPopup.SetActive(false);
        
        if (creditsPopup != null)
            creditsPopup.SetActive(false);
    }

    public void OpenOptions()
    {
        if (isAnimating) return;
        
        // Slider değerlerini güncelle (MainMenuController üzerinden)
        MainMenuController mainMenu = FindFirstObjectByType<MainMenuController>();
        if (mainMenu != null)
        {
            mainMenu.RefreshSliders();
        }
        
        StartCoroutine(OpenPopupAnimation(optionsPopup));
    }

    public void OpenCredits()
    {
        if (isAnimating) return;
        
        StartCoroutine(OpenPopupAnimation(creditsPopup));
    }

    public void ClosePopup(GameObject popup)
    {
        if (isAnimating) return;
        
        StartCoroutine(ClosePopupAnimation(popup));
    }

    System.Collections.IEnumerator OpenPopupAnimation(GameObject popup)
    {
        isAnimating = true;
        
        // Popup'ı aç
        if (popup != null)
            popup.SetActive(true);
        
        CanvasGroup popupCanvas = popup.GetComponent<CanvasGroup>();
        if (popupCanvas == null)
            popupCanvas = popup.AddComponent<CanvasGroup>();
        
        // Fade in
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            
            // Popup fade in
            popupCanvas.alpha = Mathf.Lerp(0, 1, t);
            
            // Menu fade out
            if (menuContainer != null)
            {
                CanvasGroup menuCanvas = menuContainer.GetComponent<CanvasGroup>();
                if (menuCanvas == null)
                    menuCanvas = menuContainer.AddComponent<CanvasGroup>();
                
                menuCanvas.alpha = Mathf.Lerp(1, 0, t);
            }
            
            yield return null;
        }
        
        // Finalize
        popupCanvas.alpha = 1;
        popupCanvas.interactable = true;
        popupCanvas.blocksRaycasts = true;
        
        if (menuContainer != null)
        {
            CanvasGroup menuCanvas = menuContainer.GetComponent<CanvasGroup>();
            if (menuCanvas != null)
                menuCanvas.alpha = 0;
        }
        
        isAnimating = false;
    }

    System.Collections.IEnumerator ClosePopupAnimation(GameObject popup)
    {
        isAnimating = true;
        
        CanvasGroup popupCanvas = popup.GetComponent<CanvasGroup>();
        
        // Fade out
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            
            // Popup fade out
            if (popupCanvas != null)
                popupCanvas.alpha = Mathf.Lerp(1, 0, t);
            
            // Menu fade in
            if (menuContainer != null)
            {
                CanvasGroup menuCanvas = menuContainer.GetComponent<CanvasGroup>();
                if (menuCanvas != null)
                    menuCanvas.alpha = Mathf.Lerp(0, 1, t);
            }
            
            yield return null;
        }
        
        // Finalize
        if (popupCanvas != null)
        {
            popupCanvas.alpha = 0;
            popupCanvas.interactable = false;
            popupCanvas.blocksRaycasts = false;
        }
        
        if (menuContainer != null)
        {
            CanvasGroup menuCanvas = menuContainer.GetComponent<CanvasGroup>();
            if (menuCanvas != null)
                menuCanvas.alpha = 1;
        }
        
        popup.SetActive(false);
        
        isAnimating = false;
    }
}