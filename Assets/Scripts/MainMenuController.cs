using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Ana Menü Butonları")]
    public Button playButton;
    public Button quitButton;
    
    [Header("Ses Ayarları - Ayrı Kontrol")]
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Buton click event'lerini bağla
        if (playButton != null)
            playButton.onClick.AddListener(PlayGame);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        
        // Music slider ayarla
        if (musicSlider != null)
        {
            float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            musicSlider.value = savedMusic;
            SetMusicVolume(savedMusic); // Başlangıçta uygula
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        
        // SFX slider ayarla
        if (sfxSlider != null)
        {
            float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxSlider.value = savedSFX;
            SetSFXVolume(savedSFX); // Başlangıçta uygula
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    // PLAY
    public void PlayGame()
    {
        Debug.Log("Oyun başlatılıyor...");
        // SceneManager.LoadScene("WorldMap");
        SceneTransition.Instance.LoadScene("WorldMap");
    }

    // QUIT
    public void QuitGame()
    {
        Debug.Log("Oyundan çıkılıyor...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    /// <summary>
    /// Müzik sesini ayarla (sadece müzik)
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        MusicManager musicManager = FindFirstObjectByType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.SetMusicVolume(volume);
            Debug.Log($"Müzik seviyesi: {volume:F2}");
        }
        
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// SFX sesini ayarla (oyun sesleri) - MÜZİĞE DOKUNMA!
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        // SADECE SFXManager'ı güncelle
        SFXManager sfxManager = SFXManager.Instance;
        if (sfxManager != null)
        {
            sfxManager.SetSFXVolume(volume);
        }
        
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
        
        Debug.Log($"SFX seviyesi: {volume:F2}");
    }
}