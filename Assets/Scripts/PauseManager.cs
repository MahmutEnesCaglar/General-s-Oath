using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Header("Pause UI")]
    public GameObject pausePanel;
    public Button pauseButton;
    public Image darkOverlay;
    
    [Header("Pause Button Görselleri")]
    public Sprite pauseNormalSprite;
    public Sprite pauseActiveSprite;
    
    [Header("Panel İçi Butonlar")]
    public Button homeButton;
    public Button resumeButton;
    
    [Header("Ses Ayarları - Ayrı Kontrol")] // ← DEĞİŞTİ
    public Slider musicSlider;
    public Slider sfxSlider;
    
    [Header("Kamera Kontrolü")]
    public CameraZoom cameraZoom;

    private bool isPaused = false;
    private Image pauseButtonImage;

    void Start()
    {
        if (pauseButton != null)
            pauseButtonImage = pauseButton.GetComponent<Image>();
        
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        if (darkOverlay != null)
            darkOverlay.gameObject.SetActive(false);
        
        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);
        
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        
        if (homeButton != null)
            homeButton.onClick.AddListener(ReturnToMap);
        
        // Music slider yükle
        if (musicSlider != null)
        {
            float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            musicSlider.value = savedMusic;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        
        // SFX slider yükle
        if (sfxSlider != null)
        {
            float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxSlider.value = savedSFX;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        
        Debug.Log("PauseManager başlatıldı");
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    void PauseGame()
    {
        isPaused = true;
        
        if (pausePanel != null)
            pausePanel.SetActive(true);
        
        if (darkOverlay != null)
            darkOverlay.gameObject.SetActive(true);
        
        Time.timeScale = 0f;
        
        if (pauseButtonImage != null && pauseActiveSprite != null)
            pauseButtonImage.sprite = pauseActiveSprite;
        
        if (cameraZoom != null)
            cameraZoom.SetCameraControlsEnabled(false);
        
        Debug.Log("Oyun duraklatıldı");
    }

    public void ResumeGame()
    {
        isPaused = false;
        
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        if (darkOverlay != null)
            darkOverlay.gameObject.SetActive(false);
        
        Time.timeScale = 1f;
        
        if (pauseButtonImage != null && pauseNormalSprite != null)
            pauseButtonImage.sprite = pauseNormalSprite;
        
        if (cameraZoom != null)
            cameraZoom.SetCameraControlsEnabled(true);
        
        Debug.Log("Oyun devam ediyor");
    }

    public void SetMusicVolume(float volume)
    {
        MusicManager musicManager = FindFirstObjectByType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.SetMusicVolume(volume);
        }
        
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        SFXManager sfxManager = SFXManager.Instance;
        if (sfxManager != null)
        {
            sfxManager.SetSFXVolume(volume);
        }
        
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void ReturnToMap()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("WorldMap");
    }
}