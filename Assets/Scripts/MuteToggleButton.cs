using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MuteToggleButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Ses Tipi")]
    public bool isMusicButton = true;
    
    [Header("Slider")]
    public Slider volumeSlider;
    
    [Header("İkonlar")]
    public Sprite muteSprite;
    public Sprite lowSprite;
    public Sprite mediumSprite;
    public Sprite highSprite;
    
    [Header("Ses Efektleri")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
    
    private Image buttonImage;
    private bool isMuted = false;
    private float volumeBeforeMute = 1f;
    private AudioSource audioSource;
    
    void Start()
    {
        buttonImage = GetComponent<Image>();
        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        Button button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(ToggleMute);
        
        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        
        UpdateIcon(volumeSlider != null ? volumeSlider.value : 1f);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null && audioSource != null && SFXManager.Instance != null)
            SFXManager.Instance.PlaySFX(hoverSound, audioSource);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null && audioSource != null && SFXManager.Instance != null)
            SFXManager.Instance.PlaySFX(clickSound, audioSource);
    }
    
    public void ToggleMute()
    {
        if (volumeSlider == null) return;
        
        if (isMuted)
        {
            isMuted = false;
            volumeSlider.value = volumeBeforeMute > 0.01f ? volumeBeforeMute : 0.5f;
        }
        else
        {
            isMuted = true;
            volumeBeforeMute = volumeSlider.value;
            volumeSlider.value = 0f;
        }
    }
    
    void OnVolumeChanged(float value)
    {
        if (value > 0.01f && isMuted)
        {
            isMuted = false;
            volumeBeforeMute = value;
        }
        
        UpdateIcon(value);
        
        // Slider değişince ilgili volume'ü güncelle
        MainMenuController menuController = FindFirstObjectByType<MainMenuController>();
        if (menuController != null)
        {
            if (isMusicButton)
            {
                menuController.SetMusicVolume(value);
            }
            else
            {
                menuController.SetSFXVolume(value);
            }
        }
    }
    
    void UpdateIcon(float volume)
    {
        if (buttonImage == null) return;
        
        int percentage = Mathf.RoundToInt(volume * 100);
        
        if (percentage == 0)
        {
            buttonImage.sprite = muteSprite;
            buttonImage.color = new Color(1f, 0.7f, 0.7f);
        }
        else if (percentage <= 33)
        {
            buttonImage.sprite = lowSprite;
            buttonImage.color = Color.white;
        }
        else if (percentage <= 70)
        {
            buttonImage.sprite = mediumSprite;
            buttonImage.color = Color.white;
        }
        else
        {
            buttonImage.sprite = highSprite;
            buttonImage.color = Color.white;
        }
    }
}