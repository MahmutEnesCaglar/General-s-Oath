using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    [Header("Ana Menü Butonları")]
    public Button playButton;
    public Button optionsButton;
    public Button creditsButton;
    public Button quitButton;
    
    [Header("Ses Ayarı")]
    public Slider volumeSlider;

    void Start()
    {
        // Panelleri başlangıçta gizle
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        
        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        // Buton click event'lerini bağla
        if (playButton != null)
            playButton.onClick.AddListener(PlayGame);
        
        if (optionsButton != null)
            optionsButton.onClick.AddListener(OpenOptions);
        
        if (creditsButton != null)
            creditsButton.onClick.AddListener(OpenCredits);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        
        // Volume slider ayarla
        if (volumeSlider != null)
        {
            // Kaydedilmiş ses seviyesini yükle
            if (PlayerPrefs.HasKey("MasterVolume"))
            {
                float savedVolume = PlayerPrefs.GetFloat("MasterVolume");
                volumeSlider.value = savedVolume;
            }
            else
            {
                volumeSlider.value = 0.5f; // Varsayılan
            }
            
            // Slider event'ini bağla
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    // PLAY
    public void PlayGame()
    {
        Debug.Log("Oyun başlatılıyor...");
        SceneManager.LoadScene("Map_Grifon");
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

    // OPTIONS
    public void OpenOptions()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        // Global ses (SFX için)
        AudioListener.volume = volume;
        
        // MusicManager'ı bul ve müzik sesini ayarla
        MusicManager musicManager = FindFirstObjectByType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.SetMusicVolume(volume);
            Debug.Log($"MainMenu - Müzik seviyesi güncellendi: {volume:F2}");
        }
        else
        {
            Debug.LogWarning("MusicManager bulunamadı!");
        }
        
        // Ses seviyesini kaydet
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
        
        Debug.Log($"MainMenu - Ses seviyesi: {volume:F2}");
    }

    // CREDITS
    public void OpenCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }
}