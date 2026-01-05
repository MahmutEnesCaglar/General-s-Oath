using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Referanslar")]
    public CanvasGroup featherLeftCanvasGroup;
    public CanvasGroup featherRightCanvasGroup;
    public Image featherLeftImage;
    public Image featherRightImage;
    public TextMeshProUGUI buttonText;
    
    [Header("Sol Tüy Sprite'ları")]
    public Sprite featherLeftWhiteSprite;
    public Sprite featherLeftFireSprite;
    
    [Header("Sağ Tüy Sprite'ları")]
    public Sprite featherRightWhiteSprite;
    public Sprite featherRightFireSprite;
    
    [Header("Animasyon")]
    public float fadeSpeed = 8f;
    
    [Header("Text Renkleri")]
    public Color normalTextColor = Color.white;
    public Color hoverTextColor = new Color(1f, 0.87f, 0.68f);
    public Color pressTextColor = new Color(1f, 0.65f, 0.3f);
    
    [Header("Ses Efektleri")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
    
    private bool isHovered = false;
    private bool isPressed = false;
    private AudioSource audioSource;

    void Start()
    {
        // AudioSource ekle
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        ButtonState currentState = ButtonState.Normal;
        
        if (isPressed)
            currentState = ButtonState.Pressed;
        else if (isHovered)
            currentState = ButtonState.Hover;
        
        // Tüy görünürlüğü
        float targetFeatherAlpha = 0f;
        if (currentState == ButtonState.Hover || currentState == ButtonState.Pressed)
            targetFeatherAlpha = 1f;
        
        if (featherLeftCanvasGroup != null)
        {
            featherLeftCanvasGroup.alpha = Mathf.Lerp(
                featherLeftCanvasGroup.alpha,
                targetFeatherAlpha,
                fadeSpeed * Time.deltaTime
            );
        }
        
        if (featherRightCanvasGroup != null)
        {
            featherRightCanvasGroup.alpha = Mathf.Lerp(
                featherRightCanvasGroup.alpha,
                targetFeatherAlpha,
                fadeSpeed * Time.deltaTime
            );
        }
        
        // Sol tüy sprite
        if (featherLeftImage != null)
        {
            Sprite targetLeftSprite = (currentState == ButtonState.Pressed) ? 
                featherLeftFireSprite : featherLeftWhiteSprite;
            
            if (targetLeftSprite != null)
                featherLeftImage.sprite = targetLeftSprite;
        }
        
        // Sağ tüy sprite
        if (featherRightImage != null)
        {
            Sprite targetRightSprite = (currentState == ButtonState.Pressed) ? 
                featherRightFireSprite : featherRightWhiteSprite;
            
            if (targetRightSprite != null)
                featherRightImage.sprite = targetRightSprite;
        }
        
        // Text rengi
        Color targetTextColor = normalTextColor;
        if (currentState == ButtonState.Pressed)
            targetTextColor = pressTextColor;
        else if (currentState == ButtonState.Hover)
            targetTextColor = hoverTextColor;
        
        if (buttonText != null)
        {
            buttonText.color = Color.Lerp(
                buttonText.color,
                targetTextColor,
                fadeSpeed * Time.deltaTime
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        
        // Hover sesi çal - SFXManager ile (Statik metod)
        if (hoverSound != null && audioSource != null)
        {
            SFXManager.PlaySound(hoverSound, audioSource);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        
        // Click sesi çal - SFXManager ile (Statik metod)
        if (clickSound != null && audioSource != null)
        {
            SFXManager.PlaySound(clickSound, audioSource);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    private enum ButtonState
    {
        Normal,
        Hover,
        Pressed
    }
}