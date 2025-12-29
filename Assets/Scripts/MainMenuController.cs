using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Ana Menü Butonları")]
    public Button playButton;
    public Button quitButton;
    
    [Header("Ses Ayarı")]
    public Slider volumeSlider;

    void Start()
    {
        // Buton click event'lerini bağla
        if (playButton != null)
            playButton.onClick.AddListener(PlayGame);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        
        // Volume slider ayarla
        if (volumeSlider != null)
        {
            // Kaydedilmiş ses seviyesini yükle ve uygula
            if (PlayerPrefs.HasKey("MasterVolume"))
            {
                float savedVolume = PlayerPrefs.GetFloat("MasterVolume");
                volumeSlider.value = savedVolume;
                SetVolume(savedVolume); // Başlangıçta ses seviyesini uygula
            }
            else
            {
                volumeSlider.value = 0.5f; // Varsayılan
                SetVolume(0.5f); // Varsayılan ses seviyesini uygula
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
}
